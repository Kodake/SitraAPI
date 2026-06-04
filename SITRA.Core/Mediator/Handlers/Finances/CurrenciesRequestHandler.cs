using Mediator;
using SITRA.Core.DTOs.Finances.Currencies;
using SITRA.Core.Mediator.Commands.Finances.Currencies;
using SITRA.Core.Mediator.Queries.Finances.Currencies;

namespace SITRA.Core.Mediator.Handlers.Finances
{
    public static class CurrenciesRequestHandler
    {
        public static IServiceCollection AddCurrencyServices(this IServiceCollection services)
        {
            services.AddTransient<IRequestHandler<ActivateCurrencyCommand, bool>, ToggleCommandHandler<ActivateCurrencyCommand>>();
            services.AddTransient<IRequestHandler<DeactivateCurrencyCommand, bool>, ToggleCommandHandler<DeactivateCurrencyCommand>>();
            services.AddTransient<IRequestHandler<GetCurrencyByIdQuery, CurrencyDTO>, QueryHandler<GetCurrencyByIdQuery, CurrencyDTO>>();
            services.AddTransient<IRequestHandler<GetSelectsCurrenciesQuery, List<CurrencyListDTO>>, GetSelectsCurrenciesQueryHandler>();
            services.AddTransient<IRequestHandler<GetAllCurrenciesQuery, List<CurrencyListDTO>>, QueryHandler<GetAllCurrenciesQuery, List<CurrencyListDTO>>>();
            services.AddTransient<IRequestHandler<CreateCurrencyCommand, CreateCurrencyDTO>, CreateCommandHandler<Entities.Finances.Currency, CreateCurrencyDTO>>();
            services.AddTransient<IRequestHandler<UpdateCurrencyCommand, bool>, UpdateCommandHandler<Entities.Finances.Currency, CreateCurrencyDTO>>();

            return services;
        }
    }
}

