using System;

namespace GGroupp.Infra;

public sealed record class SwaggerLicenseOption
{
    public string? Name { get; init; }

    public Uri? Url { get; init; }
}