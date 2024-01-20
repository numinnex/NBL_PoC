using System.ComponentModel.DataAnnotations;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace NBL_PoC_Api.Utils;
public class TenantHeaderFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var hasCustomHeaderAttribute = context.ApiDescription.ActionDescriptor.EndpointMetadata
            .Any(em => em.GetType() == typeof(RequireTenantIdHeader));
        if (hasCustomHeaderAttribute)
        {
            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "X-TenantId",
                In = ParameterLocation.Header,
                Required = true
            });
        }
    }
}