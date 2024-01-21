namespace NBL_PoC_Api.Tenants
{
    public sealed record TenantData : ITenant
    {
        public TenantData(int tenantId)
            => TenantId = tenantId;
        public int TenantId { get; set; }
    }
}