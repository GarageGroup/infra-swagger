using System.Diagnostics.CodeAnalysis;

namespace GGroupp.Infra;

public sealed record class SwaggerOption
{
    private const string DefaultApiName = "API";

    private const string DefaultApiVersion = "v1";

    public SwaggerOption(
        string apiName = DefaultApiName,
        string apiVersion = DefaultApiVersion,
        [AllowNull] string description = null,
        bool useAuthorization = false)
    {
        ApiName = string.IsNullOrEmpty(apiName) ? DefaultApiName : apiName;
        ApiVersion = string.IsNullOrEmpty(apiVersion) ? DefaultApiVersion : apiVersion;
        Description = string.IsNullOrEmpty(description) ? null : description;
        UseAuthorization = useAuthorization;
    }

    public string ApiName { get; }

    public string ApiVersion { get; }

    public string? Description { get; }

    public bool UseAuthorization { get; }
}