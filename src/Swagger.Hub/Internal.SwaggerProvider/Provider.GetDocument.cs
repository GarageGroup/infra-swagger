using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Reader;

namespace GarageGroup.Infra;

partial class HubSwaggerDocumentProvider
{
    public ValueTask<OpenApiDocument> GetDocumentAsync(string documentName, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return ValueTask.FromCanceled<OpenApiDocument>(cancellationToken);
        }

        return InnerGetDocumentAsync(cancellationToken);
    }

    private async ValueTask<OpenApiDocument> InnerGetDocumentAsync(CancellationToken cancellationToken)
    {
        var swagger = new OpenApiDocument
        {
            Info = option.Option.InitializeOpenApiInfo()
        };

        if (option.Documents.IsEmpty)
        {
            return swagger;
        }

        var documents = await Task.WhenAll(option.Documents.AsEnumerable().Select(InnerGetDocumentAsync)).ConfigureAwait(false);

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
                CombineUri(documentOption.BaseAddress, documentOption.DocumentUrl),
                documentResponse.StatusCode,
                responseMessage);

            return null;
        }

        if (string.IsNullOrEmpty(responseMessage))
        {
            logger?.LogError(
                "A swagger document '{url}' was empty",
                CombineUri(documentOption.BaseAddress, documentOption.DocumentUrl));

            return null;
        }

        var readResult = OpenApiModelFactory.Parse(
            input: responseMessage,
            format: GetFormat(documentOption.DocumentUrl),
            settings: null);

        if (readResult.Diagnostic?.Errors.Any() is true && IsYamlFormat(documentOption.DocumentUrl) is false)
        {
            // Fallback for YAML documents exposed via extensionless/non-yaml URL.
            readResult = OpenApiModelFactory.Parse(
                input: responseMessage,
                format: OpenApiConstants.Yaml,
                settings: null);
        }

        var document = readResult.Document;
        var diagnostic = readResult.Diagnostic;

        if (document is null)
        {
            logger?.LogError(
                "A swagger document '{url}' was not loaded because parser returned null document",
                CombineUri(documentOption.BaseAddress, documentOption.DocumentUrl));

            return null;
        }

        if (diagnostic?.Errors.Any() is true)
        {
            logger?.LogError(
                "A swagger document '{url}' was not loaded because of errors: {errors}",
                CombineUri(documentOption.BaseAddress, documentOption.DocumentUrl),
                string.Join(';', diagnostic.Errors.Select(GetErrorMessage)));

            return null;
        }

        if (diagnostic?.Warnings.Any() is true)
        {
            logger?.LogWarning(
                "A swagger document '{url}' was loaded with warnings: {warnings}",
                CombineUri(documentOption.BaseAddress, documentOption.DocumentUrl),
                string.Join(';', diagnostic.Warnings.Select(GetErrorMessage)));
        }

        document = document
            .ApplyUrlSuffix(documentOption.BaseAddress, documentOption.UrlSuffix)
            .JoinSecurityRequirements(documentOption.SecurityRequirements);

        if (documentOption.IsDirectCall is false)
        {
            return document;
        }

        var servers = new[]
        {
            documentOption.BaseAddress
        };

        return document.SetPathsServers(servers);

        static string GetErrorMessage(OpenApiError error)
            =>
            error.Message;
    }

    private static string CombineUri(Uri baseAddress, string documentUrl)
        =>
        new Uri(baseAddress, documentUrl).ToString();

    private static string GetFormat(string documentUrl)
        =>
        IsYamlFormat(documentUrl) ? OpenApiConstants.Yaml : OpenApiConstants.Json;

    private static bool IsYamlFormat(string documentUrl)
    {
        if (string.IsNullOrEmpty(documentUrl))
        {
            return false;
        }

        return documentUrl.EndsWith(".yaml", StringComparison.InvariantCultureIgnoreCase)
            || documentUrl.EndsWith(".yml", StringComparison.InvariantCultureIgnoreCase);
    }
}
