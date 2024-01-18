namespace Todo_MinimalApi_Sample.Tenant
{
    public record TenantData : ITenant
    {
        public TenantData(int tenantId)
            => TenantId = tenantId;
        public int TenantId { get; set; }
    }
}