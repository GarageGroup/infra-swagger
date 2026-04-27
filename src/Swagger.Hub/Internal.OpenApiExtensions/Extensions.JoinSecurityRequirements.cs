using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi;

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

        source.Components ??= new();
        source.Components.SecuritySchemes = source.Components.SecuritySchemes
            .Join(securityRequirements.AsEnumerable().Select(InnerMapSecurityScheme))
            .ToDictionary();

        source.Paths = source.InnerGetOpenApiPathsWithSecurity(securityRequirements.Map(InnerCreateRequirement).ToArray());

        return source;

        KeyValuePair<string, IOpenApiSecurityScheme> InnerMapSecurityScheme(KeyValuePair<string, OpenApiSecurityScheme> item)
            =>
            new(item.Key, item.Value);

        OpenApiSecurityRequirement InnerCreateRequirement(KeyValuePair<string, OpenApiSecurityScheme> item)
            =>
            new()
            {
                [new(item.Key, source, string.Empty)] = []
            };
    }
}
