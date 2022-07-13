using System;
using GGroupp.Infra;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Microsoft.AspNetCore.Builder;

public static class SwaggerUIApplicationBuilderExtensions
{
    public static IApplicationBuilder UseStandardSwaggerUI(this IApplicationBuilder app, string swaggerSectionName = "Swagger")
    {
        _ = app ?? throw new ArgumentNullException(nameof(app));

        return app.InnerUseSwaggerUI(ResolveOption);

        SwaggerOption ResolveOption(IServiceProvider serviceProvider)
            =>
            serviceProvider.GetRequiredService<IConfiguration>().GetSwaggerOption(swaggerSectionName);
    }

    public static IApplicationBuilder UseStandardSwaggerUI(this IApplicationBuilder app, Func<IServiceProvider, SwaggerOption> optionResolver)
    {
        _ = app ?? throw new ArgumentNullException(nameof(app));
        _ = optionResolver ?? throw new ArgumentNullException(nameof(optionResolver));

        return app.InnerUseSwaggerUI(optionResolver);
    }

    private static IApplicationBuilder InnerUseSwaggerUI(this IApplicationBuilder app, Func<IServiceProvider, SwaggerOption> optionResolver)
    {
        return app.UseSwaggerUI(SetupSwaggerUI);

        void SetupSwaggerUI(SwaggerUIOptions options)
        {
            var option = optionResolver.Invoke(app.ApplicationServices);
            options.SwaggerEndpoint($"/swagger/{option.ApiVersion}/swagger.json", option.ApiName);
        }
    }
}