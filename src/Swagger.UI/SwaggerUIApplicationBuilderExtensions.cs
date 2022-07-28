using System;
using GGroupp.Infra;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Microsoft.AspNetCore.Builder;

public static class SwaggerUIApplicationBuilderExtensions
{
    public static TApplicationBuilder UseStandardSwaggerUI<TApplicationBuilder>(
        this TApplicationBuilder app, string swaggerSectionName = "Swagger")
        where TApplicationBuilder : class, IApplicationBuilder
    {
        _ = app ?? throw new ArgumentNullException(nameof(app));

        return app.InnerUseSwaggerUI(ResolveOption);

        SwaggerOption ResolveOption(IServiceProvider serviceProvider)
            =>
            serviceProvider.GetRequiredService<IConfiguration>().GetSwaggerOption(swaggerSectionName);
    }

    public static TApplicationBuilder UseStandardSwaggerUI<TApplicationBuilder>(
        this TApplicationBuilder app, Func<IServiceProvider, SwaggerOption> optionResolver)
        where TApplicationBuilder : class, IApplicationBuilder
    {
        _ = app ?? throw new ArgumentNullException(nameof(app));
        _ = optionResolver ?? throw new ArgumentNullException(nameof(optionResolver));

        return app.InnerUseSwaggerUI(optionResolver);
    }

    private static TApplicationBuilder InnerUseSwaggerUI<TApplicationBuilder>(
        this TApplicationBuilder app, Func<IServiceProvider, SwaggerOption> optionResolver)
        where TApplicationBuilder : IApplicationBuilder
    {
        _ = app.UseSwaggerUI(SetupSwaggerUI);
        return app;

        void SetupSwaggerUI(SwaggerUIOptions options)
        {
            var option = optionResolver.Invoke(app.ApplicationServices);
            options.SwaggerEndpoint($"/swagger/{option.ApiVersion}/swagger.json", option.ApiName);
        }
    }
}