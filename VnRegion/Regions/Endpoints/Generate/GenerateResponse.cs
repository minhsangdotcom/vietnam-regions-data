namespace VnRegion.Regions.Endpoints.Generate;

public class GenerateResponse
{
    public string? SqlGenerationPath { get; set; }

    public UpdateMetaData? UpdateMetaData { get; set; }

    public RegionMetaData? ProvinceMetaData { get; set; }

    public RegionMetaData? DistrictMetaData { get; set; }

    public RegionMetaData? WardMetaData { get; set; }
}

public class RegionMetaData
{
    public string? Path { get; set; }

    public int Total { get; set; }
}

public class UpdateMetaData
{
    public string? Path { get; set; }

    public SummaryUpdate? ProvinceSummaryUpdate { get; set; }
    public SummaryUpdate? DistrictSummaryUpdate { get; set; }
    public SummaryUpdate? WardSummaryUpdate { get; set; }
}

public class SummaryUpdate
{
    public int AddedTotal { get; set; }
    public int UpdatedTotal { get; set; }
    public int RemoveTotal { get; set; }
}
