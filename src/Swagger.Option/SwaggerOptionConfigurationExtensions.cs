using System;
using GGroupp.Infra;

namespace Microsoft.Extensions.Configuration;

public static class SwaggerOptionConfigurationExtensions
{
    public static SwaggerOption GetSwaggerOption(this IConfiguration configuration, string sectionName = "Swagger")
    {
        _ = configuration ?? throw new ArgumentNullException(nameof(configuration));

        return configuration.GetSection(sectionName).GetSwaggerOption();
    }

    private static SwaggerOption GetSwaggerOption(this IConfigurationSection section)
        =>
        new(
            apiName: section["ApiName"],
            apiVersion: section["ApiVersion"],
            description: section["Description"]);
}