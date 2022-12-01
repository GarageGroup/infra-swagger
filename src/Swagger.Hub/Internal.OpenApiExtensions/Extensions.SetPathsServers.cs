using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Models;

namespace GGroupp.Infra;

partial class OpenApiDocumentExtensions
{
    internal static OpenApiDocument SetPathsServers(this OpenApiDocument source, IReadOnlyCollection<Uri?>? serverAddresses)
    {
        if (serverAddresses?.Any() is not true || source.Paths?.Any() is not true)
        {
            return source;
        }

        var servers = GetServers();
        if (servers.Any() is false)
        {
            return source;
        }

        foreach (var path in source.Paths)
        {
            path.Value.Servers = path.Value.Servers.Join(servers).ToList();
        }

        return source;

        IEnumerable<OpenApiServer> GetServers()
        {
            foreach (var serverAddress in serverAddresses)
            {
                if (string.IsNullOrEmpty(serverAddress?.AbsoluteUri))
                {
                    continue;
                }

                yield return new()
                {
                    Url = serverAddress.AbsoluteUri
                };
            }
        }
    }
}