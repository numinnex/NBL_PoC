using Microsoft.EntityFrameworkCore;
using NBL_PoC_Api.Tenants;

namespace NBL_PoC_Api.Persistance;

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
        return _pooledFactory.CreateDbContext();
    }
    public async Task<TodoDbContext> CreateDbContextAsync() 
    {
        var connectionString = await _tenantService.GetConnectionStringAsync(_tenantId);
        var context = _pooledFactory.CreateDbContext();
        context.ConnectionString = connectionString;

        return context;
    }
}