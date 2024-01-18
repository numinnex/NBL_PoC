namespace NBL_PoC.Endpoint;

public static class EndpointBuilder
{
	public static RouteHandlerBuilder MapEndpoint<T>(this IEndpointRouteBuilder routes)
		where T : IEndpoint
	{
		return routes.MapMethods(
			T.Pattern,
			[T.Method.ToString()],
			T.Handler
		).HasApiVersion(T.EndpointVersion);
	} 
}