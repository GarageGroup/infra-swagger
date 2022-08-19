using System;
using System.Collections.Generic;

namespace GGroupp.Infra;

public sealed record class SwaggerHubOption
{
    private static readonly SwaggerOption DefaultOption;

    static SwaggerHubOption()
        =>
        DefaultOption = new(apiName: "Swagger HUB API");

    public SwaggerHubOption(SwaggerOption option, IReadOnlyCollection<SwaggerDocumentOption> documents)
    {
        Option = option ?? DefaultOption;
        Documents = documents ?? Array.Empty<SwaggerDocumentOption>();
    }

    public SwaggerOption Option { get; }

    public IReadOnlyCollection<SwaggerDocumentOption> Documents { get; }
}