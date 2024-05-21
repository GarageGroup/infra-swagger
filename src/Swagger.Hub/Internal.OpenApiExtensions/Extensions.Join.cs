using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.OpenApi.Models;

namespace GarageGroup.Infra;

partial class OpenApiDocumentExtensions
{
    internal static OpenApiDocument Join(this OpenApiDocument source, OpenApiDocument? other)
    {
        if (other is null)
        {
            return source;
        }

        if (other.Paths?.Count > 0)
        {
            var otherPaths = other.InnerGetOpenApiPathsWithSecurity(other.SecurityRequirements?.ToArray());
            source.Paths = source.Paths.Join(otherPaths).ToDictionary<OpenApiPaths, OpenApiPathItem>();
        }

        if (other.Servers?.Count > 0)
        {
            source.Servers = source.Servers.Join(other.Servers).ToList();
        }

        if (other.Tags?.Count > 0)
        {
            source.Tags = source.Tags.Join(other.Tags).ToList();
        }

        if (other.Extensions?.Count > 0)
        {
            source.Extensions = source.Extensions.Join(other.Extensions).ToDictionary();
        }

        if (other.Components is not null)
        {
            source.Components = source.Components.Join(other.Components);
        }

        return source;
    }

    private static OpenApiComponents Join(this OpenApiComponents? source, OpenApiComponents? other)
    {
        if (other is null)
        {
            return source ?? new();
        }

        var result = source ?? new();

        if (other.Schemas?.Count > 0)
        {
            result.Schemas = result.Schemas.Join(other.Schemas).ToDictionary();
        }

        if (other.Responses?.Count > 0)
        {
            result.Responses = result.Responses.Join(other.Responses).ToDictionary();
        }

        if (other.Parameters?.Count > 0)
        {
            result.Parameters = result.Parameters.Join(other.Parameters).ToDictionary();
        }

        if (other.Examples?.Count > 0)
        {
            result.Examples = result.Examples.Join(other.Examples).ToDictionary();
        }

        if (other.RequestBodies?.Count > 0)
        {
            result.RequestBodies = result.RequestBodies.Join(other.RequestBodies).ToDictionary();
        }

        if (other.Headers?.Count > 0)
        {
            result.Headers = result.Headers.Join(other.Headers).ToDictionary();
        }

        if (other.SecuritySchemes?.Count > 0)
        {
            result.SecuritySchemes = result.SecuritySchemes.Join(other.SecuritySchemes).ToDictionary();
        }

        if (other.Links?.Count > 0)
        {
            result.Links = result.Links.Join(other.Links).ToDictionary();
        }

        if (other.Callbacks?.Count > 0)
        {
            result.Callbacks = result.Callbacks.Join(other.Callbacks).ToDictionary();
        }

        if (other.Extensions?.Count > 0)
        {
            result.Extensions = result.Extensions.Join(other.Extensions).ToDictionary();
        }

        return result;
    }

    private static OpenApiPaths InnerGetOpenApiPathsWithSecurity(
        this OpenApiDocument document, [AllowNull] IReadOnlyList<OpenApiSecurityRequirement> securityRequirements)
    {
        if (securityRequirements?.Count is not > 0)
        {
            return document.Paths;
        }

        foreach (var path in document.Paths.Values.SelectMany(GetOperations))
        {
            foreach (var security in securityRequirements)
            {
                if (path.Security.Contains(security))
                {
                    continue;
                }

                path.Security.Add(security);
            }
        }

        return document.Paths;

        static IEnumerable<OpenApiOperation> GetOperations(OpenApiPathItem pathItem)
            =>
            pathItem.Operations?.Select(GetValue) ?? [];

        static TValue GetValue<TKey, TValue>(KeyValuePair<TKey, TValue> pair)
            =>
            pair.Value;
    }
}