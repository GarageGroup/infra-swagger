using System;
using GGroupp.Infra;
using GGroupp.Infra.Swagger.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Builder;

public static class SwaggerBuilderApplicationBuilderExtensions
{
    public static TApplicationBuilder UseSwagger<TApplicationBuilder>(
        this TApplicationBuilder app, Func<IServiceProvider, SwaggerOption> swaggerOptionResolver)
        where TApplicationBuilder : class, IApplicationBuilder, ISwaggerBuilder
    {
        ArgumentNullException.ThrowIfNull(app);
        ArgumentNullException.ThrowIfNull(swaggerOptionResolver);

        return app.UseSwagger(ResolveProvider);

        ISwaggerDocumentProvider ResolveProvider(IServiceProvider serviceProvider)
            =>
            new SwaggerDocumentProvider(
                swaggerOption: swaggerOptionResolver.Invoke(serviceProvider),
                swaggerBuilder: app);
    }

    public static TApplicationBuilder UseSwagger<TApplicationBuilder>(
        this TApplicationBuilder app, string swaggerSectionName = "Swagger")
        where TApplicationBuilder : class, IApplicationBuilder, ISwaggerBuilder
    {
        ArgumentNullException.ThrowIfNull(app);
        return app.UseSwagger(ResolveProvider);

        ISwaggerDocumentProvider ResolveProvider(IServiceProvider serviceProvider)
            =>
            new SwaggerDocumentProvider(
                swaggerOption: serviceProvider.GetRequiredService<IConfiguration>().GetSwaggerOption(swaggerSectionName),
                swaggerBuilder: app);
    }
}