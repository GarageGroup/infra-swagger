using System;
using Microsoft.OpenApi;

namespace GarageGroup.Infra;

public interface ISwaggerBuilder
{
    ISwaggerBuilder Use(Action<OpenApiDocument> configurator);

    OpenApiDocument Build();
}