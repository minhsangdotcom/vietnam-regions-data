using System.Collections.Concurrent;
using System.Reflection;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Options;
using MiniExcelLibs;
using VnRegion.Common;
using VnRegion.Common.Extensions;
using VnRegion.Regions.Constants;
using VnRegion.Regions.Endpoints.Generate;
using VnRegion.Regions.Entities;
using VnRegion.Regions.Mapping;
using VnRegion.Regions.Models;
using VnRegion.Regions.Settings;

namespace VnRegion.Regions.Services;

public class GenerateJsonFileService(IOptions<NameConfigurationSettings> options) : IGenerate
{
    private readonly NameConfigurationSettings nameConfigurations = options.Value;

    public async Task<GenerateResponse> GenerateAsync(
        IFormFile sourceFile,
        IFormFile? updatedFile,
        string? output = null
    )
    {
        using MemoryStream stream = new();
        await sourceFile!.CopyToAsync(stream);

        IEnumerable<ExcelRegionModel> sourceFileRows = MiniExcel.Query<ExcelRegionModel>(stream);

        string path = output ?? RegionName.OutputPath;
        Directory.CreateDirectory(path);

        if (updatedFile != null)
        {
            using MemoryStream updatedFileStream = new();
            await updatedFile!.CopyToAsync(updatedFileStream);

            IEnumerable<ExcelRegionModel> updatedFileRows = MiniExcel.Query<ExcelRegionModel>(
                updatedFileStream
            );
            DataChanges dataChanges = HasChangeData(sourceFileRows, updatedFileRows);
            string changesPath = Path.Combine(path, "Changes.json");
            File.WriteAllText(changesPath, SerializerExtension.Serialize(dataChanges!).StringJson);

            return new()
            {
                UpdateMetaData = new()
                {
                    Path = changesPath,
                    WardSummaryUpdate = new()
                    {
                        AddedTotal =
                            dataChanges?.WardChanges?.Count(x => x.Type == ChangeType.Addition)
                            ?? 0,
                        UpdatedTotal =
                            dataChanges?.WardChanges?.Count(x => x.Type == ChangeType.Update) ?? 0,
                        RemoveTotal =
                            dataChanges?.WardChanges?.Count(x => x.Type == ChangeType.Delete) ?? 0,
                    },
                    DistrictSummaryUpdate = new()
                    {
                        AddedTotal =
                            dataChanges?.DistrictChanges?.Count(x => x.Type == ChangeType.Addition)
                            ?? 0,
                        UpdatedTotal =
                            dataChanges?.DistrictChanges?.Count(x => x.Type == ChangeType.Update)
                            ?? 0,
                        RemoveTotal =
                            dataChanges?.DistrictChanges?.Count(x => x.Type == ChangeType.Delete)
                            ?? 0,
                    },
                },
            };
        }

        List<Province> provines = RegionMapping.MapFromIEnumrableExcelModelToListProvince(
            sourceFileRows.Where(x => x.ProvinceCode != null).DistinctBy(x => x.ProvinceCode)
        );

        List<District> districts = RegionMapping.MapFromIEnumrableExcelModelToListDistrict(
            sourceFileRows
                .Where(x => x.DistrictCode != null)
                .DistinctBy(x => new { x.DistrictCode, x.ProvinceCode })
        );

        districts.ForEach(x =>
        {
            x.ProvinceId = provines.Find(p => p.Code == x.ProvinceCode)?.Id;
        });

        List<Ward> wards = RegionMapping.MapFromIEnumrableExcelModelToListWard(
            sourceFileRows.Where(x => x.WardCode != null).DistinctBy(x => x.WardCode)
        );

        wards.ForEach(x =>
        {
            x.DistrictId = districts.Find(p => p.Code == x.DistrictCode)?.Id;
        });

        List<AdministrativeUnit> administrativeUnits =
        [
            new()
            {
                Id = 1,
                FullName = "Thành phố trực thuộc trung ương",
                EnglishFullName = "Municipality",
                ShortName = "Thành phố",
                EnglishShortName = "City",
            },
            new()
            {
                Id = 2,
                FullName = "Tỉnh",
                EnglishFullName = "Province",
                ShortName = "Tỉnh",
                EnglishShortName = "Province",
            },
            new()
            {
                Id = 3,
                FullName = "Thành phố thuộc thành phố trực thuộc trung ương",
                EnglishFullName = "Municipal city",
                ShortName = "Thành phố",
                EnglishShortName = "City",
            },
            new()
            {
                Id = 4,
                FullName = "Thành phố thuộc tỉnh",
                EnglishFullName = "Provincial city",
                ShortName = "Thành phố",
                EnglishShortName = "City",
            },
            new()
            {
                Id = 5,
                FullName = "Quận",
                EnglishFullName = "Urban district",
                ShortName = "Quận",
                EnglishShortName = "District",
            },
            new()
            {
                Id = 6,
                FullName = "Thị xã",
                EnglishFullName = "District-level town",
                ShortName = "Thị xã",
                EnglishShortName = "Town",
            },
            new()
            {
                Id = 7,
                FullName = "Huyện",
                EnglishFullName = "District",
                ShortName = "Huyện",
                EnglishShortName = "District",
            },
            new()
            {
                Id = 8,
                FullName = "Phường",
                EnglishFullName = "Ward",
                ShortName = "Phường",
                EnglishShortName = "Ward",
            },
            new()
            {
                Id = 9,
                FullName = "Thị trấn",
                EnglishFullName = "Commune-level town",
                ShortName = "Thị trấn",
                EnglishShortName = "Township",
            },
            new()
            {
                Id = 10,
                FullName = "Xã",
                EnglishFullName = "Commune",
                ShortName = "Xã",
                EnglishShortName = "Commune",
            },
        ];

        string strProvinces =
            nameConfigurations.ProvinceConfigs == null
                ? SerializerExtension.Serialize(provines).StringJson
                : SerializeCustomName(nameConfigurations.ProvinceConfigs.TableName!, provines);

        string strDistricts =
            nameConfigurations.DistrictConfigs == null
                ? SerializerExtension.Serialize(districts).StringJson
                : SerializeCustomName(
                    nameConfigurations.DistrictConfigs.TableName!,
                    districts: districts
                );

        string strWards =
            nameConfigurations.WardConfigs == null
                ? SerializerExtension.Serialize(wards).StringJson
                : SerializeCustomName(nameConfigurations.WardConfigs!.TableName!, wards: wards);

        string strAdministrativeUnits =
            nameConfigurations.AdministrativeUnitConfigs == null
                ? SerializerExtension.Serialize(administrativeUnits).StringJson
                : SerializeCustomName(
                    nameConfigurations.AdministrativeUnitConfigs!.TableName!,
                    administrativeUnits: administrativeUnits
                );

        string provincePath = Path.Combine(path, "Provinces.json");
        string districtPath = Path.Combine(path, "Districts.json");
        string wardPath = Path.Combine(path, "Wards.json");
        string adminPath = Path.Combine(path, "AdministrativeUnits.json");

        File.WriteAllText(provincePath, strProvinces);
        File.WriteAllText(districtPath, strDistricts);
        File.WriteAllText(wardPath, strWards);
        File.WriteAllText(adminPath, strAdministrativeUnits);

        return new()
        {
            ProvinceMetaData = new() { Path = provincePath, Total = provines.Count },
            DistrictMetaData = new() { Path = districtPath, Total = districts.Count },
            WardMetaData = new() { Path = wardPath, Total = wards.Count },
            AdministrativeUnitData = new() { Path = adminPath, Total = administrativeUnits.Count },
        };
    }

