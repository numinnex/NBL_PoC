using System.Data;
using BenchmarkDotNet.Attributes;
using Dapper;
using Microsoft.EntityFrameworkCore;
using NBL_PoC_Api.Persistance;
using NBL_PoC_Api.Seeder;
using NBL_PoC_Api.Todos;

namespace NBL_PoC_Benchamrsk;

[MemoryDiagnoser]
public class DatabaseBenchmarks
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
        await _generator.GenerateTodos(150);
    }

    [GlobalCleanup]
    public async Task Cleanup()
    {
        await _generator.CleanupTodos();
    }
    
    [Benchmark]
    public async Task<Todo?> QuerySingleOrDefaultWithAsNotracking()
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
        return await _ctx.Database.SqlQueryRaw<Todo?>("""SELECT * FROM "Todos" WHERE "Id" = {0}""", [id])
            .AsNoTracking()
            .SingleOrDefaultAsync();
    }
    
    [Benchmark]
    public async Task<Todo?> QuerySingleOrDefaultDapper()
    {
        var sql = $"""SELECT * FROM "Todos" WHERE "Id" = @Id""";
        return await _connection.QuerySingleOrDefaultAsync<Todo>(sql, new { Id = 69});
    }

    [Benchmark]
    public async Task<List<Todo>> QueryAllWithAsNoTracking()
    {
        return await _ctx.Todos.AsNoTracking().ToListAsync();
    }

    [Benchmark]
    public async Task<List<Todo>> QueryAllRawSql()
    {
        return await _ctx.Database.SqlQueryRaw<Todo>("""SELECT * FROM "Todos" """).AsNoTracking().ToListAsync();
    }

    [Benchmark]
    public async Task<List<Todo>> QueryAllDapper()
    {
        var sql = $"""SELECT * FROM "Todos" """;
        return (await _connection.QueryAsync<Todo>(sql)).ToList();
    }

    [Benchmark]
    public async Task<Todo> QueryFilteredWithNoTracking()
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
    public async Task<Todo> QueryFilterCompiled()
    {
        return await QueryFilter(_ctx, 69);    
    }

    [Benchmark]
    public async Task<Todo> QueryFilterRawSql()
    {
        return await _ctx.Database.SqlQueryRaw<Todo?>($"""SELECT * FROM "Todos" WHERE "IsCompleted" = true AND "Id" = {69}""")
            .AsNoTracking()
            .SingleOrDefaultAsync();
    }

    [Benchmark]
    public async Task<Todo> QueryFilterDapper()
    {
        var sql = $"""SELECT * FROM "Todos" WHERE "IsCompleted" = true AND "Id" = @Id""";
        return await _connection.QueryFirstOrDefaultAsync<Todo>(sql, new { Id = 69});
    }
}
