using VnRegion.Regions.Constants;

namespace VnRegion.Regions.Services;

public interface IGenerator
{
    public IGenerate GetGenerationService(GenerationType type);
}
