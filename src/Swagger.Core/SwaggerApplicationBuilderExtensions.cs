using System;
using System.Linq;
using System.Net.Http;
using System.Net.Mime;
using System.Threading.Tasks;
using GarageGroup.Infra;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Extensions;

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
        var format = httpContext.GetOpenApiFormat();

        var swaggerDocumentProvider = providerResolver.Invoke(httpContext.RequestServices);
        var document = await swaggerDocumentProvider.GetDocumentAsync(string.Empty, httpContext.RequestAborted).ConfigureAwait(false);

        var json = document.Serialize(OpenApiSpecVersion.OpenApi3_0, format);

        httpContext.Response.StatusCode = StatusCodes.Status200OK;
        httpContext.Response.Headers["Content-Type"] = format is OpenApiFormat.Json ? MediaTypeNames.Application.Json : "application/yaml";

        await httpContext.Response.WriteAsync(json, httpContext.RequestAborted).ConfigureAwait(false);
    }

    private static OpenApiFormat GetOpenApiFormat(this HttpContext httpContext)
    {
        string? format = null;

        if (httpContext.GetRouteData().Values.TryGetValue(FormatRouteName, out var value))
        {
            format = value?.ToString();
        }

        if (string.IsNullOrEmpty(format))
        {
            return default;
        }

        if (YamlFormats.Contains(format, StringComparer.InvariantCultureIgnoreCase))
        {
            return OpenApiFormat.Yaml;
        }

        return OpenApiFormat.Json;
    }
}