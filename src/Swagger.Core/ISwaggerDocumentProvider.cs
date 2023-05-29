using System.Threading;
using System.Threading.Tasks;
using Microsoft.OpenApi.Models;

namespace GarageGroup.Infra;

public interface ISwaggerDocumentProvider
{
    ValueTask<OpenApiDocument> GetDocumentAsync(string documentName, CancellationToken cancellationToken = default);
}