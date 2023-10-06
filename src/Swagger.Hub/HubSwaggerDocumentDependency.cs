using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using PrimeFuncPack;

namespace GarageGroup.Infra;

public static class HubSwaggerDocumentDependency
{
    public static Dependency<IHubSwaggerDocumentProvider> UseHubSwaggerDocumentProvider(
        this Dependency<HttpMessageHandler, SwaggerHubOption> dependency)
    {
        ArgumentNullException.ThrowIfNull(dependency);
        return dependency.With(ResolveLoggerFactory).Fold<IHubSwaggerDocumentProvider>(HubSwaggerDocumentProvider.Create);
    }

    public static Dependency<IHubSwaggerDocumentProvider> UseHubSwaggerDocumentProvider(
        this Dependency<HttpMessageHandler> dependency, string sectionName = "Swagger")
    {
        ArgumentNullException.ThrowIfNull(dependency);
        return dependency.With(ResolveOption).With(ResolveLoggerFactory).Fold<IHubSwaggerDocumentProvider>(HubSwaggerDocumentProvider.Create);

        SwaggerHubOption ResolveOption(IServiceProvider serviceProvider)
            =>
            serviceProvider.GetRequiredService<IConfiguration>().GetSwaggerHubOption(sectionName);
    }

    private static SwaggerHubOption GetSwaggerHubOption(this IConfiguration configuration, string sectionName)
        =>
        new(
            option: configuration.GetSwaggerOption(sectionName),
            documents: configuration.GetSection(sectionName).GetSection("Documents").GetDocumentOptions().ToFlatArray());

    private static ILoggerFactory? ResolveLoggerFactory(IServiceProvider serviceProvider)
        =>
        serviceProvider.GetService<ILoggerFactory>();

    private static Uri GetUri(this IConfigurationSection section, string key)
    {
        var value = section[key];
        if (string.IsNullOrEmpty(value))
        {
            throw new InvalidOperationException($"Configuration path '{section.Path}:{key}' value must be specified");
        }

        if (Uri.TryCreate(value, UriKind.Absolute, out var uri) is false)
        {
            throw new InvalidOperationException($"Configuration path '{section.Path}:{key}' value '{value}' must be a valid absolute URI");
        }

        return uri;
    }

    private static bool GetBoolean(this IConfigurationSection section, string key)
    {
        var value = section[key];

        if (string.IsNullOrEmpty(value))
        {
            return false;
        }

        return string.Equals("true", value, StringComparison.InvariantCultureIgnoreCase);
    }

    private static IEnumerable<SwaggerDocumentOption> GetDocumentOptions(this IConfigurationSection section)
    {
        foreach (var documentSection in section.GetChildren())
        {
            if (documentSection.GetBoolean("IsDisabled"))
            {
                continue;
            }

            yield return new(
                baseAddress: documentSection.GetUri("BaseAddressUrl"),
                documentUrl: documentSection["DocumentUrl"])
            {
                UrlSuffix = documentSection["UrlSuffix"],
                IsDirectCall = documentSection.GetBoolean("IsDirectCall"),
                SecurityRequirements = documentSection.GetSection("SecurityRequirements").GetSecurityRequirements()
            };
        }
    }

    private static FlatArray<KeyValuePair<string, OpenApiSecurityScheme>> GetSecurityRequirements(this IConfigurationSection section)
    {
        var values = section.Get<Dictionary<string, OpenApiSecurityScheme>>()?.ToArray();

        if (values?.Length is not > 0)
        {
            return default;
        }

        var builder = FlatArray<KeyValuePair<string, OpenApiSecurityScheme>>.Builder.OfLength(values.Length);

        for (var i = 0; i < values.Length; i++)
        {
            var item = values[i];

            item.Value.Reference ??= new()
            {
                Type = ReferenceType.SecurityScheme,
                Id = item.Key
            };

            builder[i] = item;
        }

        return builder.MoveToFlatArray();
    }
}