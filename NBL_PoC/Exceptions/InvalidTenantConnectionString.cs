namespace NBL_PoC_Api.Exceptions;

public sealed class InvalidTenantConnectionString : Exception
{
    private new const string Message = "Couldn't find tenant's connection string";
    public InvalidTenantConnectionString() : base(Message)
    {
        
    } 
}