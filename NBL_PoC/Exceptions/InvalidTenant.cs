public sealed class InvalidTenant : Exception
{
    private new const string Message = "Invalid TenantId";
    public InvalidTenant() : base(Message)
    {
        
    } 
}