using MediatR;
using SITRA.Core.DTOs.AdministrativeTools.FiscalSupports;
using SITRA.Core.Mediator.Commands.AdministrativeTools.FiscalSupports;
using SITRA.Core.Mediator.Queries.AdministrativeTools.FiscalSupports;
using SITRA.Core.Mediator.Queries.Products.Items;

namespace SITRA.Core.Mediator.Handlers.AdministrativeTools
{
    public static class FiscalSupportsRequestHandler
    {
        public static IServiceCollection AddFiscalSupportServices(this IServiceCollection services)
        {
            services.AddTransient<IRequestHandler<ActivateFiscalSupportCommand, bool>, ToggleCommandHandler<ActivateFiscalSupportCommand>>();
            services.AddTransient<IRequestHandler<DeactivateFiscalSupportCommand, bool>, ToggleCommandHandler<DeactivateFiscalSupportCommand>>();
            services.AddTransient<IRequestHandler<GetAllFiscalSupportsQuery, List<FiscalSupportListDTO>>, QueryHandler<GetAllFiscalSupportsQuery, List<FiscalSupportListDTO>>>();
            services.AddTransient<IRequestHandler<GetFiscalSupportByIdQuery, FiscalSupportDTO>, QueryHandler<GetFiscalSupportByIdQuery, FiscalSupportDTO>>();
            services.AddTransient<IRequestHandler<GetDocumentsByTypeIdAndWarehouseIdQuery, FiscalSupportDTO?>, QueryHandler<GetDocumentsByTypeIdAndWarehouseIdQuery, FiscalSupportDTO?>>();
            services.AddTransient<IRequestHandler<GetSelectsFiscalSupportsQuery, List<FiscalSupportListDTO>>, QueryHandler<GetSelectsFiscalSupportsQuery, List<FiscalSupportListDTO>>>();
            services.AddTransient<IRequestHandler<CreateFiscalSupportCommand, CreateFiscalSupportDTO>, CreateCommandHandler<Entities.AdministrativeTools.FiscalSupport, CreateFiscalSupportDTO>>();
            services.AddTransient<IRequestHandler<UpdateFiscalSupportCommand, bool>, UpdateCommandHandler<Entities.AdministrativeTools.FiscalSupport, CreateFiscalSupportDTO>>();

            return services;
        }
    }
}
