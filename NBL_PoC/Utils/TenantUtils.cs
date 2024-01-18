public static class TenantUtils 
{
    public static string GenerateConnectionString(TenantDto dto) 
    {
        var prefix = "Host=db;Database=";
        // Thos two will be generated in future.
        var username = "super_admin";
        var password = "SomeSecretPassword;";

        var tenantHash = Hasher.Compute128BitHash(dto.Id);
        var connectionString = $"{prefix}tenant_{tenantHash};Username={username};Password={password}";
        return connectionString;
    }
}