using VnRegion.Common;

namespace VnRegion.Regions.Entities;

public class District : Region
{
    public string? ProvinceCode { get; set; }

    public Ulid? ProvinceId { get; set; }
}
