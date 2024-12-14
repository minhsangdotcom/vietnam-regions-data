using VnRegion.Regions.Entities;

namespace VnRegion.Regions.Models;

public class DataChanges
{
    public List<WardChange>? WardChanges { get; set; }

    public List<DistrictChange>? DistrictChanges { get; set; }
}

public class WardChange
{
    public string? Code { get; set; }

    public Ward? Old { get; set; }

    public Ward? New { get; set; }

    public List<Change>? Changes { get; set; }

    public UpdateWard? Update { get; set; }

    public ChangeType Type { get; set; }
}

public class UpdateWard
{
    public string? Code { get; set; }

    public string? Name { get; set; }

    public string? EnglishName { get; set; }

    public string? FullName { get; set; }

    public string? EnglishFullName { get; set; }

    public Ulid? DistrictId { get; set; }

    public string? DistrictCode { get; set; }

    public string? CustomName { get; set; }

    public int AdministrativeUnitId { get; set; }
}

public class DistrictChange
{
    public string? Code { get; set; }

    public District? Old { get; set; }

    public District? New { get; set; }

    public List<Change>? Changes { get; set; }

    public UpdateDistrict? Update { get; set; }

    public ChangeType Type { get; set; }
}

public class UpdateDistrict
{
    public string? Code { get; set; }

    public string? Name { get; set; }

    public string? EnglishName { get; set; }

    public string? FullName { get; set; }

    public string? EnglishFullName { get; set; }

    public Ulid? ProvinceId { get; set; }

    public string? ProvinceCode { get; set; }

    public string? CustomName { get; set; }

    public int AdministrativeUnitId { get; set; }
}

public class Change
{
    public string? Property { get; set; }

    public string? Old { get; set; }

    public string? Current { get; set; }
}

public enum ChangeType
{
    Addition = 1,
    Update = 2,
    Delete = 3,
}
