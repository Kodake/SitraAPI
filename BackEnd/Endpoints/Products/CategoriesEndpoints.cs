using AutoMapper;
using BackEnd.DTOs;
using BackEnd.DTOs.Products.Categories;
using BackEnd.Entities.Products;
using BackEnd.Enums;
using BackEnd.Filters;
using BackEnd.Helpers;
using BackEnd.Repositories;
using BackEnd.Repositories.Products;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace BackEnd.Endpoints
{
    public static class CategoriesEndpoints
    {
        public static RouteGroupBuilder MapCategories(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetAll).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("categories-get"))
                .AddPaginationParamsOpenAPI();
            group.MapGet("/selects", GetSelects).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("categories-get"));
            group.MapGet("/{id:int}", GetById);
            group.MapPost("/", Create).DisableAntiforgery()
                .AddEndpointFilter<ValidationFilter<CreateCategoryDTO>>()
                //.RequireAuthorization()
                .WithOpenApi();
            group.MapPut("/{id:int}", Update).DisableAntiforgery()
                .AddEndpointFilter<ValidationFilter<CreateCategoryDTO>>()
                //.RequireAuthorization()
                .WithOpenApi();
            group.MapPatch("/{id:int}/activate", Activate);
            group.MapDelete("/{id:int}/deactivate", Deactivate);
            //.RequireAuthorization("isAdmin");
            return group;
        }

        static async Task<Ok<List<CategoryListDTO>>> GetAll(
            [FromServices] IUnitOfWork unitOfWork,
            [FromServices] IMapper mapper, PaginationDTO pagination)
        {
            var categories = await unitOfWork.CategoryRepository.GetAllAsync(pagination);
            var categoriesDTO = mapper.Map<List<CategoryListDTO>>(categories);

            return TypedResults.Ok(categoriesDTO);
        }

        static async Task<Ok<List<CategoryListDTO>>> GetSelects(
            [FromServices] IUnitOfWork unitOfWork,
            IMapper mapper, [FromQuery] StatusType status = StatusType.All)
        {
            var categories = await unitOfWork.CategoryRepository.GetAll(status);
            var categoriesDTO = mapper.Map<List<CategoryListDTO>>(categories);

            return TypedResults.Ok(categoriesDTO);
        }

        static async Task<Results<Ok<CategoryDTO>, NotFound>> GetById(
            [FromServices] IUnitOfWork unitOfWork,
            [FromServices] IMapper mapper, int id)
        {
            var category = await unitOfWork.CategoryRepository.GetByIdAsync(id);

            if (category is null)
            {
                return TypedResults.NotFound();
            }

            var categoryDTO = mapper.Map<CategoryDTO>(category);

            return TypedResults.Ok(categoryDTO);
        }

        static async Task<Results<Created<CategoryDTO>, ValidationProblem>> Create([FromBody] CreateCategoryDTO dto,
            [FromServices] IUnitOfWork unitOfWork,
            IOutputCacheStore outputCacheStore,
            [FromServices] IMapper mapper)
        {
            var category = mapper.Map<Category>(dto);

            await unitOfWork.CategoryRepository.CreateAsync(category);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("categories-get", default);

            var categoryDTO = mapper.Map<CategoryDTO>(category);

            return TypedResults.Created($"/categories/{category.Id}", categoryDTO);
        }

        static async Task<Results<Ok, NotFound, ValidationProblem>> Update(int id,           [FromBody] CreateCategoryDTO dto,
            [FromServices] IUnitOfWork unitOfWork,
            IOutputCacheStore outputCacheStore,
            [FromServices] IMapper mapper)
        {
            var categoryDB = await unitOfWork.CategoryRepository.GetByIdAsync(id);

            if (categoryDB is null)
            {
                return TypedResults.NotFound();
            }

            categoryDB.Name = dto.Name;
            categoryDB.Description = dto.Description;
            categoryDB.IsActive = categoryDB.IsActive;

            unitOfWork.CategoryRepository.Update(categoryDB);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("categories-get", default);

            return TypedResults.Ok();
        }

        static async Task<Results<NotFound, Ok>> Activate([FromRoute] int id,
            [FromServices] IUnitOfWork unitOfWork,
            IOutputCacheStore outputCacheStore)
        {
            var categoryDB = await unitOfWork.CategoryRepository.GetByIdAsync(id);

            if (categoryDB is null)
            {
                return TypedResults.NotFound();
            }

            await unitOfWork.CategoryRepository.ActivateAsync(id);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("categories-get", default);

            return TypedResults.Ok();
        }

        static async Task<Results<NotFound, Ok>> Deactivate([FromRoute] int id,
            [FromServices] IUnitOfWork unitOfWork,
            IOutputCacheStore outputCacheStore)
        {
            var categoryDB = await unitOfWork.CategoryRepository.GetByIdAsync(id);

            if (categoryDB is null)
            {
                return TypedResults.NotFound();
            }

            await unitOfWork.CategoryRepository.DeactivateAsync(id);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("categories-get", default);

            return TypedResults.Ok();
        }
    }
}
