using Microsoft.EntityFrameworkCore;
using Blog4Devs.Data;
using Blog4Devs.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OpenApi;
namespace Blog4Devs.API;

public static class CommentEndpoints
{
    public static void MapCommentEndpoints (this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Comment").WithTags(nameof(Comments));

        group.MapGet("/", async (ApplicationDbContext db) =>
        {
            return await db.Comment.ToListAsync();
        })
        .WithName("GetAllComments")
        .WithOpenApi();

        group.MapGet("/{id}", async Task<Results<Ok<Comments>, NotFound>> (Guid id, ApplicationDbContext db) =>
        {
            return await db.Comment.AsNoTracking()
                .FirstOrDefaultAsync(model => model.Id == id)
                is Comments model
                    ? TypedResults.Ok(model)
                    : TypedResults.NotFound();
        })
        .WithName("GetCommentById")
        .WithOpenApi();

        group.MapPut("/{id}", async Task<Results<Ok, NotFound>> (Guid id, Comments comment, ApplicationDbContext db) =>
        {
            var affected = await db.Comment
                .Where(model => model.Id == id)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(m => m.Id, comment.Id)
                    .SetProperty(m => m.AuthorName, comment.AuthorName)
                    .SetProperty(m => m.Content, comment.Content)
                    .SetProperty(m => m.CreatedAt, comment.CreatedAt)
                    .SetProperty(m => m.PostId, comment.PostId)
                    );
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("UpdateComment")
        .WithOpenApi();

        group.MapPost("/", async (Comments comment, ApplicationDbContext db) =>
        {
            db.Comment.Add(comment);
            await db.SaveChangesAsync();
            return TypedResults.Created($"/api/Comment/{comment.Id}",comment);
        })
        .WithName("CreateComment")
        .WithOpenApi();

        group.MapDelete("/{id}", async Task<Results<Ok, NotFound>> (Guid id, ApplicationDbContext db) =>
        {
            var affected = await db.Comment
                .Where(model => model.Id == id)
                .ExecuteDeleteAsync();
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("DeleteComment")
        .WithOpenApi();
    }
}
