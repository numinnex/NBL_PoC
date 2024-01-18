using Asp.Versioning;
using Asp.Versioning.Builder;

namespace NBL_PoC_Api.Version;

public static class ApiVersioning
{
    private const string ApiName = "NBL_Api_Version";
    public static ApiVersion V1 => new(1);
    public static ApiVersionSet Set => new ApiVersionSetBuilder(ApiName)
        .HasApiVersion(V1)
        .ReportApiVersions()
        .Build();
}