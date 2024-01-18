using Microsoft.EntityFrameworkCore;
using Todo_MinimalApi_Sample.Persistance;

public class TodoDbContextScopedFactory : IDbContextFactory<TodoDbContext>
{
    private const int DefaultTenantId = -1;

    private readonly int _tenantId;
    private readonly IDbContextFactory<TodoDbContext> _pooledFactory;
    private readonly ITenantService _tenantService;

    public TodoDbContextScopedFactory(
        IDbContextFactory<TodoDbContext> pooledFactory,
        ITenantService tenantService,
        ITenant tenant)
    {
        _tenantId = tenant?.TenantId ?? DefaultTenantId;
        _tenantService = tenantService;
        _pooledFactory = pooledFactory;
    }

    public TodoDbContext CreateDbContext()
    {
        throw new NotImplementedException();
    }
    public async Task<TodoDbContext> CreateDbContextAsync() 
    {
        var connectionString = await _tenantService.GetConnectionStringAsync(_tenantId);
        var context = _pooledFactory.CreateDbContext();
        context.ConnectionString = connectionString;

        return context;
    }
}