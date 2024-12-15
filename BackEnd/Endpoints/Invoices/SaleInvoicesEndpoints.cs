using AutoMapper;
using BackEnd.Context;
using BackEnd.DTOs.Invoices;
using BackEnd.Entities.Accounts;
using BackEnd.Entities.AdministrativeTools;
using BackEnd.Entities.Invoices;
using BackEnd.Enums;
using BackEnd.Helpers;
using BackEnd.Repositories;
using BackEnd.Repositories.Accounts;
using BackEnd.Repositories.AdministrativeTools;
using BackEnd.Repositories.Contacts;
using BackEnd.Repositories.Finances;
using BackEnd.Repositories.Invoices;
using BackEnd.Repositories.Products;
using BackEnd.Repositories.Warehouses;
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
            group.MapPost("/", Create).DisableAntiforgery().WithOpenApi();
            return group;
        }

        static async Task<Results<Created, ValidationProblem>> Create(
            [FromBody] CreateSaleInvoiceDTO dto,
            [FromServices] IValidator<CreateSaleInvoiceDTO> validator,
            [FromServices] ISaleInvoiceRepository repository,
            [FromServices] IFiscalSupportRepository fiscalSupportRepository,
            [FromServices] IUnitOfWork unitOfWork,
            [FromServices] IReceivableRepository accountsReceivableRepository,
            IOutputCacheStore outputCacheStore,
            [FromServices] IMapper mapper,
            [FromServices] ApplicationDbContext dbContext)
        {
            var validationResult = await validator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                return (Results<Created, ValidationProblem>)Results.ValidationProblem(validationResult.ToDictionary());
            }

            using var transaction = await dbContext.Database.BeginTransactionAsync();

            try
            {
                var fiscalSupportDB = await fiscalSupportRepository.GetById(dto.FiscalSupportId);
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
                await fiscalSupportRepository.Update(fiscalSupportDB);

                double currencyRate = currencyDB.Rate;
                await CalculateInvoiceTotals(dto.SaleInvoiceItems, saleInvoice, unitOfWork, currencyRate);

                saleInvoice.DeadlineId = 1;
                var invoiceId = await repository.Create(saleInvoice);

                var accountsReceivable = new Receivable
                {
                    SaleInvoiceId = invoiceId,
                    ContactId = saleInvoice.CustomerId,
                    TotalAmountDue = saleInvoice.TotalAmount,
                    TotalOutstanding = 0,
                    Status = AccountStatus.PendingReceivable,
                };

                await accountsReceivableRepository.Create(accountsReceivable);

                await AddInvoiceItems(dto.SaleInvoiceItems, invoiceId, repository, unitOfWork, mapper, currencyRate);
                await ReduceStock(dto, unitOfWork);

                await transaction.CommitAsync();
                await outputCacheStore.EvictByTagAsync("fiscal-supports-get", default);
                await outputCacheStore.EvictByTagAsync("stocks-get", default);
                await outputCacheStore.EvictByTagAsync("receivables-get", default);
                return TypedResults.Created($"/invoices/{invoiceId}");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return HandleException(ex);
            }
        }

        private static async Task ReduceStock(CreateSaleInvoiceDTO dto, IUnitOfWork unitOfWork)
        {
            foreach (var item in dto.SaleInvoiceItems)
            {
                if (item.ItemType == ItemType.Product)
                {
                    var stock = await unitOfWork.StockRepository.GetByItemIdAndWarehouseId(item.ItemId, dto.WarehouseId);

                    if (stock == null)
                    {
                        throw new InvalidOperationException($"Stock not found for ItemId {item.ItemId} in the specified warehouse.");
                    }

                    stock.Qty -= item.Qty;
                    unitOfWork.StockRepository.Update(stock);
                }
            }

            await unitOfWork.SaveChangesAsync();
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
                        var productTaxAmount = product.ItemTaxes.FirstOrDefault(x => x.ItemId == itemDTO.ItemId)?.Tax?.Amount ?? 0;

                        totalQtyItems += itemDTO.Qty;
                        totalItems += 1;
                        totalAmount += productBasePrice * itemDTO.Qty;
                        totalTaxes += (productBasePrice * productTaxAmount / 100) * itemDTO.Qty;
                        break;

                    case ItemType.Service:
                        var service = await unitOfWork.ServiceRepository.GetByIdAsync(itemDTO.ItemId)
                            ?? throw new Exception($"Service with id {itemDTO.ItemId} not found.");

                        var serviceBasePrice = service.PricingItems.First(x => x.ItemId == itemDTO.ItemId).BasePrice / currencyRate;
                        var serviceTaxAmount = service.ItemTaxes.FirstOrDefault(x => x.ItemId == itemDTO.ItemId)?.Tax?.Amount ?? 0;

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
            saleInvoice.TotalPrice = totalPrice + (saleInvoice.Shipping ?? 0);
            saleInvoice.TotalAmount = totalAmount;
            saleInvoice.TotalTaxes = totalTaxes;
        }

        private static async Task AddInvoiceItems(
            IEnumerable<SaleInvoiceItemDTO> items,
            long invoiceId,
            ISaleInvoiceRepository repository,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            double currencyRate)
        {
            foreach (var itemDTO in items)
            {
                switch (itemDTO.ItemType)
                {
                    case ItemType.Product:
                        var product = await unitOfWork.ProductRepository.GetByIdAsync(itemDTO.ItemId)
                            ?? throw new Exception($"Product with id {itemDTO.ItemId} not found.");

                        var productBasePrice = product.PricingItems.First(x => x.ItemId == itemDTO.ItemId).BasePrice / currencyRate;
                        var productTaxAmount = product.ItemTaxes.FirstOrDefault(x => x.ItemId == itemDTO.ItemId)?.Tax?.Amount ?? 0;

                        var productSaleInvoiceItem = mapper.Map<SaleInvoiceItem>(itemDTO);
                        productSaleInvoiceItem.TotalAmount = productBasePrice * itemDTO.Qty;
                        productSaleInvoiceItem.TotalTaxes = (productBasePrice * productTaxAmount / 100) * itemDTO.Qty;
                        productSaleInvoiceItem.TotalPrice = productSaleInvoiceItem.TotalAmount + productSaleInvoiceItem.TotalTaxes;
                        productSaleInvoiceItem.BasePrice = productBasePrice;
                        productSaleInvoiceItem.SaleInvoiceId = invoiceId;

                        await repository.AddInvoiceItem(productSaleInvoiceItem);
                        break;

                    case ItemType.Service:
                        var service = await unitOfWork.ServiceRepository.GetByIdAsync(itemDTO.ItemId)
                            ?? throw new Exception($"Service with id {itemDTO.ItemId} not found.");

                        var serviceBasePrice = service.PricingItems.First(x => x.ItemId == itemDTO.ItemId).BasePrice / currencyRate;
                        var serviceTaxAmount = service.ItemTaxes.FirstOrDefault(x => x.ItemId == itemDTO.ItemId)?.Tax?.Amount ?? 0;

                        var serviceSaleInvoiceItem = mapper.Map<SaleInvoiceItem>(itemDTO);
                        serviceSaleInvoiceItem.TotalAmount = serviceBasePrice * itemDTO.Qty;
                        serviceSaleInvoiceItem.TotalTaxes = (serviceBasePrice * serviceTaxAmount / 100) * itemDTO.Qty;
                        serviceSaleInvoiceItem.TotalPrice = serviceSaleInvoiceItem.TotalAmount + serviceSaleInvoiceItem.TotalTaxes;
                        serviceSaleInvoiceItem.BasePrice = serviceBasePrice;
                        serviceSaleInvoiceItem.SaleInvoiceId = invoiceId;

                        await repository.AddInvoiceItem(serviceSaleInvoiceItem);
                        break;

                    default:
                        throw new InvalidOperationException($"Unknown ItemType {itemDTO.ItemType} for ItemId {itemDTO.ItemId}");
                }
            }
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

        private static ValidationProblem ValidationError(string message)
        {
            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                { "Error", new[] { message } }
            });
        }

        private static ValidationProblem HandleException(Exception ex)
        {
            return ValidationError("An error occurred while creating the sale invoice.");
        }
    }
}