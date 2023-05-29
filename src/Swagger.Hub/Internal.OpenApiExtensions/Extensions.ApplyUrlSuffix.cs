using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Models;

namespace GarageGroup.Infra;

partial class OpenApiDocumentExtensions
{
    internal static OpenApiDocument ApplyUrlSuffix(this OpenApiDocument source, Uri baseAddress, string? urlSuffix)
    {
        if (source.Paths?.Count is not > 0)
        {
            return source;
        }

        if (string.IsNullOrEmpty(urlSuffix))
        {
            return source;
        }

        var basePath = Slash + baseAddress.AbsolutePath?.Trim(Slash);
        var suffix = urlSuffix.Trim(Slash);

        var paths = new Dictionary<string, OpenApiPathItem>(source.Paths.Count);
        foreach (var path in source.Paths)
        {
            var pathKey = path.Key;
            if (pathKey.StartsWith(basePath, StringComparison.InvariantCultureIgnoreCase))
            {
                pathKey = pathKey[basePath.Length..];
            }

            if (pathKey.StartsWith(Slash) is false)
            {
                pathKey = Slash + pathKey;
            }

            pathKey = Slash + suffix + pathKey;
            paths.Add(pathKey, path.Value);
        }

        source.Paths = paths.ToDictionary<OpenApiPaths, OpenApiPathItem>();
        return source;
    }
}