    private string SerializeCustomName(
        string tableName,
        List<Province>? provinces = null,
        List<District>? districts = null,
        List<Ward>? wards = null,
        List<AdministrativeUnit>? administrativeUnits = null
    )
    {
        if (string.IsNullOrWhiteSpace(tableName))
            return string.Empty;

        var tableMapping = new Dictionary<string, (dynamic Config, dynamic Data)?>
        {
            {
                nameConfigurations.ProvinceConfigs!.TableName!,
                new(nameConfigurations.ProvinceConfigs, provinces!)
            },
            {
                nameConfigurations.DistrictConfigs!.TableName!,
                new(nameConfigurations.DistrictConfigs, districts!)
            },
            {
                nameConfigurations.WardConfigs!.TableName!,
                new(nameConfigurations.WardConfigs, wards!)
            },
            {
                nameConfigurations.AdministrativeUnitConfigs!.TableName!,
                new(nameConfigurations.AdministrativeUnitConfigs, administrativeUnits!)
            },
        };

        if (
            !tableMapping.TryGetValue(tableName, out var mapping)
            || mapping == null
            || mapping.Value.Data == null
        )
            return string.Empty;

        // Serialize based on configuration
        var (config, data) = mapping!.Value;
        return SerializeEntities(config.ColumnNames, data);
    }

