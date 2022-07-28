using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Swagger;

namespace Microsoft.AspNetCore.Builder;

public static class SwaggerApplicationBuilderExtensions
{
    public static TApplicationBuilder UseSwagger<TApplicationBuilder>(
        this TApplicationBuilder app, Func<IServiceProvider, ISwaggerProvider> swaggerProviderResolver)
        where TApplicationBuilder : class, IApplicationBuilder
    {
        _ = app ?? throw new ArgumentNullException(nameof(app));
        _ = swaggerProviderResolver ?? throw new ArgumentNullException(nameof(swaggerProviderResolver));

        _ = app.Use(InvokeSwaggerAsync);
        return app;

        Task InvokeSwaggerAsync(HttpContext context, RequestDelegate next)
        {
            var middleware = new SwaggerMiddleware(next, new());
            var swaggerProvider = swaggerProviderResolver.Invoke(context.RequestServices);

            return middleware.Invoke(context, swaggerProvider);
        }
    }
}