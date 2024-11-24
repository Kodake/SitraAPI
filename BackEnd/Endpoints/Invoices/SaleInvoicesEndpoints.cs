using AutoMapper;
using BackEnd.Context;
using BackEnd.DTOs.Invoices;
using BackEnd.Entities.AdministrativeTools;
using BackEnd.Entities.Invoices;
using BackEnd.Enums;
using BackEnd.Helpers;
using BackEnd.Repositories.AdministrativeTools;
using BackEnd.Repositories.Contacts;
using BackEnd.Repositories.Finances;
using BackEnd.Repositories.Invoices;
using BackEnd.Repositories.Products;
using BackEnd.Repositories.Warehouses;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

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
            [FromServices] IPricingRepository pricingRepository,
            [FromServices] IContactRepository contactRepository,
            [FromServices] IFiscalSupportRepository fiscalSupportRepository,
            [FromServices] IDeadlineRepository deadlineRepository,
            [FromServices] ICurrencyRepository currencyRepository,
            [FromServices] IProductRepository productRepository,
            [FromServices] IStockRepository stockRepository,
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
                var currencyDB = await currencyRepository.GetById(dto.CurrencyId);

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
                await CalculateInvoiceTotals(dto.SaleInvoiceItems, saleInvoice, productRepository, currencyRate);

                saleInvoice.DeadlineId = 1;
                var invoiceId = await repository.Create(saleInvoice);

                await AddInvoiceItems(dto.SaleInvoiceItems, invoiceId, repository, productRepository, mapper, currencyRate);
                await ReduceStock(dto, stockRepository);

                await transaction.CommitAsync();
                return TypedResults.Created($"/invoices/{invoiceId}");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return HandleException(ex);
            }
        }

        private static async Task ReduceStock(CreateSaleInvoiceDTO dto, IStockRepository stockRepository)
        {
            foreach (var item in dto.SaleInvoiceItems)
            {
                if (item.ItemType == ItemType.Product)
                {
                    var stock = await stockRepository.GetByItemIdAndWarehouseId(item.ItemId, dto.WarehouseId);

                    if (stock == null)
                    {
                        throw new InvalidOperationException($"Stock not found for ItemId {item.ItemId} in the specified warehouse.");
                    }

                    stock.Qty -= item.Qty;
                    await stockRepository.Update(stock);
                }
            }
        }

        private static async Task CalculateInvoiceTotals(
            IEnumerable<SaleInvoiceItemDTO> items,
            SaleInvoice saleInvoice,
            IProductRepository productRepository,
            double currencyRate)
        {
            double totalQtyItems = 0, totalItems = 0, totalAmount = 0, totalPrice = 0, totalTaxes = 0;

            foreach (var itemDTO in items)
            {
                var product = await productRepository.GetById(itemDTO.ItemId)
                               ?? throw new Exception($"Product with id {itemDTO.ItemId} not found.");

                var basePrice = product.PricingItems.First(x => x.ItemId == itemDTO.ItemId).BasePrice / currencyRate;
                var taxAmount = product.ItemTaxes.First(x => x.ItemId == itemDTO.ItemId).Tax.Amount / currencyRate;

                double initialQty = 1;
                totalQtyItems += itemDTO.Qty;
                totalItems += initialQty;
                totalPrice += (basePrice + taxAmount) * itemDTO.Qty;
                totalAmount += (basePrice) * itemDTO.Qty;
                totalTaxes += taxAmount * itemDTO.Qty;
            }

            saleInvoice.TotalQtyItems = totalQtyItems;
            saleInvoice.TotalItems = totalItems;
            saleInvoice.TotalPrice = totalPrice + saleInvoice.Shipping ?? 0;
            saleInvoice.TotalAmount = totalAmount;
            saleInvoice.TotalTaxes = totalTaxes;
        }

        private static async Task AddInvoiceItems(
            IEnumerable<SaleInvoiceItemDTO> items,
            long invoiceId,
            ISaleInvoiceRepository repository,
            IProductRepository productRepository,
            IMapper mapper,
            double currencyRate)
        {
            foreach (var itemDTO in items)
            {
                var product = await productRepository.GetById(itemDTO.ItemId)
                               ?? throw new Exception($"Product with id {itemDTO.ItemId} not found.");

                var saleInvoiceItem = mapper.Map<SaleInvoiceItem>(itemDTO);
                var basePrice = product.PricingItems.First(x => x.ItemId == itemDTO.ItemId).BasePrice;
                var taxes = product.ItemTaxes.First(x => x.ItemId == itemDTO.ItemId).Tax.Amount;

                saleInvoiceItem.SaleInvoiceId = invoiceId;
                saleInvoiceItem.BasePrice = basePrice / currencyRate;
                saleInvoiceItem.TotalPrice = (basePrice + taxes) * itemDTO.Qty;
                saleInvoiceItem.TotalAmount = (basePrice) * itemDTO.Qty;
                saleInvoiceItem.TotalTaxes = taxes * itemDTO.Qty;

                await repository.AddInvoiceItem(saleInvoiceItem);
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