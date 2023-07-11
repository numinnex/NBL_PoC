using Asp.Versioning;

namespace Todo_MinimalApi_Sample.Endpoint;

public interface IEndpoint
{
	public static abstract ApiVersion EndpointVersion { get; }
	public static abstract string Pattern { get; } 
	public static abstract HttpMethod Method { get; }
	public static abstract Delegate Handler { get; }
}