using Bogus;
using Microsoft.EntityFrameworkCore;
using NBL_PoC_Api.Persistance;
using NBL_PoC_Api.Todos;

namespace NBL_PoC_Api.Seeder;

public sealed class TodoGenerator
{
    private readonly TodoDbContext _ctx;
    private readonly Faker<Todo> _todoFaker;

    public TodoGenerator(TodoDbContext ctx, Random random)
    {
        _ctx = ctx;
        Randomizer.Seed = random;
        _todoFaker = new Faker<Todo>()
            .RuleFor(t => t.Id, f => f.IndexFaker)
            .RuleFor(t => t.Title, f => f.Random.Words())
            .RuleFor(t => t.Description, f => f.Lorem.Sentence())
            .RuleFor(t => t.IsCompleted, f => f.Random.Bool());
    }

    public TodoGenerator()
    {
        _todoFaker = new Faker<Todo>()
            .RuleFor(t => t.Title, f => f.Random.Words())
            .RuleFor(t => t.Description, f => f.Lorem.Sentence())
            .RuleFor(t => t.IsCompleted, f => f.Random.Bool());
    }

    public List<Todo> GenerateTodos(int count)
    {
        return _todoFaker.Generate(count);
    }
    public async Task GenerateTodoAndSaveInDatabase()
    {
        var data = _todoFaker.Generate();
        await _ctx.Todos.AddAsync(data);
        await _ctx.SaveChangesAsync();
    }

    public async Task GenerateTodosAndSaveInDatabase(int count)
    {
        var data = _todoFaker.Generate(count).ToArray();
        await _ctx.Todos.AddRangeAsync(data);
        await _ctx.SaveChangesAsync();
    }

    public async Task CleanupTodos()
    {
        await _ctx.Todos.AsNoTracking().ExecuteDeleteAsync(); 
    }
}