using AutoMapper;
using SITRA.Core.DTOs;
using SITRA.Core.DTOs.Auths;
using SITRA.Core.Entities.Auths;
using SITRA.API.Filters;
using SITRA.API.Helpers;
using SITRA.Core.Helpers;
using SITRA.Core.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace SITRA.API.Endpoints
{
    public static class AuthsEndpoints
    {
        public static RouteGroupBuilder MapAuths(this RouteGroupBuilder group)
        {
            group.MapPost("/register", Register).DisableAntiforgery()
                .AddEndpointFilter<ValidationFilter<CreateRegistrationUserDTO>>()
                .WithOpenApi();
            group.MapPost("/login", Login).DisableAntiforgery()
                .AddEndpointFilter<ValidationFilter<CreateLoginUserDTO>>()
                .WithOpenApi();
            group.MapGet("/validate-opened-petty-cash", ValidateOpenedPettyCashEndpoint)
                .WithOpenApi().RequireAuthorization();

            return group;
        }

        static async Task<Results<Created, BadRequest, BadRequest<IEnumerable<IdentityError>>, ValidationProblem>> Register(
            [FromBody] CreateRegistrationUserDTO dto,
            UserManager<User> userManager,
            IMapper mapper)
        {
            if (dto == null)
                return TypedResults.BadRequest();

            var user = mapper.Map<User>(dto);

            user.TenantId = Guid.NewGuid();

            var result = await userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
                return TypedResults.BadRequest(result.Errors);

            return TypedResults.Created();
        }

        static async Task<Results<Ok<TokenResponseDTO>, BadRequest, BadRequest<string>, ValidationProblem>> Login(
            CreateLoginUserDTO dto,
            [FromServices] SignInManager<User> signInManager,
            [FromServices] UserManager<User> userManager,
            [FromServices] IUnitOfWork unitOfWork,
            IConfiguration configuration)
        {
            var user = await userManager.FindByEmailAsync(dto.Email);

            if (user is null)
            {
                return TypedResults.BadRequest();
            }

            var result = await signInManager.CheckPasswordSignInAsync(user,
                dto.Password, lockoutOnFailure: false);

            var openPettyCash = await unitOfWork.PettyCashRepository.GetOpenPettyCashAsync();

            //if (openPettyCash != null && openPettyCash.User.Email != user.Email)
            //{
            //    return TypedResults.BadRequest($"The petty cash opened by user {openPettyCash.User.Email} must be closed before logging in.");
            //}

            if (result.Succeeded)
            {
                var response = await BuildToken(dto, configuration, userManager);
                return TypedResults.Ok(response);
            }
            else
            {
                return TypedResults.BadRequest();
            }
        }

        private static async Task<Results<Ok, BadRequest<string>>> ValidateOpenedPettyCash(
            HttpContext httpContext,
            [FromServices] IUnitOfWork unitOfWork)
        {
            var userEmail = httpContext.User?.FindFirst("email")?.Value;

            if (string.IsNullOrWhiteSpace(userEmail))
            {
                return TypedResults.BadRequest("No se pudo obtener el usuario desde el token.");
            }

            var openPettyCash = await unitOfWork.PettyCashRepository.GetOpenPettyCashAsync();

            if (openPettyCash == null)
            {
                return TypedResults.BadRequest("Debe abrir la caja chica antes de continuar.");
            }

            if (openPettyCash.User != null &&
                !string.Equals(openPettyCash.User.Email?.Trim(), userEmail.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                return TypedResults.BadRequest(
                    $"El usuario {openPettyCash.User.Email} debe cerrar la caja chica previamente abierta.");
            }

            return TypedResults.Ok();
        }


        private static async Task<Results<Ok, BadRequest<string>>> ValidateOpenedPettyCashEndpoint(
            HttpContext httpContext,
            [FromServices] IUnitOfWork unitOfWork)
        {
            return await ValidateOpenedPettyCash(httpContext, unitOfWork);
        }


        private static async Task<TokenResponseDTO> BuildToken(
            CreateLoginUserDTO dto,
            IConfiguration configuration,
            UserManager<User> userManager)
        {
            var user = await userManager.FindByEmailAsync(dto.Email)
                ?? throw new UnauthorizedAccessException("User not found");

            var claims = new List<Claim>
            {
                new Claim("email", user.Email ?? string.Empty),
                new Claim("userName", user.UserName ?? user.Email ?? "unknown")
            };

            void AddIfNotNull(string type, string? value)
            {
                if (!string.IsNullOrWhiteSpace(value))
                    claims.Add(new Claim(type, value));
            }

            AddIfNotNull("companyName", user.CompanyName);
            AddIfNotNull("rucNumber", user.RucNumber);
            AddIfNotNull("address", user.Address);
            AddIfNotNull("tenantId", user.TenantId.ToString());

            var claimsDb = await userManager.GetClaimsAsync(user);
            claims.AddRange(claimsDb.Where(c => !string.IsNullOrWhiteSpace(c.Value)));

            //var roles = await userManager.GetRolesAsync(user);
            //claims.AddRange(
            //    roles.Where(r => !string.IsNullOrWhiteSpace(r))
            //         .Select(r => new Claim(ClaimTypes.Role, r))
            //);

            var signingKey = Keys.GetAllKeys(configuration).First();

            var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
            var expiration = DateTime.UtcNow.AddYears(1);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: expiration,
                signingCredentials: creds
            );

            return new TokenResponseDTO
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expire = expiration,
                IsAuthenticated = true
            };
        }
    }
}

