using System;

namespace GarageGroup.Infra;

public sealed record class SwaggerLicenseOption
{
    public string? Name { get; init; }

    public Uri? Url { get; init; }
}