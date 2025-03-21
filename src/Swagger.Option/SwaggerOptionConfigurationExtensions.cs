using System;
using GarageGroup.Infra;

namespace Microsoft.Extensions.Configuration;

public static class SwaggerOptionConfigurationExtensions
{
    private const string InfoSectionName = "Info";

    public static SwaggerOption GetSwaggerOption(this IConfiguration configuration, string sectionName = "Swagger")
    {
        ArgumentNullException.ThrowIfNull(configuration);

        var section = configuration.GetSection(sectionName);

        if (section.Exists() is false)
        {
            section = configuration.GetSection(InfoSectionName);
        }

        return new(
            apiName: section["ApiName"] ?? string.Empty,
            apiVersion: section["ApiVersion"] ?? string.Empty,
            description: section["Description"])
        {
            TermsOfService = section.GetUri("TermsOfService"),
            Contact = section.GetSwaggerContact("Contact"),
            License = section.GetSwaggerLicense("License")
        };
    }

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