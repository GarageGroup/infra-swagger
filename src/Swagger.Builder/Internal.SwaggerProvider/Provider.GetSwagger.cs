using Microsoft.OpenApi.Models;

namespace GGroupp.Infra.Swagger.Builder;

partial class SwaggerProvider
{
    public OpenApiDocument GetSwagger(string documentName, string? host = null, string? basePath = null)
        =>
        CreateDocument(lazyDocumentTemplate.Value, documentName);

    private static OpenApiDocument CreateDocument(OpenApiDocument template, string documentName)
        =>
        new()
        {
            Info = new()
            {
                Title = string.IsNullOrEmpty(documentName) is false ? documentName : template.Info?.Title,
                Version = template.Info?.Version,
                Description = template.Info?.Description
            },
            Workspace = template.Workspace,
            Servers = template.Servers,
            Paths = template.Paths,
            Components = template.Components,
            SecurityRequirements = template.SecurityRequirements,
            Tags = template.Tags,
            ExternalDocs = template.ExternalDocs,
            Extensions = template.Extensions
        };
}