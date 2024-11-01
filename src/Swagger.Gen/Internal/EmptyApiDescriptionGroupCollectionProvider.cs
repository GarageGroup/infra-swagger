using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace GarageGroup.Infra;

internal sealed class EmptyApiDescriptionGroupCollectionProvider : IApiDescriptionGroupCollectionProvider
{
    private readonly ApiDescriptionGroupCollection apiDescriptionGroups
        =
        new([], 1);

    public ApiDescriptionGroupCollection ApiDescriptionGroups
        =>
        apiDescriptionGroups;
}