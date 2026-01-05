using BackEnd.DTOs.Appointments.Appointments;
using BackEnd.Mediator.Commands.Appointments.Appointments;
using BackEnd.Mediator.Queries.Appointments.Appointments;
using MediatR;

namespace BackEnd.Mediator.Handlers.Appointments
{
    public static class AppointmentsRequestHandler
    {
        public static IServiceCollection AddAppointmentServices(this IServiceCollection services)
        {
            services.AddTransient<IRequestHandler<ActivateAppointmentCommand, bool>, ToggleCommandHandler<ActivateAppointmentCommand>>();
            services.AddTransient<IRequestHandler<DeactivateAppointmentCommand, bool>, ToggleCommandHandler<DeactivateAppointmentCommand>>();
            services.AddTransient<IRequestHandler<GetAllAppointmentsQuery, List<AppointmentListDTO>>, QueryHandler<GetAllAppointmentsQuery, List<AppointmentListDTO>>>();
            services.AddTransient<IRequestHandler<GetAppointmentByIdQuery, AppointmentDTO>, QueryHandler<GetAppointmentByIdQuery, AppointmentDTO>>();
            services.AddTransient<IRequestHandler<CreateAppointmentCommand, CreateAppointmentDTO>, CreateCommandHandler<Entities.Appointments.Appointment, CreateAppointmentDTO>>();
            services.AddTransient<IRequestHandler<UpdateAppointmentCommand, bool>, UpdateCommandHandler<Entities.Appointments.Appointment, CreateAppointmentDTO>>();

            return services;
        }
    }
}
