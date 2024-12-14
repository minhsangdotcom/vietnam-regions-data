using CaseConverter;
using VnRegion.Regions.Enums;
using VnRegion.Regions.Settings;

namespace VnRegion.Regions.Constants;

public class GenerationHelper
{
    public static string Get(
        string dbsetting,
        object? model = default,
        Dictionary<string, string>? columnNames = null,
        bool isUnderscoreLower = false,
        params string[] ignores
    )
    {
        if (model == null && columnNames == null)
        {
            throw new Exception("parameters is not null");
        }

        string result = "(";

        IEnumerable<string> columns =
            model == null
                ? columnNames!.Select(x => x.Value)
                : model!.GetType().GetProperties().Select(x => x.Name);

        if (isUnderscoreLower)
        {
            columns = columns.Where(x => !ignores.Contains(x)).Select(x => x.ToSnakeCase());
        }

        foreach (var column in columns!)
        {
            result += dbsetting == DbSettingName.PostgreSql ? $"\"{column}\"," : $"{column},";
        }

        result = result.Remove(result.Length - 1, 1) + ")";

        return result;
    }

    public static string GetShortName(string name)
    {
        ArgumentNullException.ThrowIfNull(name, nameof(name));

        string lowerName = name.ToLower().Trim();

        string[] splitName = name.Split(" ");

        string shortName = string.Empty;

        int index =
            lowerName.Contains(RegionName.City)
            || lowerName.Contains(RegionName.Town)
            || lowerName.Contains(RegionName.Township)
                ? 2
            : lowerName.Contains(RegionName.UrbanDistrict)
            || lowerName.Contains(RegionName.District)
            || lowerName.Contains(RegionName.Province)
            || lowerName.Contains(RegionName.Ward)
            || lowerName.Contains(RegionName.Commune)
                ? 1
            : 0;

        for (int i = index; i < splitName.Length; i++)
        {
            shortName += splitName[i] + " ";
        }

        return shortName.Remove(shortName.Length - 1, 1);
    }

    public static string GetEnglishName(string name)
    {
        ArgumentNullException.ThrowIfNull(name, nameof(name));

        string lowerDistricName = name.ToLower();

        string enDistrict = string.Empty;

        foreach (var englishName in RegionName.VietNamEnglishRegionName)
        {
            if (lowerDistricName.Contains(englishName.Key))
            {
                enDistrict = englishName.Value;
            }
        }

        return enDistrict;
    }

    public static string GetViName(string name)
    {
        ArgumentNullException.ThrowIfNull(name, nameof(name));

        string lowerDistricName = name.ToLower();

        string enDistrict = string.Empty;

        foreach (var englishName in RegionName.VietNamEnglishRegionName)
        {
            if (lowerDistricName.Contains(englishName.Key))
            {
                enDistrict = englishName.Value;
            }
        }

        return enDistrict;
    }

    public static int GetAdministrativeUnits(string name, RegionType? parentType = null)
    {
        ArgumentNullException.ThrowIfNull(name, nameof(name));

        string lowerName = name.ToLower();

        int unit = 0;

        switch (true)
        {
            case bool when lowerName.Contains(RegionName.City):
                unit =
                    parentType == RegionType.City ? 3
                    : parentType == RegionType.Province ? 4
                    : 1;
                break;
            case bool when lowerName.Contains(RegionName.Province):
                unit = 2;
                break;
            case bool when lowerName.Contains(RegionName.UrbanDistrict):
                unit = 5;
                break;
            case bool when lowerName.Contains(RegionName.Town):
                unit = 6;
                break;
            case bool when lowerName.Contains(RegionName.District):
                unit = 7;
                break;
            case bool when lowerName.Contains(RegionName.Ward):
                unit = 8;
                break;
            case bool when lowerName.Contains(RegionName.Township):
                unit = 9;
                break;
            case bool when lowerName.Contains(RegionName.Commune):
                unit = 10;
                break;

            default:
                break;
        }
        ;

        return unit;
    }

    public static string RemoveUnicode(string text)
    {
        string[] arr1 =
        [
            "á",
            "à",
            "ả",
            "ã",
            "ạ",
            "â",
            "ấ",
            "ầ",
            "ẩ",
            "ẫ",
            "ậ",
            "ă",
            "ắ",
            "ằ",
            "ẳ",
            "ẵ",
            "ặ",
            "đ",
            "é",
            "è",
            "ẻ",
            "ẽ",
            "ẹ",
            "ê",
            "ế",
            "ề",
            "ể",
            "ễ",
            "ệ",
            "í",
            "ì",
            "ỉ",
            "ĩ",
            "ị",
            "ó",
            "ò",
            "ỏ",
            "õ",
            "ọ",
            "ô",
            "ố",
            "ồ",
            "ổ",
            "ỗ",
            "ộ",
            "ơ",
            "ớ",
            "ờ",
            "ở",
            "ỡ",
            "ợ",
            "ú",
            "ù",
            "ủ",
            "ũ",
            "ụ",
            "ư",
            "ứ",
            "ừ",
            "ử",
            "ữ",
            "ự",
            "ý",
            "ỳ",
            "ỷ",
            "ỹ",
            "ỵ",
        ];
        string[] arr2 =
        [
            "a",
            "a",
            "a",
            "a",
            "a",
            "a",
            "a",
            "a",
            "a",
            "a",
            "a",
            "a",
            "a",
            "a",
            "a",
            "a",
            "a",
            "d",
            "e",
            "e",
            "e",
            "e",
            "e",
            "e",
            "e",
            "e",
            "e",
            "e",
            "e",
            "i",
            "i",
            "i",
            "i",
            "i",
            "o",
            "o",
            "o",
            "o",
            "o",
            "o",
            "o",
            "o",
            "o",
            "o",
            "o",
            "o",
            "o",
            "o",
            "o",
            "o",
            "o",
            "u",
            "u",
            "u",
            "u",
            "u",
            "u",
            "u",
            "u",
            "u",
            "u",
            "u",
            "y",
            "y",
            "y",
            "y",
            "y",
        ];
        for (int i = 0; i < arr1.Length; i++)
        {
            text = text.Replace(arr1[i], arr2[i]);
            text = text.Replace(arr1[i].ToUpper(), arr2[i].ToUpper());
        }
        return text;
    }
}
