public interface ITenantService 
{
    public Task<string>  GetConnectionStringAsync(int tenantId);
    public Task CreateAsync(TenantDto dto, CancellationToken token);
    public Task<bool> DeleteAsync(int id, CancellationToken token);
    public Task<List<TenantDto>> GetAll(CancellationToken token);
    public Task<TenantDto?> GetById(int id, CancellationToken token);
}