using System;
using System.Net.Http;
using GGroupp.Infra;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PrimeFuncPack;

namespace Microsoft.AspNetCore.Builder;

public static class SwaggerHubMiddleware
{
    public static TApplicationBuilder UseSwaggerHub<TApplicationBuilder>(
        this TApplicationBuilder applicationBuilder,
        Func<IServiceProvider, HttpMessageHandler> messageHandlerResolver,
        Func<IServiceProvider, SwaggerHubOption> optionResolver)
        where TApplicationBuilder : class, IApplicationBuilder
    {
        ArgumentNullException.ThrowIfNull(applicationBuilder);
        ArgumentNullException.ThrowIfNull(messageHandlerResolver);
        ArgumentNullException.ThrowIfNull(optionResolver);

        return applicationBuilder.UseSwagger(ResolveProvider);

        ISwaggerDocumentProvider ResolveProvider(IServiceProvider serviceProvider)
        {
            var messageHandler = messageHandlerResolver.Invoke(serviceProvider);
            var option = optionResolver.Invoke(serviceProvider);

            return HubSwaggerDocumentProvider.Create(messageHandler, option, serviceProvider.GetService<ILoggerFactory>());
        }
    }
}