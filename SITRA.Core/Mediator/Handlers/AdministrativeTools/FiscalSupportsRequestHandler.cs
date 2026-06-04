using Mediator;
using SITRA.Core.DTOs.AdministrativeTools.FiscalSupports;
using SITRA.Core.Mediator.Commands.AdministrativeTools.FiscalSupports;
using SITRA.Core.Mediator.Queries.AdministrativeTools.FiscalSupports;


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
            services.AddTransient<IRequestHandler<GetDocumentsByTypeQuery, FiscalSupportDTO?>, QueryHandler<GetDocumentsByTypeQuery, FiscalSupportDTO?>>();
            services.AddTransient<IRequestHandler<GetSelectsFiscalSupportsQuery, List<FiscalSupportListDTO>>, GetSelectsFiscalSupportsQueryHandler>();
            services.AddTransient<IRequestHandler<CreateFiscalSupportCommand, CreateFiscalSupportDTO>, CreateCommandHandler<Entities.AdministrativeTools.FiscalSupport, CreateFiscalSupportDTO>>();
            services.AddTransient<IRequestHandler<UpdateFiscalSupportCommand, bool>, UpdateCommandHandler<Entities.AdministrativeTools.FiscalSupport, CreateFiscalSupportDTO>>();

            return services;
        }
    }
}