    private static string SerializeEntities<T>(
        Dictionary<string, string> columnNames,
        List<T> entities
    )
    {
        var jsonArray = new JsonArray();

        foreach (var entity in entities)
        {
            var jsonObject = new JsonObject();

            foreach (var propertyInfo in typeof(T).GetProperties())
            {
                string? propertyName =
                    columnNames.GetValueOrDefault(propertyInfo.Name) ?? propertyInfo.Name;

                if (
                    string.IsNullOrWhiteSpace(propertyName)
                    || propertyName == nameof(BaseEntity.CreatedAt)
                )
                    continue;

                if (propertyName == nameof(Region.CustomName))
                {
                    var customName = propertyInfo.GetValue(entity)?.ToString();
                    if (!string.IsNullOrEmpty(customName))
                    {
                        jsonObject[propertyName] = customName;
                    }
                    continue;
                }

                // Add other properties
                jsonObject[propertyName] = propertyInfo.GetValue(entity)?.ToString();
            }

            jsonArray.Add(jsonObject);
        }

        return SerializerExtension.Serialize(jsonArray).StringJson;
    }

    private static DataChanges HasChangeData(
        IEnumerable<ExcelRegionModel> sourceFile,
        IEnumerable<ExcelRegionModel> updatedFile
    )
    {
        List<WardChange> wardChanges = WardChanges(sourceFile, updatedFile);
        List<DistrictChange> districtChanges = DistrictChanges(sourceFile, updatedFile);

        return new DataChanges { WardChanges = wardChanges, DistrictChanges = districtChanges };
    }

    private static List<WardChange> WardChanges(
        IEnumerable<ExcelRegionModel> sourceFile,
        IEnumerable<ExcelRegionModel> updatedFile
    )
    {
        List<WardChange> wardChanges = [];
        List<string?> wardsInupdatedFile = updatedFile.Select(x => x.WardCode).ToList();

        IEnumerable<ExcelRegionModel> deletedWards = sourceFile.ExceptBy(
            wardsInupdatedFile,
            x => x.WardCode
        );
        IEnumerable<ExcelRegionModel> addedWards = updatedFile.ExceptBy(
            sourceFile.Select(x => x.WardCode),
            x => x.WardCode
        );
        IEnumerable<ExcelRegionModel> updateWards = sourceFile
            .Where(x => x.WardCode != null)
            .IntersectBy(wardsInupdatedFile, x => x.WardCode);
        var updates = GetWardUpdates(updateWards, updatedFile.Where(x => x.WardCode != null));

        wardChanges.AddRange(updates);

        foreach (ExcelRegionModel addedWard in addedWards)
        {
            wardChanges.Add(
                new WardChange
                {
                    Code = addedWard.WardCode,
                    Old = null,
                    New = RegionMapping.MapFromExcelModelToUpdateWard(addedWard),
                    Type = ChangeType.Addition,
                    Update = RegionMapping.MapFromExcelModelToUpdateWard(addedWard),
                }
            );
        }

        foreach (var deletedWard in deletedWards)
        {
            wardChanges.Add(
                new WardChange
                {
                    Code = deletedWard.WardCode,
                    New = null,
                    Old = RegionMapping.MapFromExcelModelToUpdateWard(deletedWard),
                    Type = ChangeType.Delete,
                }
            );
        }

        return wardChanges;
    }

    private static List<WardChange> GetWardUpdates(
        IEnumerable<ExcelRegionModel> oldList,
        IEnumerable<ExcelRegionModel> newList
    )
    {
        var oldRegionDict = oldList.ToDictionary(r => r.WardCode!, r => r);
        var wardChanges = new ConcurrentBag<WardChange>();

        Parallel.ForEach(
            newList,
            newRegion =>
            {
                string uniqueKey = newRegion.WardCode!;

                // Check if the region exists in the old list
                if (oldRegionDict.TryGetValue(uniqueKey, out var oldRegion))
                {
                    List<Change> changes = [];

                    // Compare properties that may change
                    if (oldRegion.Ward != newRegion.Ward)
                    {
                        changes.Add(
                            new()
                            {
                                Property = nameof(ExcelRegionModel.Ward),
                                Old = oldRegion.Ward,
                                Current = newRegion.Ward,
                            }
                        );
                    }

                    if (oldRegion.DistrictCode != newRegion.DistrictCode)
                    {
                        changes.Add(
                            new()
                            {
                                Property = nameof(ExcelRegionModel.DistrictCode),
                                Old = oldRegion.DistrictCode,
                                Current = newRegion.DistrictCode,
                            }
                        );
                    }

                    if (changes.Count > 0)
                    {
                        ExcelRegionModel update = oldRegion.Clone();
                        UpdateWard updateOld = RegionMapping.MapFromExcelModelToUpdateWard(
                            Update(changes, update)
                        );
                        wardChanges.Add(
                            new WardChange()
                            {
                                Code = uniqueKey,
                                Old = RegionMapping.MapFromExcelModelToUpdateWard(oldRegion),
                                New = RegionMapping.MapFromExcelModelToUpdateWard(newRegion),
                                Type = ChangeType.Update,
                                Changes = changes,
                                Update = updateOld!,
                            }
                        );
                    }
                }
            }
        );

        return [.. wardChanges];
    }

