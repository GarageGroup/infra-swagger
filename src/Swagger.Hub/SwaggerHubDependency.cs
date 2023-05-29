using System;
using GarageGroup.Infra;
using PrimeFuncPack;

namespace Microsoft.AspNetCore.Builder;

public static class SwaggerHubDependency
{
    public static TApplicationBuilder MapSwaggerHub<TApplicationBuilder>(
        this Dependency<IHubSwaggerDocumentProvider> dependency, TApplicationBuilder applicationBuilder)
        where TApplicationBuilder : class, IApplicationBuilder
    {
        ArgumentNullException.ThrowIfNull(dependency);
        ArgumentNullException.ThrowIfNull(applicationBuilder);

        return applicationBuilder.UseSwagger(dependency.Resolve);
    }
}