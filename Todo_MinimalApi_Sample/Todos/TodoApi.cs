using Asp.Versioning;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Todo_MinimalApi_Sample.Endpoint;
using Todo_MinimalApi_Sample.Persistance;
using Todo_MinimalApi_Sample.Version;

namespace Todo_MinimalApi_Sample.Todos;

internal static class TodoApi
{
	public static RouteGroupBuilder MapTodos(this IEndpointRouteBuilder routes)
	{
		var group = routes.MapGroup("/api");
		group.WithApiVersionSet(ApiVersioning.Set);
		group.WithTags("Todos");


		group.MapEndpoint<GetTodos>().MapToApiVersion(ApiVersioning.V1);
		group.MapEndpoint<GetTodoById>().MapToApiVersion(ApiVersioning.V1);
		group.MapEndpoint<CreateTodo>().MapToApiVersion(ApiVersioning.V1);
		group.MapEndpoint<UpdateTodo>().MapToApiVersion(ApiVersioning.V1);
		group.MapEndpoint<DeleteTodo>().MapToApiVersion(ApiVersioning.V1);

		return group;
	}
}
public sealed class GetTodos : IEndpoint
{
	public static ApiVersion EndpointVersion => ApiVersioning.V1;
	public static string Pattern => "todos";
	public static HttpMethod Method => HttpMethod.Get;

	public static Delegate Handler => async ([FromServices] TodoDbContext ctx, CancellationToken token) =>
	{
		Console.WriteLine($"DB CONTEXT HERERERE {ctx.Todos}");
		return await ctx.Todos.Select(t => t.AsTodoDto()).AsNoTracking().ToListAsync(token);
	};
}

public sealed class GetTodoById : IEndpoint
{
	public static ApiVersion EndpointVersion => ApiVersioning.V1;
	public static string Pattern => "todos/{id}";
	public static HttpMethod Method => HttpMethod.Get;

	public static Delegate Handler =>
		async Task<Results<Ok<TodoDto>, NotFound>> ([FromServices] TodoDbContext ctx, int id, CancellationToken token) =>
	{
		return await ctx.Todos.FindAsync(new object[] { id }, token) switch
		{
			Todo todo => TypedResults.Ok(todo.AsTodoDto()),
			_ => TypedResults.NotFound()
		};
	};
}

public sealed class CreateTodo : IEndpoint
{
	public static ApiVersion EndpointVersion => ApiVersioning.V1;
	public static string Pattern => "todos";
	public static HttpMethod Method => HttpMethod.Post;

	public static Delegate Handler =>
		async Task<Created<TodoDto>> ([FromServices] TodoDbContext ctx, TodoDto dto, CancellationToken token) =>
		{
			var todo = new Todo
			{
				Title = dto.Title,
				Description = dto.Description,
				IsCompleted = false,
			};

			await ctx.Todos.AddAsync(todo, token);
			await ctx.SaveChangesAsync(token);

			return TypedResults.Created($"/todos/{todo.Id}", todo.AsTodoDto());
		};
}

public sealed class UpdateTodo : IEndpoint
{
	public static ApiVersion EndpointVersion => ApiVersioning.V1;
	public static string Pattern => "todos/{id}";
	public static HttpMethod Method => HttpMethod.Put;

	public static Delegate Handler =>
		async Task<Results<NoContent, NotFound, BadRequest>> ([FromServices] TodoDbContext ctx, int id, TodoDto dto,
			CancellationToken token) =>
		{
			if (id != dto.Id)
			{
				return TypedResults.BadRequest();
			}

			var rowsAffected = await ctx.Todos.Where(x => x.Id == id)
				.ExecuteUpdateAsync(updates =>
					updates.SetProperty(t => t.Description, dto.Description)
						.SetProperty(t => t.Title, dto.Title)
						.SetProperty(t => t.IsCompleted, dto.IsCompleted), token);

			return rowsAffected == 0 ? TypedResults.NotFound() : TypedResults.NoContent();
		};
}
public sealed class DeleteTodo : IEndpoint
{
	public static ApiVersion EndpointVersion => ApiVersioning.V1;
	public static string Pattern => "todos/{id}";
	public static HttpMethod Method => HttpMethod.Delete;

	public static Delegate Handler =>
		async Task<Results<NoContent, NotFound>> ([FromServices] TodoDbContext ctx, int id, CancellationToken token) =>
		{
			var rowsAffected = await ctx.Todos.Where(x => x.Id == id)
				.ExecuteDeleteAsync(token);
			
			return rowsAffected == 0 ? TypedResults.NotFound() : TypedResults.NoContent();
		};
}
