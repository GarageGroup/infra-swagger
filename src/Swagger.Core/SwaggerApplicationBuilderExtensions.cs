using System;
using System.Linq;
using System.Net.Http;
using System.Net.Mime;
using System.Threading.Tasks;
using GarageGroup.Infra;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.OpenApi;

namespace Microsoft.AspNetCore.Builder;

public static class SwaggerApplicationBuilderExtensions
{
    private const string FormatRouteName
        =
        "format";

    private const string RouteTemplate
        =
        "/swagger/swagger.{" + FormatRouteName + "}";

    private static readonly string[] YamlFormats
        =
        ["yaml", "yml"];

    public static TApplicationBuilder UseSwagger<TApplicationBuilder>(
        this TApplicationBuilder applicationBuilder, Func<IServiceProvider, ISwaggerDocumentProvider> swaggerProviderResolver)
        where TApplicationBuilder : class, IApplicationBuilder
    {
        ArgumentNullException.ThrowIfNull(applicationBuilder);
        ArgumentNullException.ThrowIfNull(swaggerProviderResolver);

        var route = new RouteBuilder(applicationBuilder).MapVerb(HttpMethod.Get.Method, RouteTemplate, InnerInvokeAsync).Build();

        _ = applicationBuilder.UseRouter(route);
        return applicationBuilder;

        Task InnerInvokeAsync(HttpContext context)
            =>
            context.InvokeAsync(swaggerProviderResolver);
    }

    private static async Task InvokeAsync(this HttpContext httpContext, Func<IServiceProvider, ISwaggerDocumentProvider> providerResolver)
    {
        var isYaml = httpContext.IsOpenApiYamsJson();

        var swaggerDocumentProvider = providerResolver.Invoke(httpContext.RequestServices);
        var document = await swaggerDocumentProvider.GetDocumentAsync(string.Empty, httpContext.RequestAborted).ConfigureAwait(false);

        var format = isYaml ? OpenApiConstants.Yaml : OpenApiConstants.Json;
        var json = await document.SerializeAsync(OpenApiSpecVersion.OpenApi3_0, format, httpContext.RequestAborted).ConfigureAwait(false);

        httpContext.Response.StatusCode = StatusCodes.Status200OK;
        httpContext.Response.Headers["Content-Type"] = isYaml ? MediaTypeNames.Application.Yaml : MediaTypeNames.Application.Json;

        await httpContext.Response.WriteAsync(json, httpContext.RequestAborted).ConfigureAwait(false);
    }

    private static bool IsOpenApiYamsJson(this HttpContext httpContext)
    {
        string? format = null;

        if (httpContext.GetRouteData().Values.TryGetValue(FormatRouteName, out var value))
        {
            format = value?.ToString();
        }

        return YamlFormats.Contains(format, StringComparer.InvariantCultureIgnoreCase);
    }
}
