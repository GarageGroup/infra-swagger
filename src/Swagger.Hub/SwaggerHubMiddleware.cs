using System;
using GarageGroup.Infra;

namespace Microsoft.AspNetCore.Builder;

public static class SwaggerHubMiddleware
{
    public static TApplicationBuilder UseSwaggerHub<TApplicationBuilder>(
        this TApplicationBuilder applicationBuilder,
        Func<IServiceProvider, IHubSwaggerDocumentProvider> documentProviderResolver)
        where TApplicationBuilder : class, IApplicationBuilder
    {
        ArgumentNullException.ThrowIfNull(applicationBuilder);
        ArgumentNullException.ThrowIfNull(documentProviderResolver);

        return applicationBuilder.UseSwagger(documentProviderResolver);
    }
}