using AutoMapper;
using BackEnd.DTOs;
using BackEnd.DTOs.Invoices.Quotations;
using BackEnd.DTOs.Invoices.SaleInvoices;
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
    public static class QuotationsEndpoints
    {
        public static RouteGroupBuilder MapQuotations(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetAll).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("quotations-get"))
                .AddPaginationParamsOpenAPI();
            group.MapGet("/{id:long}", GetById);
            group.MapPost("/", Create).DisableAntiforgery().WithOpenApi();
            group.MapPut("/{id:long}", Update).DisableAntiforgery().WithOpenApi();
            group.MapGet("/{invoiceId}/generate-pdf", GeneratePdf).WithOpenApi();
            return group;
        }

        static async Task<Ok<List<QuotationListDTO>>> GetAll(
            [FromServices] IUnitOfWork unitOfWork,
            IMapper mapper, PaginationDTO pagination)
        {
            var quotations = await unitOfWork.QuotationRepository.GetAllAsync(pagination);
            var quotationsDTO = mapper.Map<List<QuotationListDTO>>(quotations);

            return TypedResults.Ok(quotationsDTO);
        }

        static async Task<Results<Ok<QuotationDTO>, NotFound>> GetById(
            [FromServices] IUnitOfWork unitOfWork,
            [FromServices] IMapper mapper, long id)
        {
            var quotation = await unitOfWork.QuotationRepository.GetByIdAsync(id);

            if (quotation is null)
            {
                return TypedResults.NotFound();
            }

            var quotationDTO = mapper.Map<QuotationDTO>(quotation);

            return TypedResults.Ok(quotationDTO);
        }

        static async Task<Results<Created, ValidationProblem>> Create(
            [FromBody] CreateQuotationDTO dto,
            [FromServices] IValidator<CreateQuotationDTO> validator,
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
                var currencyDB = await unitOfWork.CurrencyRepository.GetByIdAsync(dto.CurrencyId);

                if (currencyDB == null)
                {
                    return TypedResults.ValidationProblem(new Dictionary<string, string[]>
                    {
                        {
                            nameof(dto.CurrencyId),
                            new[] { string.Format(Utilities.NotExistsWith, nameof(dto.CurrencyId), dto.CurrencyId) }
                        }
                    });
                }

                var saleInvoice = mapper.Map<SaleInvoice>(dto);
                saleInvoice.InvoiceType = InvoiceType.Quotation;

                double currencyRate = currencyDB.Rate;
                await CalculateInvoiceTotals(dto.SaleInvoiceItems, saleInvoice, unitOfWork, currencyRate);

                await unitOfWork.SaleInvoiceRepository.CreateAsync(saleInvoice);

                var quotation = new Quotation
                {
                    SaleInvoice = saleInvoice,
                    ContactId = saleInvoice.CustomerId,
                    QuotationNumber = dto.QuotationNumber,
                };

                await unitOfWork.QuotationRepository.CreateAsync(quotation);

                await AddInvoiceItems(dto.SaleInvoiceItems, saleInvoice, unitOfWork, mapper, currencyRate);
                await unitOfWork.SaveChangesAsync();
                await outputCacheStore.EvictByTagAsync("quotations-get", default);

                return TypedResults.Created($"/quotations/{saleInvoice.Id}");
            }
            catch (Exception ex)
            {
                return TypedResults.ValidationProblem(new Dictionary<string, string[]>
                {
                    { "Error", new[] { $"An error occurred while creating the sale invoice: {ex.Message}" } }
                });
            }
        }

        static async Task<Results<Ok, NotFound, ValidationProblem>> Update(
            [FromRoute] long id,
            [FromBody] CreateQuotationDTO dto,
            [FromServices] IValidator<CreateQuotationDTO> validator,
            [FromServices] IUnitOfWork unitOfWork,
            IOutputCacheStore outputCacheStore,
            [FromServices] IMapper mapper)
        {
            // Validate DTO
            var validationResult = await validator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                return (Results<Ok, NotFound, ValidationProblem>)Results.ValidationProblem(validationResult.ToDictionary());
            }

            try
            {
                var quotation = await unitOfWork.QuotationRepository.GetByIdAsync(id);
                if (quotation == null)
                {
                    return TypedResults.NotFound();
                }

                var currencyDB = await unitOfWork.CurrencyRepository.GetByIdAsync(dto.CurrencyId);
                if (currencyDB == null)
                {
                    return TypedResults.ValidationProblem(new Dictionary<string, string[]>
                    {
                        {
                            nameof(dto.CurrencyId),
                            new[] { string.Format(Utilities.NotExistsWith, nameof(dto.CurrencyId), dto.CurrencyId) }
                        }
                    });
                }

                var saleInvoice = quotation.SaleInvoice;
                var existingItems = saleInvoice.SaleInvoiceItems.ToList();
                unitOfWork.SaleInvoiceRepository.DeleteInvoiceItemsRange(existingItems);
                mapper.Map(dto, saleInvoice);
                saleInvoice.InvoiceType = InvoiceType.Quotation;

                double currencyRate = currencyDB.Rate;
                await CalculateInvoiceTotals(dto.SaleInvoiceItems, saleInvoice, unitOfWork, currencyRate);

                unitOfWork.SaleInvoiceRepository.Update(saleInvoice);

                quotation.ContactId = saleInvoice.CustomerId;

                unitOfWork.QuotationRepository.Update(quotation);

                await AddInvoiceItems(dto.SaleInvoiceItems, saleInvoice, unitOfWork, mapper, currencyRate);

                await unitOfWork.SaveChangesAsync();
                await outputCacheStore.EvictByTagAsync("quotations-get", default);

                return TypedResults.Ok();
            }
            catch (Exception ex)
            {
                return TypedResults.ValidationProblem(new Dictionary<string, string[]>
                {
                    { "Error", new[] { $"An error occurred while updating the sale invoice: {ex.Message}" } }
                });
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
                        totalTaxes += productBasePrice * productTaxAmount / 100 * itemDTO.Qty;
                        break;

                    case ItemType.Service:
                        var service = await unitOfWork.ServiceRepository.GetByIdAsync(itemDTO.ItemId)
                            ?? throw new Exception($"Service with id {itemDTO.ItemId} not found.");

                        var serviceBasePrice = service.PricingItems.First(x => x.ItemId == itemDTO.ItemId).BasePrice / currencyRate;
                        var serviceTaxAmount = service.ItemTaxes.Sum(x => x?.Tax?.Amount) ?? 0;

                        totalQtyItems += itemDTO.Qty;
                        totalItems += 1;
                        totalAmount += serviceBasePrice * itemDTO.Qty;
                        totalTaxes += serviceBasePrice * serviceTaxAmount / 100 * itemDTO.Qty;
                        break;

                    default:
                        throw new InvalidOperationException($"Unknown ItemType {itemDTO.ItemType} for ItemId {itemDTO.ItemId}");
                }
            }

            totalPrice = totalAmount + totalTaxes;
            saleInvoice.TotalQtyItems = totalQtyItems;
            saleInvoice.TotalItems = totalItems;
            saleInvoice.TotalPrice = saleInvoice.Discount.HasValue && saleInvoice.Discount > 0
                ? (double)(totalPrice + (saleInvoice.Shipping ?? 0)
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

        public static async Task<IResult> GeneratePdf(
            [FromRoute] int invoiceId,
            [FromServices] IUnitOfWork unitOfWork,
            [FromServices] IMapper mapper)
        {
            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
            var saleInvoice = await unitOfWork.SaleInvoiceRepository.GetByIdAsync(invoiceId)
                ?? throw new Exception($"Quotation with number {invoiceId} not found.");

            var quotationDocument = new QuotationDocumentDTO(saleInvoice);
            var quotationNumber = saleInvoice.Quotation?.QuotationNumber ?? "Unknown";

            var filePath = Path.Combine(Path.GetTempPath(), $"Quotation_{quotationNumber}.pdf");
            quotationDocument.GeneratePdf(filePath);

            var fileBytes = await File.ReadAllBytesAsync(filePath);
            return Results.File(fileBytes, "application/pdf", $"Quotation_{quotationNumber}.pdf");
        }
    }
}