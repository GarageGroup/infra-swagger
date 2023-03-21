using System;
using System.Diagnostics.CodeAnalysis;

namespace GGroupp.Infra;

public sealed record class SwaggerDocumentOption
{
    private const string DefaultDocumentUrl = "/swagger/swagger.json";

    public SwaggerDocumentOption(Uri baseAddress, [AllowNull] string documentUrl = DefaultDocumentUrl, Uri? proxyAddress = null)
    {
        BaseAddress = baseAddress;
        DocumentUrl = string.IsNullOrEmpty(documentUrl) ? DefaultDocumentUrl : documentUrl;
        ProxyAddress = proxyAddress;
    }

    public Uri BaseAddress { get; }

    public string DocumentUrl { get; }

    public Uri? ProxyAddress { get; }
}