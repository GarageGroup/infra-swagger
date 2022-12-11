using System;
using GGroupp.Infra;

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
            description: section[prefix + "Description"]);
}