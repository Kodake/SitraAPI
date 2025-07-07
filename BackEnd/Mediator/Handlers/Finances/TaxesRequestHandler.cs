using BackEnd.DTOs.Finances.Taxes;
using BackEnd.Mediator.Commands.Finances.Taxes;
using BackEnd.Mediator.Queries.Finances.Taxes;
using MediatR;

namespace BackEnd.Mediator.Handlers.Finances
{
    public static class TaxesRequestHandler
    {
        public static IServiceCollection AddTaxServices(this IServiceCollection services)
        {
            services.AddTransient<IRequestHandler<ActivateTaxCommand, bool>, ToggleCommandHandler<ActivateTaxCommand>>();
            services.AddTransient<IRequestHandler<DeactivateTaxCommand, bool>, ToggleCommandHandler<DeactivateTaxCommand>>();
            services.AddTransient<IRequestHandler<GetTaxByIdQuery, TaxDTO>, QueryHandler<GetTaxByIdQuery, TaxDTO>>();
            services.AddTransient<IRequestHandler<GetSelectsTaxesQuery, List<TaxListDTO>>, QueryHandler<GetSelectsTaxesQuery, List<TaxListDTO>>>();
            services.AddTransient<IRequestHandler<GetAllTaxesQuery, List<TaxListDTO>>, QueryHandler<GetAllTaxesQuery, List<TaxListDTO>>>();
            services.AddTransient<IRequestHandler<CreateTaxCommand, CreateTaxDTO>, CreateCommandHandler<Entities.Finances.Tax, CreateTaxDTO>>();
            services.AddTransient<IRequestHandler<UpdateTaxCommand, bool>, UpdateCommandHandler<Entities.Finances.Tax, CreateTaxDTO>>();

            return services;
        }
    }
}
