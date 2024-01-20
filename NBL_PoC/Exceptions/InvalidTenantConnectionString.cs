namespace NBL_PoC_Api.Exceptions;

public class InvalidTenantConnectionString : Exception
{
    private new const string Message = "Couldn't find tenant's connection string";
    public InvalidTenantConnectionString() : base(Message)
    {
        
    } 
}