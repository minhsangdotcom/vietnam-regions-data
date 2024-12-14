namespace VnRegion.Regions.Constants;

public class RegionName
{
    public const string City = "thành phố";
    public const string Province = "tỉnh";
    public const string UrbanDistrict = "quận";
    public const string District = "huyện";
    public const string Town = "thị xã";
    public const string Ward = "phường";
    public const string Township = "thị trấn";
    public const string Commune = "xã";

    public static readonly Dictionary<string, string> VietNamEnglishRegionName =
        new()
        {
            { City, nameof(City) },
            { Province, nameof(Province) },
            { UrbanDistrict, nameof(District) },
            { District, nameof(District) },
            { Town, nameof(Town) },
            { Ward, nameof(Ward) },
            { Township, nameof(Township) },
            { Commune, nameof(Commune) },
        };

    public static readonly string OutputPath = Path.Combine(
        Directory.GetCurrentDirectory(),
        "Outputs"
    );
}
