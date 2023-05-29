using System;
using System.Diagnostics.CodeAnalysis;

namespace GarageGroup.Infra;

public sealed record class SwaggerDocumentOption
{
    private const string DefaultDocumentUrl = "swagger/swagger.json";

    public SwaggerDocumentOption(Uri baseAddress, [AllowNull] string documentUrl = DefaultDocumentUrl)
    {
        BaseAddress = baseAddress;
        DocumentUrl = string.IsNullOrEmpty(documentUrl) ? DefaultDocumentUrl : documentUrl;
    }

    public Uri BaseAddress { get; }

    public string DocumentUrl { get; }

    public string? UrlSuffix { get; init; }

    public bool IsDirectCall { get; init; }
}