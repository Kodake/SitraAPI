using MediatR;
using Microsoft.AspNetCore.Identity;
using SITRA.Core.DTOs;
using SITRA.Core.Mediator.Commands.Auths;
using SITRA.Core.Mediator.Queries.Auths;

namespace SITRA.Core.Mediator.Handlers.Auths
{
    public static class AuthsRequestHandler
    {
        public static IServiceCollection AddAuthServices(this IServiceCollection services)
        {
            services.AddTransient<IRequestHandler<LoginUserCommand, TokenResponseDTO?>, AuthCommandHandler<LoginUserCommand, TokenResponseDTO?>>();
            services.AddTransient<IRequestHandler<RegisterUserCommand, IdentityResult>, AuthCommandHandler<RegisterUserCommand, IdentityResult>>();
            services.AddTransient<IRequestHandler<ChangeLanguageCommand, bool>, AuthCommandHandler<ChangeLanguageCommand, bool>>();
            services.AddTransient<IRequestHandler<CreateAssociatedUserCommand, IdentityResult>, AuthCommandHandler<CreateAssociatedUserCommand, IdentityResult>>();
            services.AddTransient<IRequestHandler<ValidateOpenedPettyCashQuery, (bool Success, string? ErrorMessage)>, AuthQueryHandler<ValidateOpenedPettyCashQuery, (bool Success, string? ErrorMessage)>>();
            services.AddTransient<IRequestHandler<UpdateUserRoleCommand, IdentityResult>, AuthCommandHandler<UpdateUserRoleCommand, IdentityResult>>();

            return services;
        }
    }
}
