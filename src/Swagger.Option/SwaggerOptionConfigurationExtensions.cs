using System;
using GGroupp.Infra;

namespace Microsoft.Extensions.Configuration;

public static class SwaggerOptionConfigurationExtensions
{
    public static SwaggerOption GetSwaggerOption(this IConfiguration configuration, string sectionName = "Swagger")
    {
        ArgumentNullException.ThrowIfNull(configuration);
        return configuration.GetSection(sectionName).GetSwaggerOption();
    }

    private static SwaggerOption GetSwaggerOption(this IConfigurationSection section)
        =>
        new(
            apiName: section["ApiName"] ?? string.Empty,
            apiVersion: section["ApiVersion"] ?? string.Empty,
            description: section["Description"]);
}