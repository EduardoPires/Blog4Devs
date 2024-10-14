using Microsoft.EntityFrameworkCore;
using Blog4Devs.Data;
using Blog4Devs.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.AspNetCore.Authorization;
namespace Blog4Devs.API;

public static class PostsEndpoints
{
    public static void MapPostsEndpoints (this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Posts").WithTags(nameof(Posts));

        group.MapGet("/", [AllowAnonymous] async (ApplicationDbContext db) =>
        {
            return await db.Posts.ToListAsync();
        }).ProducesValidationProblem()
        .WithName("GetAllPosts")
        .WithOpenApi();

        group.MapGet("/{id}", [AllowAnonymous] async Task<Results<Ok<Posts>, NotFound>> (Guid id, ApplicationDbContext db) =>
        {
            return await db.Posts.AsNoTracking()
                .FirstOrDefaultAsync(model => model.Id == id)
                is Posts model
                    ? TypedResults.Ok(model)
                    : TypedResults.NotFound();
        }).ProducesValidationProblem()
        .WithName("GetPostsById")
        .WithOpenApi();

        group.MapPut("/{id}", [Authorize] async Task<Results<Ok, NotFound>> (Guid id, Posts posts, ApplicationDbContext db) =>
        {
            var affected = await db.Posts
                .Where(model => model.Id == id)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(m => m.Id, posts.Id)
                    .SetProperty(m => m.Title, posts.Title)
                    .SetProperty(m => m.Description, posts.Description)
                    .SetProperty(m => m.Status, posts.Status)
                    .SetProperty(m => m.CreatedBy, posts.CreatedBy)
                    .SetProperty(m => m.CreatedAt, posts.CreatedAt)
                    );
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        }).ProducesValidationProblem()
        .WithName("UpdatePosts")
        .WithOpenApi();

        group.MapPost("/", async (Posts posts, ApplicationDbContext db) =>
        {
            db.Posts.Add(posts);
            await db.SaveChangesAsync();
            return TypedResults.Created($"/api/Posts/{posts.Id}",posts);
        }).ProducesValidationProblem()
        .WithName("CreatePosts")
        .WithOpenApi();

        group.MapDelete("/{id}", [Authorize] async Task<Results<Ok, NotFound>> (Guid id, ApplicationDbContext db) =>
        {
            var affected = await db.Posts
                .Where(model => model.Id == id)
                .ExecuteDeleteAsync();
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .RequireAuthorization("ExcluirPosts")
        .WithName("DeletePosts")
        .WithOpenApi();
    }
}
