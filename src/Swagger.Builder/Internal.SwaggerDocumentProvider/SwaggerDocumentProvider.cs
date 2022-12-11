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

            document.Info = swaggerOption.InitializeOpenApiInfo();
            return document;
        }
    }
}