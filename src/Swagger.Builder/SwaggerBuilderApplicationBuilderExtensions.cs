using System;
using GGroupp.Infra;
using GGroupp.Infra.Swagger.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;

namespace Microsoft.AspNetCore.Builder;

public static class SwaggerBuilderApplicationBuilderExtensions
{
    public static TApplicationBuilder UseSwagger<TApplicationBuilder>(
        this TApplicationBuilder app!!, Func<IServiceProvider, SwaggerOption> swaggerOptionResolver!!)
        where TApplicationBuilder : class, IApplicationBuilder, ISwaggerBuilder
    {
        return app.UseSwagger(ResolveProvider);

        ISwaggerProvider ResolveProvider(IServiceProvider serviceProvider)
            =>
            new SwaggerProvider(
                swaggerOption: swaggerOptionResolver.Invoke(serviceProvider),
                swaggerBuilder: app);
    }

    public static TApplicationBuilder UseSwagger<TApplicationBuilder>(
        this TApplicationBuilder app!!, string swaggerSectionName = "Swagger")
        where TApplicationBuilder : class, IApplicationBuilder, ISwaggerBuilder
    {
        return app.UseSwagger(ResolveProvider);

        ISwaggerProvider ResolveProvider(IServiceProvider serviceProvider)
            =>
            new SwaggerProvider(
                swaggerOption: serviceProvider.GetRequiredService<IConfiguration>().GetSwaggerOption(swaggerSectionName),
                swaggerBuilder: app);
    }
}