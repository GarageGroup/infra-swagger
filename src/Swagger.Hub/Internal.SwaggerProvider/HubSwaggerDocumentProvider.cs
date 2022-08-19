using System;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Readers;
using Microsoft.OpenApi.Readers.Interface;

namespace GGroupp.Infra;

internal sealed partial class HubSwaggerDocumentProvider : ISwaggerDocumentProvider
{
    public static HubSwaggerDocumentProvider Create(HttpMessageHandler httpMessageHandler, SwaggerHubOption option, ILoggerFactory? loggerFactory)
    {
        _ = httpMessageHandler ?? throw new ArgumentNullException(nameof(httpMessageHandler));
        _ = option ?? throw new ArgumentNullException(nameof(option));

        return new(httpMessageHandler, option, loggerFactory?.CreateLogger<HubSwaggerDocumentProvider>());
    }

    private static readonly IOpenApiReader<string, OpenApiDiagnostic> OpenApiReader;

    static HubSwaggerDocumentProvider()
        =>
        OpenApiReader = new OpenApiStringReader();

    private HubSwaggerDocumentProvider(HttpMessageHandler httpMessageHandler, SwaggerHubOption option, ILogger? logger)
    {
        this.httpMessageHandler = httpMessageHandler;
        this.option = option;
        this.logger = logger;
    }

    private readonly HttpMessageHandler httpMessageHandler;

    private readonly SwaggerHubOption option;

    private readonly ILogger? logger;
}