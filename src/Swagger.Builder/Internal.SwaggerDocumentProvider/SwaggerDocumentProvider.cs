using System;
using Microsoft.OpenApi.Models;

namespace GGroupp.Infra.Swagger.Builder;

internal sealed partial class SwaggerDocumentProvider : ISwaggerDocumentProvider
{
    private readonly Lazy<OpenApiDocument> lazyDocumentTemplate;

    internal SwaggerDocumentProvider(SwaggerOption? swaggerOption, ISwaggerBuilder? swaggerBuilder)
    {
        lazyDocumentTemplate = new(CreateTemplate);

        OpenApiDocument CreateTemplate()
        {
            var document = swaggerBuilder?.Build() ?? new();
            if (swaggerOption is null)
            {
                return document;
            }

            document.Info ??= new();

            if (string.IsNullOrEmpty(swaggerOption.ApiName) is false)
            {
                document.Info.Title = swaggerOption.ApiName;
            }

            if (string.IsNullOrEmpty(swaggerOption.ApiVersion) is false)
            {
                document.Info.Version = swaggerOption.ApiVersion;
            }

            if (string.IsNullOrEmpty(swaggerOption.Description) is false)
            {
                document.Info.Description = swaggerOption.Description;
            }

            return document;
        }
    }
}