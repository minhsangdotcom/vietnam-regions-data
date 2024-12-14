using VnRegion.Regions.Constants;
using VnRegion.Regions.Entities;
using VnRegion.Regions.Enums;
using VnRegion.Regions.Models;

namespace VnRegion.Regions.Mapping;

public class RegionMapping
{
    public static Province MapFromExcelModelToProvince(ExcelRegionModel source)
    {
        string name = GenerationHelper.GetShortName(source.Province!);
        string enProvince = GenerationHelper.GetEnglishName(source.Province!);

        Province destination =
            new()
            {
                Code = source.ProvinceCode,
                FullName = source.Province,
                Name = name.Trim(),
                EnglishName = GenerationHelper.RemoveUnicode(name),
                EnglishFullName = $"{GenerationHelper.RemoveUnicode(name)} " + enProvince,
                AdministrativeUnitId = GenerationHelper.GetAdministrativeUnits(source.Province!),
            };

        if (name!.All(x => char.IsDigit(x)) || source.ProvinceCode == "79")
        {
            destination.CustomName = destination.FullName;
        }
        return destination;
    }

    public static List<Province> MapFromIEnumrableExcelModelToListProvince(
        IEnumerable<ExcelRegionModel> sources
    )
    {
        static Province func(ExcelRegionModel source) => MapFromExcelModelToProvince(source);
        return [.. sources.Select(func)];
    }

    public static District MapFromExcelModelToDistrict(
        ExcelRegionModel source,
        Ulid? provinceId = null
    )
    {
        string fullName = source.District!.Contains('\'', StringComparison.CurrentCulture)
            ? source.District!.Insert(source.District.IndexOf('\'') + 1, "'")
            : source.District;

        string name = GenerationHelper.GetShortName(fullName);
        string enDistrict = GenerationHelper.GetEnglishName(fullName);

        bool isCity = source.Province!.Contains(
            "thành phố",
            StringComparison.CurrentCultureIgnoreCase
        );

        string nameWithoutUnicode = GenerationHelper.RemoveUnicode(name);
        string englishFullName = int.TryParse(name, out _)
            ? enDistrict + $" {nameWithoutUnicode}"
            : $"{nameWithoutUnicode} " + enDistrict;

        District destination =
            new()
            {
                Code = source.DistrictCode,
                FullName = fullName,
                Name = name,
                EnglishName = nameWithoutUnicode,
                EnglishFullName = englishFullName,
                ProvinceCode = source.ProvinceCode,
                AdministrativeUnitId = GenerationHelper.GetAdministrativeUnits(
                    source.District!,
                    isCity ? RegionType.City : RegionType.Province
                ),
                ProvinceId = provinceId,
            };

        if (name!.All(x => char.IsDigit(x)))
        {
            destination.CustomName = destination.FullName;
        }

        return destination;
    }

    public static List<District> MapFromIEnumrableExcelModelToListDistrict(
        IEnumerable<ExcelRegionModel> sources,
        IEnumerable<Province>? provinces = null
    )
    {
        District func(ExcelRegionModel source)
        {
            Ulid? provinceId = provinces?.FirstOrDefault(x => x.Code == source.ProvinceCode)?.Id;
            return MapFromExcelModelToDistrict(source, provinceId);
        }
        return [.. sources.Select(func)];
    }

    public static Ward MapFromExcelModelToWard(ExcelRegionModel source, Ulid? districtId = null)
    {
        string? fullName = source.Ward!.Contains('\'', StringComparison.CurrentCulture)
            ? source.Ward.Insert(source.Ward.IndexOf('\'') + 1, "'")
            : source.Ward;

        string name = GenerationHelper.GetShortName(fullName!);
        string enWard = GenerationHelper.GetEnglishName(fullName!);

        // Cache results of expensive operations
        string nameWithoutUnicode = GenerationHelper.RemoveUnicode(name);
        string englishFullName = int.TryParse(name, out _)
            ? $"{enWard} {nameWithoutUnicode}"
            : $"{nameWithoutUnicode} {enWard}";

        Ward destination =
            new()
            {
                Code = source.WardCode,
                FullName = fullName,
                Name = name,
                EnglishName = nameWithoutUnicode,
                EnglishFullName = englishFullName,
                DistrictCode = source.DistrictCode,
                AdministrativeUnitId = GenerationHelper.GetAdministrativeUnits(source.Ward!),
                DistrictId = districtId,
            };

        if (name.All(x => char.IsDigit(x)))
        {
            destination.CustomName = destination.FullName;
        }

        return destination;
    }

    public static List<Ward> MapFromIEnumrableExcelModelToListWard(
        IEnumerable<ExcelRegionModel> sources,
        IEnumerable<District>? districts = null
    )
    {
        Ward func(ExcelRegionModel source)
        {
            Ulid? districtId = districts?.FirstOrDefault(x => x.Code == source.DistrictCode)?.Id;
            return MapFromExcelModelToWard(source, districtId);
        }
        return [.. sources.Select(func)];
    }

    public static UpdateWard MapFromExcelModelToUpdateWard(ExcelRegionModel source)
    {
        string? fullName = source.Ward!.Contains('\'', StringComparison.CurrentCulture)
            ? source.Ward.Insert(source.Ward.IndexOf('\'') + 1, "'")
            : source.Ward;

        string name = GenerationHelper.GetShortName(fullName!);
        string enWard = GenerationHelper.GetEnglishName(fullName!);

        // Cache results of expensive operations
        string nameWithoutUnicode = GenerationHelper.RemoveUnicode(name);
        string englishFullName = int.TryParse(name, out _)
            ? $"{enWard} {nameWithoutUnicode}"
            : $"{nameWithoutUnicode} {enWard}";

        UpdateWard destination =
            new()
            {
                Code = source.WardCode,
                FullName = fullName,
                Name = name,
                EnglishName = nameWithoutUnicode,
                EnglishFullName = englishFullName,
                DistrictCode = source.DistrictCode,
                AdministrativeUnitId = GenerationHelper.GetAdministrativeUnits(source.Ward!),
            };

        if (name.All(x => char.IsDigit(x)))
        {
            destination.CustomName = destination.FullName;
        }

        return destination;
    }

    public static UpdateDistrict MapFromExcelModelToUpdateDistrict(ExcelRegionModel source)
    {
        string fullName = source.District!.Contains('\'', StringComparison.CurrentCulture)
            ? source.District!.Insert(source.District.IndexOf('\'') + 1, "'")
            : source.District;

        string name = GenerationHelper.GetShortName(fullName);
        string enDistrict = GenerationHelper.GetEnglishName(fullName);

        bool isCity = source.Province!.Contains(
            "thành phố",
            StringComparison.CurrentCultureIgnoreCase
        );

        string nameWithoutUnicode = GenerationHelper.RemoveUnicode(name);
        string englishFullName = int.TryParse(name, out _)
            ? enDistrict + $" {nameWithoutUnicode}"
            : $"{nameWithoutUnicode} " + enDistrict;

        UpdateDistrict destination =
            new()
            {
                Code = source.DistrictCode,
                FullName = fullName,
                Name = name,
                EnglishName = nameWithoutUnicode,
                EnglishFullName = englishFullName,
                ProvinceCode = source.ProvinceCode,
                AdministrativeUnitId = GenerationHelper.GetAdministrativeUnits(
                    source.District!,
                    isCity ? RegionType.City : RegionType.Province
                ),
            };

        if (name!.All(x => char.IsDigit(x)))
        {
            destination.CustomName = destination.FullName;
        }

        return destination;
    }
}
