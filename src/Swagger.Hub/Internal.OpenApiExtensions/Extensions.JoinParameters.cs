using System;
using System.Linq;
using Microsoft.OpenApi.Models;

namespace GarageGroup.Infra;

partial class OpenApiDocumentExtensions
{
    internal static OpenApiDocument JoinParameters(this OpenApiDocument source, FlatArray<OpenApiParameter> parameters)
    {
        if (parameters.IsEmpty)
        {
            return source;
        }

        foreach (var path in source.Paths.Values)
        {
            parameters.ForEach(path.InnerAddParameter);
        }

        return source;
    }

    private static void InnerAddParameter(this OpenApiPathItem path, OpenApiParameter parameter)
    {
        if (path.Parameters.Any(IsNameEqualToParameterName))
        {
            return;
        }

        path.Parameters.Add(parameter);

        bool IsNameEqualToParameterName(OpenApiParameter pathParameter)
            =>
            string.Equals(pathParameter?.Name, parameter.Name, StringComparison.InvariantCultureIgnoreCase);
    }
}