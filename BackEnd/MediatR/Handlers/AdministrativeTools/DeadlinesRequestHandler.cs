using BackEnd.DTOs.AdministrativeTools.Deadlines;
using BackEnd.MediatR.Commands.AdministrativeTools.Deadlines;
using BackEnd.MediatR.Queries.AdministrativeTools.Deadlines;
using MediatR;

namespace BackEnd.MediatR.Handlers.AdministrativeTools
{
    public static class DeadlinesRequestHandler
    {
        public static IServiceCollection AddDeadlineServices(this IServiceCollection services)
        {
            services.AddTransient<IRequestHandler<ActivateDeadlineCommand, bool>, ToggleCommandHandler<ActivateDeadlineCommand>>();
            services.AddTransient<IRequestHandler<DeactivateDeadlineCommand, bool>, ToggleCommandHandler<DeactivateDeadlineCommand>>();
            services.AddTransient<IRequestHandler<GetAllDeadlinesQuery, List<DeadlineListDTO>>, QueryHandler<GetAllDeadlinesQuery, List<DeadlineListDTO>>>();
            services.AddTransient<IRequestHandler<GetDeadlineByIdQuery, DeadlineDTO>, QueryHandler<GetDeadlineByIdQuery, DeadlineDTO>>();
            services.AddTransient<IRequestHandler<GetSelectsDeadlinesQuery, List<DeadlineListDTO>>, QueryHandler<GetSelectsDeadlinesQuery, List<DeadlineListDTO>>>();
            services.AddTransient<IRequestHandler<CreateDeadlineCommand, CreateDeadlineDTO>, CreateCommandHandler<Entities.AdministrativeTools.Deadline, CreateDeadlineDTO>>();
            services.AddTransient<IRequestHandler<UpdateDeadlineCommand, bool>, UpdateCommandHandler<Entities.AdministrativeTools.Deadline, CreateDeadlineDTO>>();

            return services;
        }
    }
}
