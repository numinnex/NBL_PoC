public class Tenant 
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string ConnectionString { get; set; }

}

public static class TenantMappingExtensions
{
    public static TenantDto AsTenantDto(this Tenant tenant)
    {
        return new TenantDto(tenant.Id, tenant.Name);
    }
}