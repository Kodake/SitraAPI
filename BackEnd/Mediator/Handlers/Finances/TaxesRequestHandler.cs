using BackEnd.DTOs.Finances.Currencies;
using BackEnd.Mediator.Commands.Finances.Currencies;
using BackEnd.Mediator.Queries.Finances.Currencies;
using MediatR;

namespace BackEnd.Mediator.Handlers.Finances
{
    public static class TaxesRequestHandler
    {
        public static IServiceCollection AddCurrencyServices(this IServiceCollection services)
        {
            services.AddTransient<IRequestHandler<ActivateCurrencyCommand, bool>, ToggleCommandHandler<ActivateCurrencyCommand>>();
            services.AddTransient<IRequestHandler<DeactivateCurrencyCommand, bool>, ToggleCommandHandler<DeactivateCurrencyCommand>>();
            services.AddTransient<IRequestHandler<GetCurrencyByIdQuery, CurrencyDTO>, QueryHandler<GetCurrencyByIdQuery, CurrencyDTO>>();
            services.AddTransient<IRequestHandler<GetSelectsCurrenciesQuery, List<CurrencyListDTO>>, QueryHandler<GetSelectsCurrenciesQuery, List<CurrencyListDTO>>>();
            services.AddTransient<IRequestHandler<GetAllCurrenciesQuery, List<CurrencyListDTO>>, QueryHandler<GetAllCurrenciesQuery, List<CurrencyListDTO>>>();
            services.AddTransient<IRequestHandler<CreateCurrencyCommand, CreateCurrencyDTO>, CreateCommandHandler<Entities.Finances.Currency, CreateCurrencyDTO>>();
            services.AddTransient<IRequestHandler<UpdateCurrencyCommand, bool>, UpdateCommandHandler<Entities.Finances.Currency, CreateCurrencyDTO>>();

            return services;
        }
    }
}
