using Mediator;
using Microsoft.AspNetCore.Identity;
using SITRA.Core.DTOs;
using SITRA.Core.DTOs.Auths;
using SITRA.Core.Mediator.Commands.Auths;
using SITRA.Core.Mediator.Queries.Auths;

namespace SITRA.Core.Mediator.Handlers.Auths
{
    public static class AuthsRequestHandler
    {
        public static IServiceCollection AddAuthServices(this IServiceCollection services)
        {
            services.AddTransient<IRequestHandler<LoginUserCommand, TokenResponseDTO?>, AuthCommandHandler<LoginUserCommand, TokenResponseDTO?>>();
            services.AddTransient<IRequestHandler<RefreshTokenCommand, TokenResponseDTO?>, AuthCommandHandler<RefreshTokenCommand, TokenResponseDTO?>>();
            services.AddTransient<IRequestHandler<RegisterUserCommand, IdentityResult>, AuthCommandHandler<RegisterUserCommand, IdentityResult>>();
            services.AddTransient<IRequestHandler<ChangeLanguageCommand, bool>, AuthCommandHandler<ChangeLanguageCommand, bool>>();
            services.AddTransient<IRequestHandler<CreateAssociatedUserCommand, IdentityResult>, AuthCommandHandler<CreateAssociatedUserCommand, IdentityResult>>();
            services.AddTransient<IRequestHandler<ValidateOpenedPettyCashQuery, (bool Success, string? ErrorMessage)>, AuthQueryHandler<ValidateOpenedPettyCashQuery, (bool Success, string? ErrorMessage)>>();
            services.AddTransient<IRequestHandler<ValidateUploadedCertificateRncQuery, (bool Success, string? ErrorMessage)>, AuthQueryHandler<ValidateUploadedCertificateRncQuery, (bool Success, string? ErrorMessage)>>();
            services.AddTransient<IRequestHandler<UpdateAssociatedUserRoleCommand, IdentityResult>, AuthCommandHandler<UpdateAssociatedUserRoleCommand, IdentityResult>>();
            services.AddTransient<IRequestHandler<GetUserQuery, UserDTO?>, AuthQueryHandler<GetUserQuery, UserDTO?>>();
            services.AddTransient<IRequestHandler<UpdateProfileCommand, bool>, AuthCommandHandler<UpdateProfileCommand, bool>>();
            services.AddTransient<IRequestHandler<ForgotPasswordCommand, bool>, AuthCommandHandler<ForgotPasswordCommand, bool>>();
            services.AddTransient<IRequestHandler<ResetPasswordCommand, bool>, AuthCommandHandler<ResetPasswordCommand, bool>>();
            return services;
        }
    }
}
