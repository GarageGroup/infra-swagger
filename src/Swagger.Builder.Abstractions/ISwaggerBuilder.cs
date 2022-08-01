using System;
using Microsoft.OpenApi.Models;

namespace GGroupp.Infra;

public interface ISwaggerBuilder
{
    ISwaggerBuilder Use(Action<OpenApiDocument> configurator);

    OpenApiDocument Build();
}