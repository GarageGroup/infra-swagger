using System;
using GarageGroup.Infra;

namespace Microsoft.Extensions.Configuration;

public static class SwaggerOptionConfigurationExtensions
{
    public static SwaggerOption GetSwaggerOption(this IConfiguration configuration, string sectionName = "Swagger")
    {
        ArgumentNullException.ThrowIfNull(configuration);
        return configuration.GetSection(sectionName).InnerGetSwaggerOption(string.Empty);
    }

    public static SwaggerOption GetSwaggerOptionWithPrefix(this IConfiguration configuration, string prefix = "Swagger")
    {
        ArgumentNullException.ThrowIfNull(configuration);
        return configuration.InnerGetSwaggerOption(prefix);
    }

    private static SwaggerOption InnerGetSwaggerOption(this IConfiguration section, string prefix)
        =>
        new(
            apiName: section[prefix + "ApiName"] ?? string.Empty,
            apiVersion: section[prefix + "ApiVersion"] ?? string.Empty,
            description: section[prefix + "Description"])
        {
            TermsOfService = section.GetUri(prefix + "TermsOfService"),
            Contact = section.GetSwaggerContact(prefix + "Contact"),
            License = section.GetSwaggerLicense(prefix + "License")
        };

    private static SwaggerContactOption? GetSwaggerContact(this IConfiguration configuration, string sectionName)
    {
        var section = configuration.GetSection(sectionName);
        if (section.Exists() is false)
        {
            return null;
        }

        return new()
        {
            Name = section["Name"],
            Email = section["Email"],
            Url = section.GetUri("Url")
        };
    }

    private static SwaggerLicenseOption? GetSwaggerLicense(this IConfiguration configuration, string sectionName)
    {
        var section = configuration.GetSection(sectionName);
        if (section.Exists() is false)
        {
            return null;
        }

        return new()
        {
            Name = section["Name"],
            Url = section.GetUri("Url")
        };
    }

    private static Uri? GetUri(this IConfiguration configuration, string key)
    {
        var textValue = configuration[key];
        return string.IsNullOrWhiteSpace(textValue) ? null : new(textValue);
    }
}