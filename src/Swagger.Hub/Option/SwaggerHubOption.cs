using System;

namespace GarageGroup.Infra;

public sealed record class SwaggerHubOption
{
    private static readonly SwaggerOption DefaultOption;

    static SwaggerHubOption()
        =>
        DefaultOption = new(apiName: "Swagger HUB API");

    public SwaggerHubOption(SwaggerOption option, FlatArray<SwaggerDocumentOption> documents)
    {
        Option = option ?? DefaultOption;
        Documents = documents;
    }

    public SwaggerOption Option { get; }

    public FlatArray<SwaggerDocumentOption> Documents { get; }
}