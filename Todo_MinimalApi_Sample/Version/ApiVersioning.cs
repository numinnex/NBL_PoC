using Asp.Versioning;
using Asp.Versioning.Builder;

namespace Todo_MinimalApi_Sample.Version;

public static class ApiVersioning
{
	private const string ApiName = "TodoApi_Version";
	public static ApiVersion V1 => new (1);
	public static ApiVersionSet Set => new ApiVersionSetBuilder(ApiName)
		.HasApiVersion(V1)
		.ReportApiVersions()
		.Build();
}