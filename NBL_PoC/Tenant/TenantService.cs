
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
    public async Task CreateAsync(TenantDto dto, CancellationToken token) 
    {
        //TODO(Grzegorz Koszyk) - Encrypt the connection string before storing it in database
        //with AES-256 
        var connectionString = TenantUtils.GenerateConnectionString(dto);
        var tenant = new Tenant 
        {
            Id = dto.Id,
            Name = dto.Name,
            ConnectionString = connectionString
        };

        await _ctx.Tenants.AddAsync(tenant, token);
        await _ctx.SaveChangesAsync(token);
    }
    public async Task<bool> DeleteAsync(int id, CancellationToken token) 
    {
        var rowsAffected = await _ctx.Tenants.Where(x => x.Id == id)
                .ExecuteDeleteAsync(token);
        return rowsAffected > 0; 
    }

    public async Task<List<TenantDto>> GetAll(CancellationToken token)
    {
        return await _ctx.Tenants.Select(x => x.AsTenantDto()).ToListAsync(token);
    }
    
    public async Task<TenantDto?> GetById(int id, CancellationToken token)
    {
        return await _ctx.Tenants.FindAsync([id], token) switch 
        {
            Tenant tenant => tenant.AsTenantDto(),
            _ => null,
        };
    }
}