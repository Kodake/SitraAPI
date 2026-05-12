using AutoMapper;
using Microsoft.AspNetCore.OutputCaching;
using SITRA.Core.DTOs.Invoices.SaleInvoices;
using SITRA.Core.Entities.Accounts;
using SITRA.Core.Entities.CashBoxes;
using SITRA.Core.Entities.Invoices;
using SITRA.Core.Enums;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Commands.Invoices.SaleInvoices
{
    /// <summary>
    /// Command that creates an Electronic Credit Note (E34) or Debit Note (E33).
    /// The business flow mirrors CreateSaleInvoiceCommand, but:
    ///   - ConditionType / Payments are optional (annulment notes carry zero amounts).
    ///   - The DGII ECF document includes an InformacionReferencia section populated
    ///     from ModifiedNcfNumber and ModificationCode.
    /// </summary>
    public class CreateNoteCommand(CreateNoteDTO dto) : CreateCommand<Invoice, CreateNoteDTO>(dto), IIdempotentCommand
    {
        public Func<Invoice, Task<(bool isAccepted, string trackId, string status, string securityCode, string rawResponse)>>? DgiiProcess { get; set; }

        public override async Task<CreateNoteDTO> ExecuteAction(
            IUnitOfWork unitOfWork,
            IOutputCacheStore outputCacheStore,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var user = await unitOfWork.UserService.GetUserAsync();
            var pettyCash = await unitOfWork.PettyCashRepository.GetOpenPettyCashAsync();

            var fiscalSupportDB = Dto.FiscalSupportId.HasValue
                ? await unitOfWork.FiscalSupportRepository.GetByIdAsync(Dto.FiscalSupportId.Value)
                : null;

            var currencyDB = await unitOfWork.CurrencyRepository.GetByIdAsync(Dto.CurrencyId);

            // Map base invoice
            var note = mapper.Map<Invoice>(Dto);
            note.InvoiceType = InvoiceType.Sale;
            note.Date = DateTimeOffset.Now;

            // Carry note-specific reference fields through to the entity (NotMapped, used by ECF mapper)
            note.ModifiedNcfNumber = Dto.ModifiedNcfNumber;
            note.ModificationCode  = Dto.ModificationCode ?? 3; // Default: 1 = Anula el NCF modificado

            if (Dto.CustomerId != null)
                note.InvoiceBuyer = new InvoiceBuyer { ContactId = Dto.CustomerId.Value };

            if (Dto.BillerId != null)
                note.InvoiceBiller = new InvoiceBiller { ContactId = Dto.BillerId.Value };

            if (fiscalSupportDB != null)
            {
                fiscalSupportDB.IncrementSequence();
                note.FiscalSupportNumber = fiscalSupportDB.GenerateFiscalSupportNumber();
                note.InvoiceFiscalSupport = new InvoiceFiscalSupport { FiscalSupportId = fiscalSupportDB.Id };
                unitOfWork.FiscalSupportRepository.Update(fiscalSupportDB);
            }

            double currencyRate = currencyDB!.Rate;
            await CalculateInvoiceTotals(Dto.InvoiceItems, note, unitOfWork, currencyRate);

            await unitOfWork.SaleInvoiceRepository.CreateAsync(note);
            await AddInvoiceItems(Dto.InvoiceItems, note, unitOfWork, mapper, currencyRate);

            // First save to generate IDs
            await unitOfWork.SaveChangesAsync();

            // ── DGII Workflow ──────────────────────────────────────────────────────
            if (DgiiProcess != null && !string.IsNullOrEmpty(note.FiscalSupportNumber))
            {
                var fullNote = await unitOfWork.SaleInvoiceRepository.GetByIdAsync(note.Id, cancellationToken);

                // Re-attach the NotMapped reference fields so the ECF mapper can read them
                fullNote!.ModifiedNcfNumber = note.ModifiedNcfNumber;
                fullNote.ModificationCode   = note.ModificationCode;

                var dgiiResult = await DgiiProcess(fullNote!);

                note.DgiiTrackId  = string.IsNullOrEmpty(dgiiResult.trackId) ? null : dgiiResult.trackId;
                note.DgiiStatus   = dgiiResult.status;
                note.DgiiResponse = dgiiResult.rawResponse;
                note.SecurityCode = dgiiResult.securityCode;

                unitOfWork.SaleInvoiceRepository.Update(note);
                await unitOfWork.SaveChangesAsync();

                if (!dgiiResult.isAccepted)
                {
                    Dto.Id = note.Id;
                    return Dto;
                }
            }

            // ── Payment / Accounting (optional for notes) ──────────────────────────
            if (Dto.ConditionType.HasValue)
            {
                var payments = Dto.Payments.Select(p => mapper.Map<Payment>(p)).ToList();

                switch (Dto.ConditionType.Value)
                {
                    case InvoiceCondition.Credit:
                        var accountsReceivable = new Receivable
                        {
                            Invoice         = note,
                            ContactId       = Dto.CustomerId,
                            TotalAmountDue  = note.TotalPrice,
                            TotalOutstanding = 0,
                            Status          = AccountStatus.PendingReceivable,
                        };
                        await unitOfWork.ReceivableRepository.CreateAsync(accountsReceivable);
                        break;

                    case InvoiceCondition.Spot:
                        var spotPayment = new SpotPayment
                        {
                            Invoice        = note,
                            ContactId      = Dto.CustomerId == Guid.Empty ? null : Dto.CustomerId,
                            AmountReturned = Dto.AmountReturned ?? 0,
                            PaymentDate    = DateTimeOffset.UtcNow,
                            IsActive       = true
                        };
                        await unitOfWork.SpotPaymentRepository.CreateAsync(spotPayment);

                        foreach (var payment in payments)
                        {
                            payment.SpotPayment = spotPayment;
                            await unitOfWork.PaymentRepository.CreateAsync(payment);
                        }
                        break;
                }
            }

            // Notes also reduce stock (items are being credited/reversed)
            await ReduceStock(Dto, unitOfWork);

            if (pettyCash != null)
                await AddPettyCashMovement(unitOfWork, pettyCash, note);

            await unitOfWork.SaveChangesAsync();

            await outputCacheStore.EvictByTagAsync("fiscal-supports-get", cancellationToken);
            await outputCacheStore.EvictByTagAsync("stocks-get", cancellationToken);
            await outputCacheStore.EvictByTagAsync("receivables-get", cancellationToken);
            await outputCacheStore.EvictByTagAsync("spotPayments-get", cancellationToken);
            await outputCacheStore.EvictByTagAsync("pettyCashMovements-get", cancellationToken);

            Dto.Id = note.Id;
            return Dto;
        }

        // ──────────────────────────────────────────────────────────────────────────
        // Helpers (mirrors CreateSaleInvoiceCommand)
        // ──────────────────────────────────────────────────────────────────────────

        private static async Task CalculateInvoiceTotals(
            IEnumerable<SaleInvoiceItemDTO> items,
            Invoice invoice,
            IUnitOfWork unitOfWork,
            double currencyRate)
        {
            double totalQtyItems = 0, totalItems = 0, totalAmount = 0, totalPrice = 0, totalTaxes = 0;

            foreach (var itemDTO in items)
            {
                switch (itemDTO.ItemType)
                {
                    case ItemType.Product:
                        var product = await unitOfWork.ProductRepository.GetByIdAsync(itemDTO.ItemId);
                        var productBasePrice = product!.PricingItems.First(x => x.ItemId == itemDTO.ItemId).BasePrice / currencyRate;
                        var productTaxAmount = product.ItemTaxes.Sum(x => x?.Tax?.Amount) ?? 0;
                        totalQtyItems += itemDTO.Qty; totalItems += 1;
                        totalAmount   += productBasePrice * itemDTO.Qty;
                        totalTaxes    += (productBasePrice * productTaxAmount / 100) * itemDTO.Qty;
                        break;

                    case ItemType.Service:
                        var service = await unitOfWork.ServiceRepository.GetByIdAsync(itemDTO.ItemId);
                        var serviceBasePrice = service!.PricingItems.First(x => x.ItemId == itemDTO.ItemId).BasePrice / currencyRate;
                        var serviceTaxAmount = service.ItemTaxes.Sum(x => x?.Tax?.Amount) ?? 0;
                        totalQtyItems += itemDTO.Qty; totalItems += 1;
                        totalAmount   += serviceBasePrice * itemDTO.Qty;
                        totalTaxes    += (serviceBasePrice * serviceTaxAmount / 100) * itemDTO.Qty;
                        break;

                    case ItemType.Combo:
                        var combo = await unitOfWork.ComboRepository.GetByIdAsync(itemDTO.ItemId);
                        var comboBasePrice = combo!.PricingItems.First(x => x.ItemId == itemDTO.ItemId).BasePrice / currencyRate;
                        var comboTaxAmount = combo.ItemTaxes.Sum(x => x?.Tax?.Amount) ?? 0;
                        totalQtyItems += itemDTO.Qty; totalItems += 1;
                        totalAmount   += comboBasePrice * itemDTO.Qty;
                        totalTaxes    += (comboBasePrice * comboTaxAmount / 100) * itemDTO.Qty;
                        break;
                }
            }

            totalPrice = totalAmount + totalTaxes;
            invoice.TotalQtyItems = totalQtyItems;
            invoice.TotalItems    = totalItems;
            invoice.TotalPrice    = invoice.Discount.HasValue && invoice.Discount > 0
                ? (double)((totalPrice + (invoice.Shipping ?? 0))
                   - ((totalPrice + (invoice.Shipping ?? 0)) * invoice.Discount.Value / 100))
                : totalPrice + (invoice.Shipping ?? 0);
            invoice.TotalAmount = totalAmount;
            invoice.TotalTaxes  = totalTaxes;
        }

        private static async Task AddInvoiceItems(
            IEnumerable<SaleInvoiceItemDTO> items,
            Invoice invoice,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            double currencyRate)
        {
            foreach (var itemDTO in items)
            {
                var invoiceItem = invoice.InvoiceItems.FirstOrDefault(i => i.ItemId == itemDTO.ItemId);
                if (invoiceItem == null)
                {
                    invoiceItem = mapper.Map<InvoiceItem>(itemDTO);
                    invoice.InvoiceItems.Add(invoiceItem);
                }

                invoiceItem.InvoiceId = invoice.Id;
                invoiceItem.Invoice   = invoice;

                var (basePrice, taxAmount) = await GetItemPricingAndTax(itemDTO, unitOfWork, currencyRate);

                invoiceItem.TotalAmount = basePrice * itemDTO.Qty;
                invoiceItem.TotalTaxes  = basePrice * taxAmount / 100 * itemDTO.Qty;
                invoiceItem.TotalPrice  = invoiceItem.TotalAmount + invoiceItem.TotalTaxes;
                invoiceItem.BasePrice   = basePrice;

                await unitOfWork.SaleInvoiceRepository.AddInvoiceItem(invoiceItem);
            }
        }

        private static async Task ReduceStock(CreateNoteDTO dto, IUnitOfWork unitOfWork)
        {
            var stockAdjustments = new Dictionary<Guid, double>();

            foreach (var item in dto.InvoiceItems)
            {
                if (item.ItemType == ItemType.Product)
                {
                    var stock = await unitOfWork.StockRepository.GetByItemIdAndWarehouseId(item.ItemId, dto.WarehouseId);
                    if (!stockAdjustments.ContainsKey(stock!.Id)) stockAdjustments[stock.Id] = 0;
                    stockAdjustments[stock.Id] -= item.Qty;
                }

                if (item.ItemType == ItemType.Combo)
                {
                    var combo = await unitOfWork.ComboRepository.GetByIdAsync(item.ItemId);
                    foreach (var comboItem in combo!.ComboItems)
                    {
                        var stock = await unitOfWork.StockRepository.GetByItemIdAndWarehouseId(comboItem.ChildItemId, dto.WarehouseId);
                        if (!stockAdjustments.ContainsKey(stock!.Id)) stockAdjustments[stock.Id] = 0;
                        stockAdjustments[stock.Id] -= comboItem.Qty * item.Qty;
                    }
                }
            }

            foreach (var adjustment in stockAdjustments)
            {
                var stock = await unitOfWork.StockRepository.GetByIdAsync(adjustment.Key);
                stock!.Qty += adjustment.Value;
                unitOfWork.StockRepository.Update(stock);
            }
        }

        private async Task AddPettyCashMovement(IUnitOfWork unitOfWork, PettyCash pettyCash, Invoice invoice)
        {
            var movement = new PettyCashMovement
            {
                PettyCashId = pettyCash.Id,
                PettyCash   = pettyCash,
                InvoiceId   = invoice.Id,
                Invoice     = invoice,
                Amount      = invoice.TotalPrice,
                Type        = MovementType.Income,
            };

            await unitOfWork.PettyCashMovementRepository.CreateAsync(movement);

            var cashTotal = Dto.Payments
                .Where(x => x.PaymentId == PaymentMethod.Cash)
                .Sum(x => x.Amount);

            pettyCash.ClosingAmount += cashTotal;
            unitOfWork.PettyCashRepository.Update(pettyCash);
        }

        private static async Task<(double basePrice, double taxAmount)> GetItemPricingAndTax(
            SaleInvoiceItemDTO itemDTO,
            IUnitOfWork unitOfWork,
            double currencyRate)
        {
            var item      = await unitOfWork.ItemRepository.GetByIdAsync(itemDTO.ItemId);
            var basePrice = item!.PricingItems.First(x => x.ItemId == itemDTO.ItemId).BasePrice / currencyRate;
            var taxAmount = item.ItemTaxes.Sum(x => x?.Tax?.Amount) ?? 0;
            return (basePrice, taxAmount);
        }
    }
}
