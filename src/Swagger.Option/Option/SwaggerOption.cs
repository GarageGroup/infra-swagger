using System;
using System.Diagnostics.CodeAnalysis;

namespace GGroupp.Infra;

public sealed record class SwaggerOption
{
    private const string DefaultApiName = "API";

    private const string DefaultApiVersion = "v1";

    public SwaggerOption(
        string apiName = DefaultApiName,
        string apiVersion = DefaultApiVersion,
        [AllowNull] string description = null)
    {
        ApiName = string.IsNullOrEmpty(apiName) ? DefaultApiName : apiName;
        ApiVersion = string.IsNullOrEmpty(apiVersion) ? DefaultApiVersion : apiVersion;
        Description = string.IsNullOrEmpty(description) ? null : description;
    }

    public string ApiName { get; }

    public string ApiVersion { get; }

    public string? Description { get; }

    public Uri? TermsOfService { get; init; }

    public SwaggerContactOption? Contact { get; init; }

    public SwaggerLicenseOption? License { get; init; }
}