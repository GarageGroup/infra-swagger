using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace GGroupp.Infra;

partial class HubSwaggerDocumentProvider
{
    public ValueTask<OpenApiDocument> GetDocumentAsync(string documentName, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return ValueTask.FromCanceled<OpenApiDocument>(cancellationToken);
        }

        return InnerGetDocumentAsync(documentName, cancellationToken);
    }

    private async ValueTask<OpenApiDocument> InnerGetDocumentAsync(string? documentName, CancellationToken cancellationToken)
    {
        var swagger = new OpenApiDocument
        {
            Info = new OpenApiInfo
            {
                Title = option.Option.ApiName,
                Version = string.IsNullOrEmpty(documentName) ?  option.Option.ApiVersion : documentName,
                Description = option.Option.Description
            }
        };

        if (option.Documents.Any() is false)
        {
            return swagger;
        }

        var documents = await Task.WhenAll(option.Documents.Select(InnerGetDocumentAsync)).ConfigureAwait(false);

        foreach (var document in documents)
        {
            swagger = swagger.Join(document);
        }

        return swagger;

        Task<OpenApiDocument?> InnerGetDocumentAsync(SwaggerDocumentOption documentOption)
            =>
            GetDocumentAsync(documentOption, cancellationToken);
    }

    private async Task<OpenApiDocument?> GetDocumentAsync(SwaggerDocumentOption documentOption, CancellationToken cancellationToken)
    {
        using var httpClient = new HttpClient(handler: httpMessageHandler, disposeHandler: false)
        {
            BaseAddress = documentOption.BaseAddress
        };

        var documentResponse = await httpClient.GetAsync(documentOption.DocumentUrl, cancellationToken).ConfigureAwait(false);
        var responseMessage = await documentResponse.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        if (documentResponse.IsSuccessStatusCode is false)
        {
            logger?.LogError(
                "A swagger document '{url}' was not loaded. Status: {statusCode}, message: {responseMessage}",
                new Uri(documentOption.BaseAddress, documentOption.DocumentUrl),
                documentResponse.StatusCode,
                responseMessage);

            return null;
        }

        if (string.IsNullOrEmpty(responseMessage))
        {
            logger?.LogError(
                "A swagger document '{url}' was empty",
                new Uri(documentOption.BaseAddress, documentOption.DocumentUrl));

            return null;
        }

        var document = OpenApiReader.Read(responseMessage, out var diagnostic);
        
        if (diagnostic?.Errors.Any() is true)
        {
            logger?.LogError(
                "A swagger document '{url}' was not loaded because of errors: {errors}",
                new Uri(documentOption.BaseAddress, documentOption.DocumentUrl),
                string.Join(';', diagnostic.Errors.Select(GetErrorMessage)));

            return null;
        }

        if (diagnostic?.Warnings.Any() is true)
        {
            logger?.LogWarning(
                "A swagger document '{url}' was loaded with warnings: {warnings}",
                new Uri(documentOption.BaseAddress, documentOption.DocumentUrl),
                string.Join(';', diagnostic.Warnings.Select(GetErrorMessage)));
        }

        var servers = new[]
        {
            documentOption.ProxyAddress ?? documentOption.BaseAddress
        };

        return document.SetPathsServers(servers);

        static string GetErrorMessage(OpenApiError error)
            =>
            error.Message;
    }
}