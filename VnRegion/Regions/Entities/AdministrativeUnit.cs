using VnRegion.Common;

namespace VnRegion.Regions.Entities;

public class AdministrativeUnit : BaseEntity<int>
{
    public string? FullName { get; set; }

    public string? FullNameEn { get; set; }

    public string? Shortname { get; set; }

    public string? ShortnameEn { get; set; }
}
