namespace NBL_PoC_Api.Tenant
{
    public record TenantData : ITenant
    {
        public TenantData(int tenantId)
            => TenantId = tenantId;
        public int TenantId { get; set; }
    }
}