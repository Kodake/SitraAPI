using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.OutputCaching;
using SITRA.Core.DTOs;
using SITRA.Core.DTOs.Products.Products;
using SITRA.Core.Entities.Products;
using SITRA.Core.Enums;
using SITRA.Core.Interfaces;
using System.Globalization;

namespace SITRA.Core.Mediator.Commands.Products.Products
{
    public class UploadCsvProductCommand : IRequest<bool>
    {
        public CsvDTO CsvDto { get; }
        public UploadCsvProductCommand(CsvDTO csvDto)
        {
            CsvDto = csvDto;
        }

        public class Handler : IRequestHandler<UploadCsvProductCommand, bool>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly IMapper _mapper;
            private readonly IOutputCacheStore _outputCacheStore;
            private readonly IValidator<CreateProductCsvDTO> _validator;
            private readonly IConfiguration _configuration;

            public Handler(IUnitOfWork unitOfWork, IMapper mapper, IOutputCacheStore outputCacheStore, IValidator<CreateProductCsvDTO> validator, IConfiguration configuration)
            {
                _unitOfWork = unitOfWork;
                _mapper = mapper;
                _outputCacheStore = outputCacheStore;
                _validator = validator;
                _configuration = configuration;
            }

            public async Task<bool> Handle(UploadCsvProductCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    var validFileType = _configuration.GetValue<string>("CsvUpload:ValidFileType");
                    var validFileName = _configuration.GetValue<string>("CsvUpload:ProductFileName");

                    if (request.CsvDto.CsvFile.ContentType != validFileType || request.CsvDto.CsvFile.FileName != validFileName)
                    {
                        return false;
                    }

                    using var reader = new StreamReader(request.CsvDto.CsvFile.OpenReadStream());
                    using var csv = new CsvHelper.CsvReader(reader, CultureInfo.InvariantCulture);

                    var records = csv.GetRecords<CreateProductCsvDTO>().ToList();
                    var maxFileRecords = _configuration.GetValue<int>("CsvUpload:MaxFileRecords");

                    if (records.Count > maxFileRecords)
                    {
                        return false;
                    }

                    foreach (var record in records)
                    {
                        var validationResult = await _validator.ValidateAsync(record);
                        if (!validationResult.IsValid)
                        {
                            continue;
                        }

                        var product = _mapper.Map<Item>(record);
                        product.ItemType = ItemType.Product;
                        var productSaved = await _unitOfWork.ProductRepository.CreateAsync(product);

                        var pricing = await _unitOfWork.PricingRepository.GetPricingByName(Enum.GetName(PricingName.Principal));
                        if (pricing == null)
                        {
                            pricing = new Pricing
                            {
                                Name = Enum.GetName(PricingName.Principal)?.ToUpper() ?? "PRINCIPAL",
                                PricingType = PricingType.Value
                            };
                            pricing = await _unitOfWork.PricingRepository.CreateAsync(pricing);
                        }

                        var pricingItem = new PricingItem
                        {
                            Item = productSaved,
                            Pricing = pricing,
                            InitialCost = record.InitialCost,
                            BasePrice = record.BasePrice,
                            IsPrincipal = true
                        };

                        await _unitOfWork.PricingItemRepository.CreateAsync(pricingItem);
                    }

                    await _unitOfWork.SaveChangesAsync();
                    await _outputCacheStore.EvictByTagAsync("pricing-items-get", cancellationToken);
                    await _outputCacheStore.EvictByTagAsync("pricings-get", cancellationToken);
                    await _outputCacheStore.EvictByTagAsync("products-get", cancellationToken);

                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }
    }
}

