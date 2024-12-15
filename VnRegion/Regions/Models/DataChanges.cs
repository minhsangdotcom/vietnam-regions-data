namespace VnRegion.Regions.Models;

public class DataChanges
{
    public List<WardChange>? WardChanges { get; set; }

    public List<DistrictChange>? DistrictChanges { get; set; }

    public List<ProvinceChange>? ProvinceChanges { get; set; }
}

public class WardChange
{
    public string? Code { get; set; }

    public UpdateWard? Old { get; set; }

    public UpdateWard? New { get; set; }

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

    public string? DistrictCode { get; set; }

    public string? CustomName { get; set; }

    public int AdministrativeUnitId { get; set; }
}

public class DistrictChange
{
    public string? Code { get; set; }

    public UpdateDistrict? Old { get; set; }

    public UpdateDistrict? New { get; set; }

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

    public string? ProvinceCode { get; set; }

    public string? CustomName { get; set; }

    public int AdministrativeUnitId { get; set; }
}

public class ProvinceChange
{
    public string? Code { get; set; }

    public UpdateProvince? Old { get; set; }

    public UpdateProvince? New { get; set; }

    public List<Change>? Changes { get; set; }

    public UpdateProvince? Update { get; set; }

    public ChangeType Type { get; set; }
}

public class UpdateProvince
{
    public string? Code { get; set; }

    public string? Name { get; set; }

    public string? EnglishName { get; set; }

    public string? FullName { get; set; }

    public string? EnglishFullName { get; set; }

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
