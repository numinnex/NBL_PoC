using Microsoft.EntityFrameworkCore;
using Todo_MinimalApi_Sample.Persistance;

public class TodoDbContextScopedFactory : IDbContextFactory<TodoDbContext>
{
    private const int DefaultTenantId = -1;

    private readonly IDbContextFactory<TodoDbContext> _pooledFactory;
    private readonly int _tenantId;

    public TodoDbContextScopedFactory(
        IDbContextFactory<TodoDbContext> pooledFactory,
        ITenant tenant)
    {
        _pooledFactory = pooledFactory;
        _tenantId = tenant?.TenantId ?? DefaultTenantId;
    }

    public TodoDbContext CreateDbContext()
    {
        var context = _pooledFactory.CreateDbContext();
        context.TenantId = _tenantId;
        return context;
    }
}