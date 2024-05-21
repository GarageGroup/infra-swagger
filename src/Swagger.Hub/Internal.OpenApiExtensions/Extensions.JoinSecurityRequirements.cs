using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Models;

namespace GarageGroup.Infra;

partial class OpenApiDocumentExtensions
{
    internal static OpenApiDocument JoinSecurityRequirements(
        this OpenApiDocument source, FlatArray<KeyValuePair<string, OpenApiSecurityScheme>> securityRequirements)
    {
        if (securityRequirements.IsEmpty)
        {
            return source;
        }

        source.Components.SecuritySchemes = source.Components.SecuritySchemes.Join(securityRequirements.AsEnumerable()).ToDictionary();
        source.Paths = source.InnerGetOpenApiPathsWithSecurity(securityRequirements.Map(InnerCreateRequirement).ToArray());

        return source;

        static OpenApiSecurityRequirement InnerCreateRequirement(KeyValuePair<string, OpenApiSecurityScheme> item)
            =>
            new()
            {
                [item.Value] = []
            };
    }
}