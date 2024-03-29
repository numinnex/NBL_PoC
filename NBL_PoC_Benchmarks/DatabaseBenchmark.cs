using System.Data;
using BenchmarkDotNet.Attributes;
using Dapper;
using Microsoft.EntityFrameworkCore;
using NBL_PoC_Api.Persistance;
using NBL_PoC_Api.Seeder;
using NBL_PoC_Api.Todos;

namespace NBL_PoC_DatabaseBenchmark;

[MemoryDiagnoser]
public class DatabaseBenchmark
{
    private TodoDbContext _ctx = null!;
    private IDbConnection _connection = null!;
    private TodoGenerator _generator = null!;
    private Random _random = null!;

    [GlobalSetup]
    public async Task Setup()
    {
        var cs = "Host=localhost;Port=5432;Database=Todo;Username=super_admin;Password=SomeSecretPassword;";
        var options = new DbContextOptionsBuilder<TodoDbContext>()
            .UseNpgsql(cs)
            .Options;

        _ctx = new TodoDbContext(options);
        await _ctx.Database.EnsureCreatedAsync();
        _connection = _ctx.Database.GetDbConnection();
        
        _random = new Random(420);
        _generator = new TodoGenerator(_ctx, _random);
        await _generator.CleanupTodos();
        await _generator.GenerateTodosAndSaveInDatabase(150);
    }

    [GlobalCleanup]
    public async Task Cleanup()
    {
        await _generator.CleanupTodos();
    }
    
    [Benchmark]
    public async Task<Todo?> QuerySingleOrDefault()
    {
        return await _ctx.Todos.AsNoTracking().SingleOrDefaultAsync(x => x.Id == 69);
    }

    private static readonly Func<TodoDbContext, int, Task<Todo?>> SingleTodoAsync =
        EF.CompileAsyncQuery(
            (TodoDbContext ctx, int id) => ctx.Todos.AsNoTracking()
                .SingleOrDefault(x => x.Id == id));

    [Benchmark]
    public async Task<Todo?> QuerySingleOrDefaultCompiled()
    {
        return await SingleTodoAsync(_ctx, 69);
    }
    [Benchmark]
    public async Task<Todo?> QuerySingleOrDefaultRawSql()
    {
        var id = 69;
        return await _ctx.Database.SqlQueryRaw<Todo?>("""SELECT * FROM todos."Todos" WHERE "Id" = {0}""", [id])
            .AsNoTracking()
            .SingleOrDefaultAsync();
    }
    
    [Benchmark]
    public async Task<Todo?> QuerySingleOrDefaultDapper()
    {
        var sql = $"""SELECT * FROM todos."Todos" WHERE "Id" = @Id""";
        return await _connection.QuerySingleOrDefaultAsync<Todo>(sql, new { Id = 69});
    }

    [Benchmark]
    public async Task<List<Todo>> QueryAll()
    {
        return await _ctx.Todos.AsNoTracking().ToListAsync();
    }

    [Benchmark]
    public async Task<List<Todo>> QueryAllRawSql()
    {
        return await _ctx.Database.SqlQueryRaw<Todo>("""SELECT * FROM todos."Todos" """).AsNoTracking().ToListAsync();
    }

    [Benchmark]
    public async Task<List<Todo>> QueryAllDapper()
    {
        var sql = $"""SELECT * FROM todos."Todos" """;
        return (await _connection.QueryAsync<Todo>(sql)).ToList();
    }

    [Benchmark]
    public async Task<Todo> QueryFiltered()
    {
        return await _ctx.Todos
            .AsNoTracking()
            .Where(x => x.IsCompleted)
            .SingleOrDefaultAsync(x => x.Id == 69);
    }

    private static readonly Func<TodoDbContext, int, Task<Todo?>> QueryFilter =
        EF.CompileAsyncQuery(
            (TodoDbContext ctx, int id) => ctx.Todos.AsNoTracking()
                .Where(x => x.IsCompleted).SingleOrDefault(x => x.Id == id));
    [Benchmark]
    public async Task<Todo> QueryFilteredCompiled()
    {
        return await QueryFilter(_ctx, 69);    
    }

    [Benchmark]
    public async Task<Todo> QueryFilteredRawSql()
    {
        return await _ctx.Database.SqlQueryRaw<Todo?>($"""SELECT * FROM todos."Todos" WHERE "IsCompleted" = true AND "Id" = {69}""")
            .AsNoTracking()
            .SingleOrDefaultAsync();
    }

    [Benchmark]
    public async Task<Todo> QueryFilteredDapper()
    {
        var sql = $"""SELECT * FROM todos."Todos" WHERE "IsCompleted" = true AND "Id" = @Id""";
        return await _connection.QueryFirstOrDefaultAsync<Todo>(sql, new { Id = 69});
    }
}
