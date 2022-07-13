using System;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace GGroupp.Infra;

internal sealed class EmptyApiDescriptionGroupCollectionProvider : IApiDescriptionGroupCollectionProvider
{
    private readonly ApiDescriptionGroupCollection apiDescriptionGroups = new(Array.Empty<ApiDescriptionGroup>(), 1);

    public ApiDescriptionGroupCollection ApiDescriptionGroups => apiDescriptionGroups;
}