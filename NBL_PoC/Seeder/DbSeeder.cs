using System.Security.Cryptography.Xml;
using Microsoft.EntityFrameworkCore;

public static class DbSeeder 
{
    public async static Task SeedTenantsAsync(int tenantsCount, TenantsDbContext ctx, IEncryptor encryptor) 
    {
        if(!await ctx.Tenants.AnyAsync())
        {
            await ctx.Tenants.AddRangeAsync(Enumerable.Range(1, tenantsCount).Select(x => {
                return new Tenant 
                {
                    Id = x,
                    Name = $"Tenant-{x}",
                    ConnectionString = encryptor.Encrypt(TenantUtils.GenerateConnectionString(new TenantDto(x, $"Tenant-{x}")))
                };
            }));
            await ctx.SaveChangesAsync();
        }
    }
}