    private static List<DistrictChange> DistrictChanges(
        IEnumerable<ExcelRegionModel> sourceFile,
        IEnumerable<ExcelRegionModel> updatedFile
    )
    {
        List<DistrictChange> districtChanges = [];
        IEnumerable<string?> districtsInupdatedFile = updatedFile
            .DistinctBy(x => x.DistrictCode)
            .Select(x => x.DistrictCode)
            .ToList();

        IEnumerable<ExcelRegionModel> deletedDistricts = sourceFile
            .DistinctBy(x => x.DistrictCode)
            .ExceptBy(districtsInupdatedFile, x => x.DistrictCode);
        IEnumerable<ExcelRegionModel> addedDistricts = updatedFile
            .DistinctBy(x => x.DistrictCode)
            .ExceptBy(sourceFile.Select(x => x.DistrictCode), x => x.DistrictCode);
        List<ExcelRegionModel> districtsToUpdate = sourceFile
            .DistinctBy(x => x.DistrictCode)
            .IntersectBy(districtsInupdatedFile, x => x.DistrictCode)
            .ToList();

        var updates = GetDistrictUpdates(
            districtsToUpdate,
            updatedFile.DistinctBy(x => x.DistrictCode)
        );
        districtChanges.AddRange(updates);

        foreach (ExcelRegionModel addedDistrict in addedDistricts)
        {
            districtChanges.Add(
                new DistrictChange
                {
                    Code = addedDistrict.WardCode,
                    Old = null,
                    New = RegionMapping.MapFromExcelModelToUpdateDistrict(addedDistrict),
                    Type = ChangeType.Addition,
                    Update = RegionMapping.MapFromExcelModelToUpdateDistrict(addedDistrict),
                }
            );
        }

        foreach (var deletedDistrict in deletedDistricts)
        {
            districtChanges.Add(
                new DistrictChange
                {
                    Code = deletedDistrict.WardCode,
                    New = null,
                    Old = RegionMapping.MapFromExcelModelToUpdateDistrict(deletedDistrict),
                    Type = ChangeType.Delete,
                }
            );
        }

        return districtChanges;
    }

    private static List<DistrictChange> GetDistrictUpdates(
        IEnumerable<ExcelRegionModel> oldList,
        IEnumerable<ExcelRegionModel> newList
    )
    {
        var oldRegionDict = oldList.ToDictionary(r => r.DistrictCode!, r => r);
        var districtChanges = new ConcurrentBag<DistrictChange>();

        Parallel.ForEach(
            newList,
            newRegion =>
            {
                string uniqueKey = newRegion.DistrictCode!;

                // Check if the region exists in the old list
                if (oldRegionDict.TryGetValue(uniqueKey, out var oldRegion))
                {
                    List<Change> changes = [];

                    // Compare properties that may change
                    if (oldRegion.District != newRegion.District)
                    {
                        changes.Add(
                            new()
                            {
                                Property = nameof(ExcelRegionModel.District),
                                Old = oldRegion.District,
                                Current = newRegion.District,
                            }
                        );
                    }

                    if (oldRegion.ProvinceCode != newRegion.ProvinceCode)
                    {
                        changes.Add(
                            new()
                            {
                                Property = nameof(ExcelRegionModel.ProvinceCode),
                                Old = oldRegion.ProvinceCode,
                                Current = newRegion.ProvinceCode,
                            }
                        );
                    }

                    if (changes.Count > 0)
                    {
                        ExcelRegionModel update = oldRegion.Clone();
                        UpdateDistrict updateOld = RegionMapping.MapFromExcelModelToUpdateDistrict(
                            Update(changes, update)
                        );
                        districtChanges.Add(
                            new DistrictChange()
                            {
                                Code = uniqueKey,
                                Old = RegionMapping.MapFromExcelModelToUpdateDistrict(oldRegion),
                                New = RegionMapping.MapFromExcelModelToUpdateDistrict(newRegion),
                                Type = ChangeType.Update,
                                Changes = changes,
                                Update = updateOld!,
                            }
                        );
                    }
                }
            }
        );

        return [.. districtChanges];
    }

    private static ExcelRegionModel Update(List<Change> changes, ExcelRegionModel update)
    {
        for (int i = 0; i < changes.Count; i++)
        {
            Change change = changes[i];
            PropertyInfo? property = update.GetType().GetProperty(change.Property!);

            if (property == null)
            {
                continue;
            }

            property.SetValue(update, change.Current, null);
        }
        return update;
    }
}
