using System;
using System.Text.Json;
using GGroupp.Infra;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Microsoft.AspNetCore.Builder;

public static class SwagerGenApplicationBuilderExtensions
{
    private const string AuthorizationScheme = "bearer";

    public static IApplicationBuilder UseStandardSwagger(this IApplicationBuilder app, string swaggerSectionName = "Swagger")
    {
        _ = app ?? throw new ArgumentNullException(nameof(app));

        return app.InnerUseStandardSwagger(ResolveOption);

        SwaggerOption ResolveOption(IServiceProvider serviceProvider)
            =>
            serviceProvider.GetRequiredService<IConfiguration>().GetSwaggerOption(swaggerSectionName);
    }

    public static IApplicationBuilder UseStandardSwagger(this IApplicationBuilder app, Func<IServiceProvider, SwaggerOption> optionResolver)
    {
        _ = app ?? throw new ArgumentNullException(nameof(app));
        _ = optionResolver ?? throw new ArgumentNullException(nameof(optionResolver));

        return app.InnerUseStandardSwagger(optionResolver);
    }

    private static IApplicationBuilder InnerUseStandardSwagger(this IApplicationBuilder app, Func<IServiceProvider, SwaggerOption> optionResolver)
    {
        return app.UseSwagger(ResolveSwaggerProvider);

        ISwaggerProvider ResolveSwaggerProvider(IServiceProvider serviceProvider)
            =>
            serviceProvider.Pipe(optionResolver).Pipe(MapOption).Pipe(serviceProvider.GetSwaggerGenerator);
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

        if (option.UseAuthorization is false)
        {
            return options;
        }

        options.AddSecurityDefinition(AuthorizationScheme, new()
        {
            Name = "Authorization",
            Description = "JWT authorization header",
            In = ParameterLocation.Header,
            Scheme = AuthorizationScheme
        });

        options.AddSecurityRequirement(new()
        {
            {
                new()
                {
                    Reference = new()
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = AuthorizationScheme
                    }
                },
                Array.Empty<string>()
            }
        });

        return options;
    }
}