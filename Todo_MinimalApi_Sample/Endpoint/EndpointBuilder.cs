namespace Todo_MinimalApi_Sample.Endpoint;

public static class EndpointBuilder
{
	public static RouteHandlerBuilder MapEndpoint<T>(this IEndpointRouteBuilder routes)
		where T : IEndpoint
	{
		return routes.MapMethods(
			T.Pattern,
			new[] { T.Method.ToString() },
			T.Handler
		).HasApiVersion(T.EndpointVersion);
	} 
}