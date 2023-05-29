using System.Diagnostics.CodeAnalysis;
using Microsoft.OpenApi.Models;

namespace GarageGroup.Infra;

public static class SwaggerOptionExtensions
{
    [return: NotNullIfNotNull(nameof(option))]
    public static OpenApiInfo? InitializeOpenApiInfo(this SwaggerOption? option)
        =>
        option is null ? null : new()
        {
            Title = option.ApiName,
            Version = option.ApiVersion,
            Description = option.Description,
            TermsOfService = option.TermsOfService,
            Contact = option.Contact is null ? null : new()
            {
                Name = option.Contact.Name,
                Email = option.Contact.Email,
                Url = option.Contact.Url
            },
            License = option.License is null ? null : new()
            {
                Name = option.License.Name,
                Url = option.License.Url
            }
        };
}