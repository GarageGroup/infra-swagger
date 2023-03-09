using System;
using System.Net.Http;
using System.Net.Mime;
using System.Threading.Tasks;
using GGroupp.Infra;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Extensions;

namespace Microsoft.AspNetCore.Builder;

public static class SwaggerApplicationBuilderExtensions
{
    private const string RouteTemplateJson = "/swagger/{documentName}/swagger.json";

    private const string RouteTemplateYaml = "/swagger/{documentName}/swagger.yaml";

    public static TApplicationBuilder UseSwagger<TApplicationBuilder>(
        this TApplicationBuilder applicationBuilder, Func<IServiceProvider, ISwaggerDocumentProvider> swaggerProviderResolver)
        where TApplicationBuilder : class, IApplicationBuilder
    {
        ArgumentNullException.ThrowIfNull(applicationBuilder);
        ArgumentNullException.ThrowIfNull(swaggerProviderResolver);

        var route = new RouteBuilder(applicationBuilder)
            .MapVerb(HttpMethod.Get.Method, RouteTemplateJson, InnerInvokeJsonAsync)
            .MapVerb(HttpMethod.Get.Method, RouteTemplateYaml, InnerInvokeYamlAsync)
            .Build();

        _ = applicationBuilder.UseRouter(route);
        return applicationBuilder;

        Task InnerInvokeJsonAsync(HttpContext context)
            =>
            context.InvokeAsync(swaggerProviderResolver, OpenApiFormat.Json);

        Task InnerInvokeYamlAsync(HttpContext context)
            =>
            context.InvokeAsync(swaggerProviderResolver, OpenApiFormat.Yaml);
    }

    private static async Task InvokeAsync(
        this HttpContext httpContext, Func<IServiceProvider, ISwaggerDocumentProvider> providerResolver, OpenApiFormat format)
    {
        var documentName = httpContext.GetDocumentName() ?? string.Empty;

        var swaggerHubProvider = providerResolver.Invoke(httpContext.RequestServices);
        var document = await swaggerHubProvider.GetDocumentAsync(documentName, httpContext.RequestAborted).ConfigureAwait(false);

        var json = document.Serialize(OpenApiSpecVersion.OpenApi3_0, format);

        httpContext.Response.StatusCode = StatusCodes.Status200OK;
        httpContext.Response.Headers["Content-Type"] = format is OpenApiFormat.Json ? MediaTypeNames.Application.Json : "application/yaml";

        await httpContext.Response.WriteAsync(json, httpContext.RequestAborted).ConfigureAwait(false);
    }

    private static string? GetDocumentName(this HttpContext httpContext)
        =>
        httpContext.GetRouteData().Values.TryGetValue("documentName", out var documentName) ? documentName?.ToString() : default;
}