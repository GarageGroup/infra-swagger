using System;
using System.Text;
using GGroupp.Infra;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Microsoft.AspNetCore.Builder;

public static class SwaggerUIApplicationBuilderExtensions
{
    private const string DescriptionStandardFontSize = "font-size: 0.8em !important;";

    public static TApplicationBuilder UseStandardSwaggerUI<TApplicationBuilder>(
        this TApplicationBuilder app, string swaggerSectionName = "Swagger")
        where TApplicationBuilder : class, IApplicationBuilder
    {
        ArgumentNullException.ThrowIfNull(app);
        return app.InnerUseSwaggerUI(ResolveOption, AddStandardStyles);

        SwaggerOption ResolveOption(IServiceProvider serviceProvider)
            =>
            serviceProvider.GetRequiredService<IConfiguration>().GetSwaggerOption(swaggerSectionName);
    }

    public static TApplicationBuilder UseStandardSwaggerUI<TApplicationBuilder>(
        this TApplicationBuilder app, Func<IServiceProvider, SwaggerOption> optionResolver)
        where TApplicationBuilder : class, IApplicationBuilder
    {
        ArgumentNullException.ThrowIfNull(app);
        ArgumentNullException.ThrowIfNull(optionResolver);

        return app.InnerUseSwaggerUI(optionResolver, AddStandardStyles);
    }

    public static TApplicationBuilder UseSwaggerUI<TApplicationBuilder>(
        this TApplicationBuilder app, Func<IServiceProvider, SwaggerOption> optionResolver, Action<SwaggerUIOptions> setupSwaggerUI)
        where TApplicationBuilder : class, IApplicationBuilder
    {
        ArgumentNullException.ThrowIfNull(app);
        ArgumentNullException.ThrowIfNull(optionResolver);
        ArgumentNullException.ThrowIfNull(setupSwaggerUI);

        return app.InnerUseSwaggerUI(optionResolver, setupSwaggerUI);
    }

    private static TApplicationBuilder InnerUseSwaggerUI<TApplicationBuilder>(
        this TApplicationBuilder app, Func<IServiceProvider, SwaggerOption> optionResolver, Action<SwaggerUIOptions>? setupSwaggerUI = null)
        where TApplicationBuilder : IApplicationBuilder
    {
        _ = app.UseSwaggerUI(SetupSwaggerUI);
        return app;

        void SetupSwaggerUI(SwaggerUIOptions options)
        {
            var option = optionResolver.Invoke(app.ApplicationServices);
            options.SwaggerEndpoint("/swagger/swagger.json", option.ApiName);

            setupSwaggerUI?.Invoke(options);
        }
    }

    private static void AddStandardStyles(SwaggerUIOptions options)
        =>
        options.HeadContent = new StringBuilder(options.HeadContent)
            .AppendLine("<style>")
            .AppendLine("\t").Append(".renderedMarkdown p {")
            .AppendLine("\t\t").Append(DescriptionStandardFontSize)
            .AppendLine("\t").Append('}')
            .AppendLine("\t").Append(".property.primitive {")
            .AppendLine("\t\t").Append(DescriptionStandardFontSize)
            .AppendLine("\t").Append('}')
            .AppendLine("</style>")
            .ToString();
}