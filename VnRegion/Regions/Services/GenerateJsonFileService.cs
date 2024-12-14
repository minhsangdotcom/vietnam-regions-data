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

        DataChanges? dataChanges = null;
        if (updatedFile != null)
        {
            using MemoryStream updatedFileStream = new();
            await updatedFile!.CopyToAsync(updatedFileStream);

            IEnumerable<ExcelRegionModel> updatedFileRows = MiniExcel.Query<ExcelRegionModel>(
                updatedFileStream
            );
            dataChanges = HasChangeData(sourceFileRows, updatedFileRows);
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

        dataChanges?.WardChanges?.ForEach(x =>
        {
            UpdateWard? updateWard = x.Update;

            if (updateWard != null)
            {
                updateWard.DistrictId =
                    districts.Find(p => p.Code == updateWard.DistrictCode)?.Id ?? Ulid.NewUlid();
            }
        });

        dataChanges?.DistrictChanges?.ForEach(x =>
        {
            UpdateDistrict? updateDistrict = x.Update;
            if (updateDistrict != null)
            {
                updateDistrict.ProvinceId = provines
                    .Find(p => p.Code == updateDistrict.ProvinceCode)
                    ?.Id;
            }
        });

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

        string provincePath = Path.Combine(path, "Provinces.json");
        string districtPath = Path.Combine(path, "Districts.json");
        string wardPath = Path.Combine(path, "Wards.json");
        string changesPath =
            dataChanges == null ? string.Empty : Path.Combine(path, "Changes.json");

        File.WriteAllText(provincePath, strProvinces);
        File.WriteAllText(districtPath, strDistricts);
        File.WriteAllText(wardPath, strWards);

        GenerateResponse response =
            new()
            {
                ProvinceMetaData = new() { Path = provincePath, Total = provines.Count },
                DistrictMetaData = new() { Path = districtPath, Total = districts.Count },
                WardMetaData = new() { Path = wardPath, Total = wards.Count },
            };

        if (!string.IsNullOrWhiteSpace(changesPath))
        {
            File.WriteAllText(changesPath, SerializerExtension.Serialize(dataChanges!).StringJson);
            response.UpdateMetaData = new()
            {
                Path = changesPath,
                WardSummaryUpdate = new()
                {
                    AddedTotal =
                        dataChanges?.WardChanges?.Count(x => x.Type == ChangeType.Addition) ?? 0,
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
                        dataChanges?.DistrictChanges?.Count(x => x.Type == ChangeType.Update) ?? 0,
                    RemoveTotal =
                        dataChanges?.DistrictChanges?.Count(x => x.Type == ChangeType.Delete) ?? 0,
                },
            };
        }

        return response;
    }

    private string SerializeCustomName(
        string tableName,
        List<Province>? provinces = null,
        List<District>? districts = null,
        List<Ward>? wards = null
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
                    New = RegionMapping.MapFromExcelModelToWard(addedWard),
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
                    Old = RegionMapping.MapFromExcelModelToWard(deletedWard),
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
                                Old = RegionMapping.MapFromExcelModelToWard(oldRegion),
                                New = RegionMapping.MapFromExcelModelToWard(newRegion),
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
                    New = RegionMapping.MapFromExcelModelToDistrict(addedDistrict),
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
                    Old = RegionMapping.MapFromExcelModelToDistrict(deletedDistrict),
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
                                Old = RegionMapping.MapFromExcelModelToDistrict(oldRegion),
                                New = RegionMapping.MapFromExcelModelToDistrict(newRegion),
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
