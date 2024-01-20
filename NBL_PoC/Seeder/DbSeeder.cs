using Microsoft.EntityFrameworkCore;
using NBL_PoC_Api.Crypto;
using NBL_PoC_Api.Persistance;
using NBL_PoC_Api.Tenants;
using NBL_PoC_Api.Utils;

namespace NBL_PoC_Api.Seeder;

public static class DbSeeder 
{
    public static async Task SeedTenantsAsync(int tenantsCount, TenantsDbContext ctx, IEncryptor encryptor) 
    {
        if(!await ctx.Tenants.AnyAsync())
        {
            await ctx.Tenants.AddRangeAsync(Enumerable.Range(1, tenantsCount).Select(x => new Tenant
            {
                Id = x,
                Name = $"Tenant-{x}",
                ConnectionString = encryptor.Encrypt(TenantUtils.GenerateConnectionString(new TenantDto(x, $"Tenant-{x}")))
            }));
            await ctx.SaveChangesAsync();
        }
    }
}