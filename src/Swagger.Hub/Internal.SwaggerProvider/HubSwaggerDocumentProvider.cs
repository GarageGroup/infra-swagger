using System;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Readers;

namespace GarageGroup.Infra;

internal sealed partial class HubSwaggerDocumentProvider : IHubSwaggerDocumentProvider
{
    public static HubSwaggerDocumentProvider Create(HttpMessageHandler httpMessageHandler, SwaggerHubOption option, ILoggerFactory? loggerFactory)
    {
        ArgumentNullException.ThrowIfNull(httpMessageHandler);
        ArgumentNullException.ThrowIfNull(option);

        return new(httpMessageHandler, option, loggerFactory?.CreateLogger<HubSwaggerDocumentProvider>());
    }

    private static readonly OpenApiStringReader OpenApiReader;

    static HubSwaggerDocumentProvider()
        =>
        OpenApiReader = new();

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