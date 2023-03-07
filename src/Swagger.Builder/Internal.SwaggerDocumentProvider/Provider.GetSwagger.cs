using System.Threading;
using System.Threading.Tasks;
using Microsoft.OpenApi.Models;

namespace GGroupp.Infra.Swagger.Builder;

partial class SwaggerDocumentProvider
{
    public ValueTask<OpenApiDocument> GetDocumentAsync(string? documentName, CancellationToken cancellationToken = default)
    {
        var document = CreateDocument(lazyDocumentTemplate.Value, documentName);
        return ValueTask.FromResult(document);
    }

    private static OpenApiDocument CreateDocument(OpenApiDocument template, string? documentName)
        =>
        new()
        {
            Info = new()
            {
                Title = template.Info?.Title,
                Version = string.IsNullOrEmpty(documentName) ? template.Info?.Version : documentName,
                Description = template.Info?.Description,
                TermsOfService = template.Info?.TermsOfService,
                Contact = template.Info?.Contact is null ? null : new()
                {
                    Name = template.Info?.Contact.Name,
                    Email = template.Info?.Contact.Email,
                    Url = template.Info?.Contact.Url
                },
                License = template.Info?.License is null ? null : new()
                {
                    Name = template.Info?.License.Name,
                    Url = template.Info?.License.Url
                }
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