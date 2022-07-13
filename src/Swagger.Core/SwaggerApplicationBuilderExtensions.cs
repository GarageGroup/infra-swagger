using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Swagger;

namespace Microsoft.AspNetCore.Builder;

public static class SwaggerApplicationBuilderExtensions
{
    public static IApplicationBuilder UseSwagger(this IApplicationBuilder app, Func<IServiceProvider, ISwaggerProvider> swaggerProviderResolver)
    {
        _ = app ?? throw new ArgumentNullException(nameof(app));
        _ = swaggerProviderResolver ?? throw new ArgumentNullException(nameof(swaggerProviderResolver));

        return app.Use(InvokeSwaggerAsync);

        Task InvokeSwaggerAsync(HttpContext context, RequestDelegate next)
        {
            var middleware = new SwaggerMiddleware(next, new());
            var swaggerProvider = swaggerProviderResolver.Invoke(context.RequestServices);

            return middleware.Invoke(context, swaggerProvider);
        }
    }
}