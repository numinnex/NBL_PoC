using Microsoft.EntityFrameworkCore;
using NBL_PoC_Api.Crypto;
using NBL_PoC_Api.Exceptions;
using NBL_PoC_Api.Persistance;
using NBL_PoC_Api.Utils;

namespace NBL_PoC_Api.Tenants;

public sealed class TenantService : ITenantService
{
    private readonly TenantsDbContext _ctx;
    private readonly IEncryptor _encryptor;
    private readonly ITenant _tenant;

    public TenantService(TenantsDbContext ctx, IEncryptor encryptor, ITenant tenant)
    {
        _ctx = ctx;
        _encryptor = encryptor;
        _tenant = tenant;
    }
    public async Task<string> GetConnectionStringAsync(int tenantId)
    {
        var tenantConnectionString = await _ctx.Tenants
            .Where(x => x.Id == tenantId)
            .Select(x => x.ConnectionString)
            .SingleOrDefaultAsync();
        
        return tenantConnectionString switch 
        {
            not null => _encryptor.Decrypt(tenantConnectionString),
            _ => throw new InvalidTenantConnectionString()
        };
    }
    public async Task CreateAsync(TenantDto dto, CancellationToken token) 
    {
        var connectionString = _encryptor.Encrypt(TenantUtils.GenerateConnectionString(dto));
        var tenant = new Tenants.Tenant 
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