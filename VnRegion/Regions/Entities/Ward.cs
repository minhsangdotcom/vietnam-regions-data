namespace VnRegion.Regions.Entities;

public class Ward : Region
{
    public string? DistrictCode { get; set; }

    public Ulid? DistrictId { get; set; }
}
