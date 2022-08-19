using System;
using System.Net.Http;
using GGroupp.Infra;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PrimeFuncPack;

namespace Microsoft.AspNetCore.Builder;

public static class SwaggerHubDependency
{
    public static TApplicationBuilder MapSwaggerHub<TApplicationBuilder>(
        this Dependency<HttpMessageHandler, SwaggerHubOption> dependency, TApplicationBuilder applicationBuilder)
        where TApplicationBuilder : class, IApplicationBuilder
    {
        _ = dependency ?? throw new ArgumentNullException(nameof(dependency));
        _ = applicationBuilder ?? throw new ArgumentNullException(nameof(applicationBuilder));

        var documentProviderDependency = dependency.With(ResolveLoggerFactory).Fold<ISwaggerDocumentProvider>(HubSwaggerDocumentProvider.Create);
        return applicationBuilder.UseSwagger(documentProviderDependency.Resolve);

        static ILoggerFactory? ResolveLoggerFactory(IServiceProvider serviceProvider)
            =>
            serviceProvider.GetService<ILoggerFactory>();
    }
}