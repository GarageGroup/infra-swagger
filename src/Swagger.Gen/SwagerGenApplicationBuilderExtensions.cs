using System;
using System.Text.Json;
using System.Threading.Tasks;
using GGroupp.Infra;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Microsoft.AspNetCore.Builder;

public static class SwagerGenApplicationBuilderExtensions
{
    public static TApplicationBuilder UseStandardSwagger<TApplicationBuilder>(
        this TApplicationBuilder app, string swaggerSectionName = "Swagger")
        where TApplicationBuilder : class, IApplicationBuilder
    {
        _ = app ?? throw new ArgumentNullException(nameof(app));

        return app.InnerUseStandardSwagger(ResolveOption);

        SwaggerOption ResolveOption(IServiceProvider serviceProvider)
            =>
            serviceProvider.GetRequiredService<IConfiguration>().GetSwaggerOption(swaggerSectionName);
    }

    public static TApplicationBuilder UseStandardSwagger<TApplicationBuilder>(
        this TApplicationBuilder app, Func<IServiceProvider, SwaggerOption> optionResolver)
        where TApplicationBuilder : class, IApplicationBuilder
    {
        _ = app ?? throw new ArgumentNullException(nameof(app));
        _ = optionResolver ?? throw new ArgumentNullException(nameof(optionResolver));

        return app.InnerUseStandardSwagger(optionResolver);
    }

    private static TApplicationBuilder InnerUseStandardSwagger<TApplicationBuilder>(
        this TApplicationBuilder app, Func<IServiceProvider, SwaggerOption> optionResolver)
        where TApplicationBuilder : class, IApplicationBuilder
    {
        _ = app.Use(InvokeSwaggerAsync);
        return app;

        Task InvokeSwaggerAsync(HttpContext context, RequestDelegate next)
        {
            var options = context.RequestServices.Pipe(optionResolver).Pipe(MapOption);
            var swaggerProvider = context.RequestServices.GetSwaggerGenerator(options);

            var middleware = new SwaggerMiddleware(next, new());
            return middleware.Invoke(context, swaggerProvider);
        }
    }

    private static SwaggerGenerator GetSwaggerGenerator(this IServiceProvider serviceProvider, SwaggerGenOptions options)
        =>
        new(
            options: options.SwaggerGeneratorOptions,
            apiDescriptionsProvider: GetApiDescriptionGroupCollectionProvider(serviceProvider),
            schemaGenerator: new SchemaGenerator(
                generatorOptions: options.SchemaGeneratorOptions,
                serializerDataContractResolver: new JsonSerializerDataContractResolver(new(JsonSerializerDefaults.Web))));

    private static IApiDescriptionGroupCollectionProvider GetApiDescriptionGroupCollectionProvider(IServiceProvider serviceProvider)
        =>
        serviceProvider.GetService<IApiDescriptionGroupCollectionProvider>() ?? new EmptyApiDescriptionGroupCollectionProvider();

    private static SwaggerGenOptions MapOption(SwaggerOption option)
    {
        var options = new SwaggerGenOptions();

        options.SwaggerDoc(option.ApiVersion, new()
        {
            Title = option.ApiName,
            Description = option.Description,
            Version = option.ApiVersion
        });

        return options;
    }
}