public static class TenantUtils 
{
    public static string GenerateConnectionString(int tenantId) 
    {
        var prefix = "Host=db;Database=";
        var username = "super_admin";
        var password = "SomeSecretPassword;";
        var connectionString = $"{prefix}tenant-{tenantId};Username={username};Password={password}";
        return connectionString;
    }
}