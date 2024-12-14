using VnRegion.Regions.Constants;

namespace VnRegion.Regions.Services;

public class Generator(IServiceProvider serviceProvider) : IGenerator
{
    public IGenerate GetGenerationService(GenerationType type)
    {
        return type switch
        {
            GenerationType.Json => (IGenerate)
                serviceProvider.GetRequiredService(typeof(GenerateJsonFileService)),
            GenerationType.Sql => (IGenerate)
                serviceProvider.GetRequiredService(typeof(GenerateSqlFileService)),
            _ => throw new NotImplementedException(),
        };
    }
}
