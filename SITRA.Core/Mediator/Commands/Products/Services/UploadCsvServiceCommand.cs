using AutoMapper;
using FluentValidation;
using Mediator;
using Microsoft.AspNetCore.OutputCaching;
using SITRA.Core.DTOs;
using SITRA.Core.DTOs.Products.Services;
using SITRA.Core.Entities.Products;
using SITRA.Core.Enums;
using SITRA.Core.Interfaces;
using System.Globalization;

namespace SITRA.Core.Mediator.Commands.Products.Services
{
    public class UploadCsvServiceCommand : IRequest<bool>
    {
        public CsvDTO CsvDto { get; }
        public UploadCsvServiceCommand(CsvDTO csvDto)
        {
            CsvDto = csvDto;
        }

        public class Handler : IRequestHandler<UploadCsvServiceCommand, bool>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly IMapper _mapper;
            private readonly IOutputCacheStore _outputCacheStore;
            private readonly IValidator<CreateServiceCsvDTO> _validator;
            private readonly IConfiguration _configuration;

            public Handler(IUnitOfWork unitOfWork, IMapper mapper, IOutputCacheStore outputCacheStore, IValidator<CreateServiceCsvDTO> validator, IConfiguration configuration)
            {
                _unitOfWork = unitOfWork;
                _mapper = mapper;
                _outputCacheStore = outputCacheStore;
                _validator = validator;
                _configuration = configuration;
            }

            public async ValueTask<bool> Handle(UploadCsvServiceCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    var validFileType = _configuration.GetValue<string>("CsvUpload:ValidFileType");
                    var validFileName = _configuration.GetValue<string>("CsvUpload:ServiceFileName");

                    if (request.CsvDto.CsvFile.ContentType != validFileType || request.CsvDto.CsvFile.FileName != validFileName)
                    {
                        return false;
                    }

                    using var reader = new StreamReader(request.CsvDto.CsvFile.OpenReadStream());
                    using var csv = new CsvHelper.CsvReader(reader, CultureInfo.InvariantCulture);

                    var records = csv.GetRecords<CreateServiceCsvDTO>().ToList();
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

                        var service = _mapper.Map<Item>(record);
                        service.ItemType = ItemType.Service;
                        var serviceSaved = await _unitOfWork.ServiceRepository.CreateAsync(service);

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
                            Item = serviceSaved,
                            Pricing = pricing,
                            BasePrice = record.BasePrice,
                            IsPrincipal = true
                        };

                        await _unitOfWork.PricingItemRepository.CreateAsync(pricingItem);
                    }

                    await _unitOfWork.SaveChangesAsync();
                    await _outputCacheStore.EvictByTagAsync("pricing-items-get", cancellationToken);
                    await _outputCacheStore.EvictByTagAsync("pricings-get", cancellationToken);
                    await _outputCacheStore.EvictByTagAsync("services-get", cancellationToken);

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

