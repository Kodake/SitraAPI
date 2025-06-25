using AutoMapper;
using BackEnd.DTOs;
using BackEnd.DTOs.Auths;
using BackEnd.Entities.Auths;
using BackEnd.Filters;
using BackEnd.Helpers;
using BackEnd.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BackEnd.Endpoints
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
            return group;
        }

        static async Task<Results<Created, BadRequest, BadRequest<IEnumerable<IdentityError>>, ValidationProblem>> Register(
            [FromBody] CreateRegistrationUserDTO dto,
            UserManager<User> userManager,
            IMapper mapper)
        {
            if (dto == null)
            {
                return TypedResults.BadRequest();
            }

            var user = mapper.Map<User>(dto);
            var result = await userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
            {
                return TypedResults.BadRequest(result.Errors);
            }

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

        private async static Task<TokenResponseDTO> BuildToken(CreateLoginUserDTO dto,
            IConfiguration configuration, UserManager<User> userManager)
        {
            var claims = new List<Claim>
            {
                new Claim("email", dto.Email)
            };

            var user = await userManager.FindByNameAsync(dto.Email);
            var claimsDB = await userManager.GetClaimsAsync(user!);

            claims.AddRange(claimsDB);

            var key = Keys.GetAllKeys(configuration);
            var creds = new SigningCredentials(key.First(), SecurityAlgorithms.HmacSha256);
            var expiration = DateTime.UtcNow.AddYears(1);

            var securityToken = new JwtSecurityToken(issuer: null, audience: null, claims: claims,
                expires: expiration, signingCredentials: creds);

            var token = new JwtSecurityTokenHandler().WriteToken(securityToken);

            return new TokenResponseDTO
            {
                Token = token,
                Expire = expiration,
                IsAuthenticated = true
            };
        }
    }
}
