using Mediator;
using SITRA.Core.DTOs.Appointments.Appointments;
using SITRA.Core.Mediator.Commands.Appointments.Appointments;
using SITRA.Core.Mediator.Queries.Appointments.Appointments;

namespace SITRA.Core.Mediator.Handlers.Appointments
{
    public static class AppointmentsRequestHandler
    {
        public static IServiceCollection AddAppointmentServices(this IServiceCollection services)
        {
            services.AddTransient<IRequestHandler<ActivateAppointmentCommand, bool>, ToggleCommandHandler<ActivateAppointmentCommand>>();
            services.AddTransient<IRequestHandler<DeactivateAppointmentCommand, bool>, ToggleCommandHandler<DeactivateAppointmentCommand>>();

            services.AddTransient<IRequestHandler<GetAppointmentByIdQuery, AppointmentDTO>, QueryHandler<GetAppointmentByIdQuery, AppointmentDTO>>();
            services.AddTransient<IRequestHandler<CreateAppointmentCommand, CreateAppointmentDTO>, CreateCommandHandler<Entities.Appointments.Appointment, CreateAppointmentDTO>>();
            services.AddTransient<IRequestHandler<UpdateAppointmentCommand, bool>, UpdateCommandHandler<Entities.Appointments.Appointment, CreateAppointmentDTO>>();

            return services;
        }
    }
}

