using AutoMapper;
using BackEnd.DTOs;
using BackEnd.DTOs.Invoices.SaleInvoices;
using BackEnd.Entities.Accounts;
using BackEnd.Entities.AdministrativeTools;
using BackEnd.Entities.Invoices;
using BackEnd.Enums;
using BackEnd.Helpers;
using BackEnd.Repositories;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace BackEnd.Endpoints.Invoices
{
    public static class SaleInvoicesEndpoints
    {
        public static RouteGroupBuilder MapInvoices(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetAll).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("receivables-get"))
                .AddPaginationParamsOpenAPI();
            group.MapGet("/{id:long}", GetById);
            group.MapPost("/", Create).DisableAntiforgery().WithOpenApi();
            group.MapGet("/{invoiceId}/generate-pdf", GeneratePdf).WithOpenApi();
            return group;
        }

        static async Task<Ok<List<SaleInvoiceListDTO>>> GetAll(
            [FromServices] IUnitOfWork unitOfWork,
            IMapper mapper, PaginationDTO pagination)
        {
            var saleInvoices = await unitOfWork.SaleInvoiceRepository.GetAllAsync(pagination);
            var saleInvoicesDTO = mapper.Map<List<SaleInvoiceListDTO>>(saleInvoices);

            return TypedResults.Ok(saleInvoicesDTO);
        }

        static async Task<Results<Ok<SaleInvoiceDTO>, NotFound>> GetById(
            [FromServices] IUnitOfWork unitOfWork,
            [FromServices] IMapper mapper, long id)
        {
            var saleInvoice = await unitOfWork.SaleInvoiceRepository.GetByIdAsync(id);

            if (saleInvoice is null)
            {
                return TypedResults.NotFound();
            }

            var saleInvoiceDTO = mapper.Map<SaleInvoiceDTO>(saleInvoice);

            return TypedResults.Ok(saleInvoiceDTO);
        }

        static async Task<Results<Created, ValidationProblem>> Create(
            [FromBody] CreateSaleInvoiceDTO dto,
            [FromServices] IValidator<CreateSaleInvoiceDTO> validator,
            [FromServices] IUnitOfWork unitOfWork,
            IOutputCacheStore outputCacheStore,
            [FromServices] IMapper mapper)
        {
            // Validate DTO
            var validationResult = await validator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                return (Results<Created, ValidationProblem>)Results.ValidationProblem(validationResult.ToDictionary());
            }

            try
            {
                // Fetch related data
                var fiscalSupportDB = await unitOfWork.FiscalSupportRepository.GetByIdAsync(dto.FiscalSupportId);
                var currencyDB = await unitOfWork.CurrencyRepository.GetByIdAsync(dto.CurrencyId);

                if (fiscalSupportDB == null || currencyDB == null)
                {
                    return TypedResults.ValidationProblem(new Dictionary<string, string[]>
                    {
                        {
                            nameof(dto.FiscalSupportId),
                            new[] { string.Format(Utilities.NotExistsWith, nameof(dto.FiscalSupportId), dto.FiscalSupportId) }
                        },
                        {
                            nameof(dto.CurrencyId),
                            new[] { string.Format(Utilities.NotExistsWith, nameof(dto.CurrencyId), dto.CurrencyId) }
                        }
                    });
                }

                var saleInvoice = mapper.Map<SaleInvoice>(dto);
                saleInvoice.FiscalSupportNumber = GenerateFiscalSupportNumber(fiscalSupportDB);
                unitOfWork.FiscalSupportRepository.Update(fiscalSupportDB);

                double currencyRate = currencyDB.Rate;
                await CalculateInvoiceTotals(dto.SaleInvoiceItems, saleInvoice, unitOfWork, currencyRate);

                await unitOfWork.SaleInvoiceRepository.CreateAsync(saleInvoice);

                var accountsReceivable = new Receivable
                {
                    SaleInvoice = saleInvoice,
                    ContactId = saleInvoice.CustomerId,
                    TotalAmountDue = saleInvoice.TotalPrice,
                    TotalOutstanding = 0,
                    Status = AccountStatus.PendingReceivable,
                };

                await unitOfWork.ReceivableRepository.CreateAsync(accountsReceivable);
                await AddInvoiceItems(dto.SaleInvoiceItems, saleInvoice, unitOfWork, mapper, currencyRate);
                await ReduceStock(dto, unitOfWork);
                await unitOfWork.SaveChangesAsync();
                await outputCacheStore.EvictByTagAsync("fiscal-supports-get", default);
                await outputCacheStore.EvictByTagAsync("stocks-get", default);
                await outputCacheStore.EvictByTagAsync("receivables-get", default);

                return TypedResults.Created($"/invoices/{saleInvoice.Id}");
            }
            catch (Exception ex)
            {
                return TypedResults.ValidationProblem(new Dictionary<string, string[]>
                {
                    { "Error", new[] { $"An error occurred while creating the sale invoice: {ex.Message}" } }
                });
            }
        }

        private static async Task ReduceStock(CreateSaleInvoiceDTO dto, IUnitOfWork unitOfWork)
        {
            foreach (var item in dto.SaleInvoiceItems)
            {
                if (item.ItemType == ItemType.Product)
                {
                    var stock = await unitOfWork.StockRepository.GetByItemIdAndWarehouseId(item.ItemId, dto.WarehouseId) ?? throw new InvalidOperationException($"Stock not found for ItemId {item.ItemId} in the specified warehouse.");
                    stock.Qty -= item.Qty;
                    unitOfWork.StockRepository.Update(stock);
                }
            }
        }

        private static async Task CalculateInvoiceTotals(
            IEnumerable<SaleInvoiceItemDTO> items,
            SaleInvoice saleInvoice,
            IUnitOfWork unitOfWork,
            double currencyRate)
        {
            double totalQtyItems = 0, totalItems = 0, totalAmount = 0, totalPrice = 0, totalTaxes = 0;

            foreach (var itemDTO in items)
            {
                switch (itemDTO.ItemType)
                {
                    case ItemType.Product:
                        var product = await unitOfWork.ProductRepository.GetByIdAsync(itemDTO.ItemId)
                            ?? throw new Exception($"Product with id {itemDTO.ItemId} not found.");

                        var productBasePrice = product.PricingItems.First(x => x.ItemId == itemDTO.ItemId).BasePrice / currencyRate;
                        var productTaxAmount = product.ItemTaxes.Sum(x => x?.Tax?.Amount) ?? 0;

                        totalQtyItems += itemDTO.Qty;
                        totalItems += 1;
                        totalAmount += productBasePrice * itemDTO.Qty;
                        totalTaxes += (productBasePrice * productTaxAmount / 100) * itemDTO.Qty;
                        break;

                    case ItemType.Service:
                        var service = await unitOfWork.ServiceRepository.GetByIdAsync(itemDTO.ItemId)
                            ?? throw new Exception($"Service with id {itemDTO.ItemId} not found.");

                        var serviceBasePrice = service.PricingItems.First(x => x.ItemId == itemDTO.ItemId).BasePrice / currencyRate;
                        var serviceTaxAmount = service.ItemTaxes.Sum(x => x?.Tax?.Amount) ?? 0;

                        totalQtyItems += itemDTO.Qty;
                        totalItems += 1;
                        totalAmount += serviceBasePrice * itemDTO.Qty;
                        totalTaxes += (serviceBasePrice * serviceTaxAmount / 100) * itemDTO.Qty;
                        break;

                    default:
                        throw new InvalidOperationException($"Unknown ItemType {itemDTO.ItemType} for ItemId {itemDTO.ItemId}");
                }
            }

            totalPrice = totalAmount + totalTaxes;
            saleInvoice.TotalQtyItems = totalQtyItems;
            saleInvoice.TotalItems = totalItems;
            saleInvoice.TotalPrice = saleInvoice.Discount.HasValue && saleInvoice.Discount > 0
                ? (double)((totalPrice + (saleInvoice.Shipping ?? 0))
                   - ((totalPrice + (saleInvoice.Shipping ?? 0)) * saleInvoice.Discount.Value / 100))
                : totalPrice + (saleInvoice.Shipping ?? 0);
            saleInvoice.TotalAmount = totalAmount;
            saleInvoice.TotalTaxes = totalTaxes;
        }

        private static async Task AddInvoiceItems(
            IEnumerable<SaleInvoiceItemDTO> items,
            SaleInvoice saleInvoice,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            double currencyRate)
        {
            foreach (var itemDTO in items)
            {
                var saleInvoiceItem = saleInvoice.SaleInvoiceItems.FirstOrDefault(i => i.ItemId == itemDTO.ItemId);
                if (saleInvoiceItem == null)
                {
                    saleInvoiceItem = mapper.Map<SaleInvoiceItem>(itemDTO);
                    saleInvoice.SaleInvoiceItems.Add(saleInvoiceItem);
                }

                saleInvoiceItem.SaleInvoiceId = saleInvoice.Id;
                saleInvoiceItem.SaleInvoice = saleInvoice;

                var (basePrice, taxAmount) = await GetItemPricingAndTax(itemDTO, unitOfWork, currencyRate);

                saleInvoiceItem.TotalAmount = basePrice * itemDTO.Qty;
                saleInvoiceItem.TotalTaxes = basePrice * taxAmount / 100 * itemDTO.Qty;
                saleInvoiceItem.TotalPrice = saleInvoiceItem.TotalAmount + saleInvoiceItem.TotalTaxes;
                saleInvoiceItem.BasePrice = basePrice;

                await unitOfWork.SaleInvoiceRepository.AddInvoiceItem(saleInvoiceItem);
            }
        }

        private static async Task<(double basePrice, double taxAmount)> GetItemPricingAndTax(
            SaleInvoiceItemDTO itemDTO,
            IUnitOfWork unitOfWork,
            double currencyRate)
        {
            var item = await unitOfWork.ItemRepository.GetByIdAsync(itemDTO.ItemId)
                ?? throw new Exception($"Item with id {itemDTO.ItemId} not found.");

            var basePrice = item.PricingItems.First(x => x.ItemId == itemDTO.ItemId).BasePrice / currencyRate;
            var taxAmount = item.ItemTaxes.Sum(x => x?.Tax?.Amount) ?? 0;

            return (basePrice, taxAmount);
        }

        private static string GenerateFiscalSupportNumber(FiscalSupport fiscalSupport)
        {
            string fiscalSupportName = Enum.GetName(typeof(FiscalSupportType), fiscalSupport.FiscalSupportType) ?? "Unknown";
            char symbolChar = fiscalSupport.Symbol.Length > 0 ? fiscalSupport.Symbol[0] : '?';
            int validSpaceNumber = (int)fiscalSupport.SpaceNumber;
            string repeatedSymbols = new string(symbolChar, validSpaceNumber);
            string sequence = ((int)fiscalSupport.InitialSequence).ToString().PadLeft(validSpaceNumber, symbolChar);
            string finalSymbols = repeatedSymbols[..^validSpaceNumber] + sequence;

            return $"{fiscalSupportName}{finalSymbols}";
        }

        public static async Task<IResult> GeneratePdf(
            [FromRoute] int invoiceId,
            [FromServices] IUnitOfWork unitOfWork,
            [FromServices] IMapper mapper)
        {
            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
            var saleInvoice = await unitOfWork.SaleInvoiceRepository.GetByIdAsync(invoiceId)
                ?? throw new Exception($"Invoice with id {invoiceId} not found.");

            var invoiceDocument = new InvoiceDocumentDTO(saleInvoice);
            var filePath = Path.Combine(Path.GetTempPath(), $"Invoice_{invoiceId}.pdf");
            invoiceDocument.GeneratePdf(filePath);

            var fileBytes = await File.ReadAllBytesAsync(filePath);
            return Results.File(fileBytes, "application/pdf", $"Invoice_{invoiceId}.pdf");
        }
    }
}