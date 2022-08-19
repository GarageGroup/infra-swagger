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
    private const string RouteTemplate = "/swagger/{documentName}/swagger.json";

    public static TApplicationBuilder UseSwagger<TApplicationBuilder>(
        this TApplicationBuilder applicationBuilder, Func<IServiceProvider, ISwaggerDocumentProvider> swaggerProviderResolver)
        where TApplicationBuilder : class, IApplicationBuilder
    {
        _ = applicationBuilder ?? throw new ArgumentNullException(nameof(applicationBuilder));
        _ = swaggerProviderResolver ?? throw new ArgumentNullException(nameof(swaggerProviderResolver));

        var route = new RouteBuilder(applicationBuilder).MapVerb(HttpMethod.Get.Method, RouteTemplate, InnerInvokeAsync).Build();

        _ = applicationBuilder.UseRouter(route);
        return applicationBuilder;

        Task InnerInvokeAsync(HttpContext context)
            =>
            context.InvokeAsync(swaggerProviderResolver);
    }

    private static async Task InvokeAsync(this HttpContext httpContext, Func<IServiceProvider, ISwaggerDocumentProvider> providerResolver)
    {
        var documentName = httpContext.GetDocumentName() ?? string.Empty;

        var swaggerHubProvider = providerResolver.Invoke(httpContext.RequestServices);
        var document = await swaggerHubProvider.GetDocumentAsync(documentName, httpContext.RequestAborted).ConfigureAwait(false);

        var json = document.Serialize(OpenApiSpecVersion.OpenApi3_0, OpenApiFormat.Json);

        httpContext.Response.StatusCode = StatusCodes.Status200OK;
        httpContext.Response.Headers["Content-Type"] = MediaTypeNames.Application.Json;

        await httpContext.Response.WriteAsync(json, httpContext.RequestAborted).ConfigureAwait(false);
    }

    private static string? GetDocumentName(this HttpContext httpContext)
        =>
        httpContext.GetRouteData().Values.TryGetValue("documentName", out var documentName) ? documentName?.ToString() : default;
}