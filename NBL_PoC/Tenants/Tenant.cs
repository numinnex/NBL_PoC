
namespace NBL_PoC_Api.Tenants;

public class Tenant 
{
    public int Id { get; init; }
    public required string Name { get; init; }
    public required string ConnectionString { get; init; }
}

public static class TenantMappingExtensions
{
    public static TenantDto AsTenantDto(this Tenant tenant)
    {
        return new TenantDto(tenant.Id, tenant.Name);
    }
}