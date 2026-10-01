using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace StudyFlow.Api.OpenApi;

internal sealed class AuthOperationTransformer
    : IOpenApiOperationTransformer
{
    public Task TransformAsync(
        OpenApiOperation operation,
        OpenApiOperationTransformerContext context,
        CancellationToken cancellationToken)
    {
        var metadata =
            context.Description.ActionDescriptor.EndpointMetadata;

        var hasAllowAnonymous =
            metadata
                .OfType<AllowAnonymousAttribute>()
                .Any();

        var hasAuthorize =
            metadata
                .OfType<AuthorizeAttribute>()
                .Any();

        if (!hasAuthorize || hasAllowAnonymous)
        {
            return Task.CompletedTask;
        }

        operation.Security ??= [];

        operation.Security.Add(
            new OpenApiSecurityRequirement
            {
                [
                    new OpenApiSecuritySchemeReference(
                        "IdentityBearer",
                        context.Document)
                ] = []
            });

        return Task.CompletedTask;
    }
}
