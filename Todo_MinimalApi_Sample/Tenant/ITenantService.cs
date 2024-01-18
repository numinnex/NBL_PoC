public interface ITenantService 
{
    public Task<string>  GetConnectionStringAsync(int tenantId);
}