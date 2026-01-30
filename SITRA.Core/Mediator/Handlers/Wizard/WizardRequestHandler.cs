using MediatR;
using SITRA.Core.DTOs.Wizard;
using SITRA.Core.Mediator.Commands.Wizard;
using SITRA.Core.Mediator.Queries.Wizard;

namespace SITRA.Core.Mediator.Handlers.Wizard
{
    public static class WizardRequestHandler
    {
        public static IServiceCollection AddWizardServices(this IServiceCollection services)
        {
            services.AddTransient<IRequestHandler<CreateWizardCommand, WizardDTO>, CreateWizardCommandHandler>();
            services.AddTransient<IRequestHandler<IsWizardRequiredQuery, bool>, IsWizardRequiredQueryHandler>();
            return services;
        }
    }
}
