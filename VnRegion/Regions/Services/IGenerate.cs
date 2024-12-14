using VnRegion.Regions.Endpoints.Generate;

namespace VnRegion.Regions.Services;

public interface IGenerate
{
    Task<GenerateResponse> GenerateAsync(
        IFormFile sourceFile,
        IFormFile? updatedFile,
        string? output = null
    );
}
