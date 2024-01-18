
using Microsoft.EntityFrameworkCore;

public class TenantService : ITenantService
{
    private readonly TenantsDbContext _ctx;

    public TenantService(TenantsDbContext ctx)
    {
        _ctx = ctx;
    }
    public async Task<string> GetConnectionStringAsync(int tenantId)
    {
        var tenantConnectionString = await _ctx.Tenants
            .Where(x => x.Id == tenantId)
            .Select(x => x.ConnectionString)
            .SingleOrDefaultAsync();

        return tenantConnectionString ?? string.Empty;
    }
}