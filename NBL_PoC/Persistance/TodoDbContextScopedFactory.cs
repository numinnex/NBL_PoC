using Microsoft.EntityFrameworkCore;
using NBL_PoC_Api.Tenants;

namespace NBL_PoC_Api.Persistance;

// This is needed to replace the connection string of each pooled context, before returning it back to the caller
// Here's more about it and the potential pitfalls, that using a pooled context with multi tenant model can cause
// https://github.com/dotnet/efcore/issues/14625
public class TodoDbContextScopedFactory : IDbContextFactory<TodoDbContext>
{

    private const int DefaultTenantId = -1;
    
    private readonly IDbContextFactory<TodoDbContext> _pooledFactory;
    private readonly int _tenantId;
    private readonly ITenantService _tenantService;

    public TodoDbContextScopedFactory(
        IDbContextFactory<TodoDbContext> pooledFactory,
        ITenant tenant,
        ITenantService tenantService)
    {
        _pooledFactory = pooledFactory;
        _tenantId = tenant?.TenantId ?? DefaultTenantId;
        _tenantService = tenantService;
    }
    public TodoDbContext CreateDbContext()
    {
        // this retrieves the connection string from database, in the future caching those results is required
        // to avoid excessive database calls
        var ctx = _pooledFactory.CreateDbContext();
        if (_tenantId != DefaultTenantId)
        {
            var tenantCs = Task.Run(async () =>
                await _tenantService.GetConnectionStringAsync(_tenantId)).Result;
            ctx.Database.SetConnectionString(tenantCs);
            return ctx;
        }

        return ctx;
    }
}