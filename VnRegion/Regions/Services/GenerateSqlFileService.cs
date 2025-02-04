using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using CaseConverter;
using Microsoft.Extensions.Options;
using MiniExcelLibs;
using VnRegion.Regions.Constants;
using VnRegion.Regions.Endpoints.Generate;
using VnRegion.Regions.Entities;
using VnRegion.Regions.Mapping;
using VnRegion.Regions.Models;
using VnRegion.Regions.Settings;

namespace VnRegion.Regions.Services;

public class GenerateSqlFileService : IGenerate
{
    private readonly NameConfigurationSettings nameConfigurations;

    public GenerateSqlFileService(IOptions<NameConfigurationSettings> options)
    {
        nameConfigurations = options.Value;
        nameConfigurations ??= new();
    }

    public async Task<GenerateResponse> GenerateAsync(
        IFormFile sourceFile,
        IFormFile? updatedFile,
        string? output = null
    )
    {
        Stopwatch stopwatch = new();
        stopwatch.Start();
        Console.WriteLine("Starting SQL file export......");
        using MemoryStream stream = new();
        await sourceFile!.CopyToAsync(stream);

        IEnumerable<ExcelRegionModel> sourceFileRows = MiniExcel.Query<ExcelRegionModel>(stream);
        string path = output ?? RegionName.OutputPath;
        Directory.CreateDirectory(path);

        (string provinceSql, List<Province> provinces) = ProvinceSqlGeneration(sourceFileRows);
        (string districtSql, List<District> districts) = DistrictSqlGeneration(
            sourceFileRows,
            provinces
        );
        string wardSql = WardSqlGeneration(sourceFileRows, districts);
        string unitInsert = UnitSqlGenertation();
        string result =
            unitInsert + "\n\n" + provinceSql + "\n \n" + districtSql + "\n\n" + wardSql;

        string fullPath = Path.Combine(path, "sql_query.sql");
        File.WriteAllText(fullPath, result);

        stopwatch.Stop();
        await Console.Out.WriteLineAsync(
            "Exporting has finished in " + stopwatch.ElapsedMilliseconds / 1000 + "s"
        );

        return new() { SqlGenerationPath = fullPath };
    }

    private SqlGenerationResponse<Province> ProvinceSqlGeneration(
        IEnumerable<ExcelRegionModel> sourceFileRows
    )
    {
        List<Province> provines = RegionMapping.MapFromIEnumrableExcelModelToListProvince(
            sourceFileRows.Where(x => x.ProvinceCode != null).DistinctBy(x => x.ProvinceCode)
        );

        string provinceTableName =
            nameConfigurations.ProvinceConfigs == null
                ? nameof(Province)
                : nameConfigurations.ProvinceConfigs!.TableName!;
        string provinceColumNames =
            nameConfigurations.ProvinceConfigs == null
                ? GenerationHelper.Get(
                    dbsetting: nameConfigurations.DbSetting!,
                    new Province(),
                    isUnderscoreLower: true
                )
                : GenerationHelper.Get(
                    columnNames: nameConfigurations.ProvinceConfigs!.ColumnNames,
                    dbsetting: nameConfigurations.DbSetting!,
                    isUnderscoreLower: true
                );

        IEnumerable<PropertyInfo?> propertyInfos =
            nameConfigurations.ProvinceConfigs == null
                ? typeof(Province).GetProperties()
                : nameConfigurations.ProvinceConfigs.ColumnNames.Keys.Select(key =>
                    typeof(Province).GetProperty(key)
                );

        string provinceSql =
            @$"INSERT INTO {provinceTableName.ToSnakeCase()} " + provinceColumNames + " VALUES\n";
        foreach (Province province in provines)
        {
            provinceSql += "\t" + $"(";
            foreach (PropertyInfo? propertyInfo in propertyInfos)
            {
                if (propertyInfo == null)
                {
                    continue;
                }
                provinceSql += GenerateValueSqlQuery(
                    nameConfigurations.DbSetting!,
                    propertyInfo,
                    province
                );
            }
            provinceSql = provinceSql[..^1];
            provinceSql += ")," + "\n";
        }

        return new(provinceSql[..^2] + ";", provines);
    }

    private SqlGenerationResponse<District> DistrictSqlGeneration(
        IEnumerable<ExcelRegionModel> sourceFileRows,
        List<Province> provinces
    )
    {
        string districtTableName =
            nameConfigurations.DistrictConfigs == null
                ? nameof(District)
                : nameConfigurations.DistrictConfigs!.TableName!;
        string districtColumNames =
            nameConfigurations.DistrictConfigs == null
                ? GenerationHelper.Get(
                    dbsetting: nameConfigurations.DbSetting!,
                    new District(),
                    isUnderscoreLower: true
                )
                : GenerationHelper.Get(
                    columnNames: nameConfigurations.DistrictConfigs!.ColumnNames,
                    dbsetting: nameConfigurations.DbSetting!,
                    isUnderscoreLower: true
                );

        List<District> districts = RegionMapping.MapFromIEnumrableExcelModelToListDistrict(
            sourceFileRows
                .Where(x => x.DistrictCode != null)
                .DistinctBy(x => new { x.DistrictCode, x.ProvinceCode }),
            provinces
        );

        IEnumerable<PropertyInfo?> propertyInfos =
            nameConfigurations.DistrictConfigs == null
                ? typeof(District).GetProperties()
                : nameConfigurations.DistrictConfigs.ColumnNames.Keys.Select(key =>
                    typeof(District).GetProperty(key)
                );

        string districtSql =
            @$"INSERT INTO {districtTableName.ToSnakeCase()} " + districtColumNames + " VALUES\n";
        foreach (District district in districts)
        {
            districtSql += "\t" + $"(";
            foreach (PropertyInfo? propertyInfo in propertyInfos)
            {
                if (propertyInfo == null)
                {
                    continue;
                }
                districtSql += GenerateValueSqlQuery(
                    nameConfigurations.DbSetting!,
                    propertyInfo,
                    district
                );
            }

            districtSql = districtSql[..^1];
            districtSql += ")," + "\n";
        }

        return new(districtSql[..^2] + ";", districts);
    }

    private string WardSqlGeneration(
        IEnumerable<ExcelRegionModel> currentFileRows,
        IEnumerable<District> districts
    )
    {
        List<Ward> wards = RegionMapping.MapFromIEnumrableExcelModelToListWard(
            currentFileRows
                .Where(x => x.WardCode != null)
                .DistinctBy(x => new { x.WardCode, x.DistrictCode }),
            districts
        );

        string wardTableName =
            nameConfigurations.WardConfigs == null
                ? nameof(Ward)
                : nameConfigurations.WardConfigs!.TableName!;
        string wardColumNames =
            nameConfigurations.WardConfigs == null
                ? GenerationHelper.Get(
                    dbsetting: nameConfigurations.DbSetting!,
                    new Ward(),
                    isUnderscoreLower: true
                )
                : GenerationHelper.Get(
                    columnNames: nameConfigurations.WardConfigs!.ColumnNames,
                    dbsetting: nameConfigurations.DbSetting!,
                    isUnderscoreLower: true
                );
        var propertyAccessors =
            nameConfigurations.WardConfigs == null
                ? typeof(Ward)
                    .GetProperties()
                    .Select(p => new KeyValuePair<string, Func<Ward, object?>>(
                        p.Name,
                        CreatePropertyAccessor<Ward>(p)
                    ))
                :
                [
                    .. nameConfigurations.WardConfigs.ColumnNames.Keys.Select(
                        key => new KeyValuePair<string, Func<Ward, object?>>(
                            key,
                            CreatePropertyAccessor<Ward>(typeof(Ward).GetProperty(key)!)
                        )
                    ),
                ];

        const int batchSize = 1000;
        var sqlBuilder = new StringBuilder();
        for (int i = 0; i < wards.Count; i += batchSize)
        {
            IEnumerable<Ward> batch = wards.Skip(i).Take(batchSize);

            // Start a new INSERT statement
            sqlBuilder.Append(
                $"INSERT INTO {wardTableName.ToSnakeCase()} {wardColumNames} VALUES\n"
            );

            foreach (Ward ward in batch)
            {
                sqlBuilder.Append("\t(");
                foreach (var accessor in propertyAccessors)
                {
                    string propertyName = accessor.Key;
                    string? value = accessor.Value(ward)?.ToString();

                    string sqlValue = string.Empty;
                    if (
                        (
                            propertyName == nameof(Region.FullName)
                            || propertyName == nameof(Region.Name)
                            || (
                                propertyName == nameof(Region.CustomName)
                                && !string.IsNullOrWhiteSpace(value)
                            )
                        )
                        && (
                            nameConfigurations.DbSetting == DbSettingName.SqlServer
                            || nameConfigurations.DbSetting == DbSettingName.OracleSql
                        )
                    )
                    {
                        sqlValue += "N";
                    }
                    sqlBuilder.Append(sqlValue + $"'{value}',");
                }
                sqlBuilder.Length--; // Remove trailing comma
                sqlBuilder.Append("),\n");
            }

            sqlBuilder.Length -= 2; // Remove trailing ",\n"
            sqlBuilder.Append(";\n"); // End batch
        }

        return sqlBuilder.ToString();
    }

    private static Func<T, object?> CreatePropertyAccessor<T>(PropertyInfo propertyInfo)
    {
        var parameter = Expression.Parameter(typeof(T), "x");
        var propertyAccess = Expression.Property(parameter, propertyInfo);
        var convert = Expression.Convert(propertyAccess, typeof(object)); // Ensure conversion to object
        return Expression.Lambda<Func<T, object?>>(convert, parameter).Compile();
    }

    private string UnitSqlGenertation()
    {
        string tableName =
            nameConfigurations.AdministrativeUnitConfigs == null
                ? nameof(AdministrativeUnit)
                : nameConfigurations.AdministrativeUnitConfigs!.TableName!;

        return $"INSERT INTO {tableName.ToSnakeCase()} ({nameof(AdministrativeUnit.FullName).ToSnakeCase()},{nameof(AdministrativeUnit.EnglishFullName).ToSnakeCase()},{nameof(AdministrativeUnit.ShortName).ToSnakeCase()},{nameof(AdministrativeUnit.EnglishShortName).ToSnakeCase()},{nameof(AdministrativeUnit.Id).ToSnakeCase()}) VALUES\r\n\t"
            + $"('Thành phố trực thuộc trung ương','Municipality','Thành phố','City',1),\r\n\t"
            + $"('Tỉnh','Province','Tỉnh','Province',2),\r\n\t"
            + $"('Thành phố thuộc thành phố trực thuộc trung ương','Municipal city','Thành phố','City',3),\r\n\t"
            + $"('Thành phố thuộc tỉnh','Provincial city','Thành phố','City',4),\r\n\t"
            + $"('Quận','Urban district','Quận','District',5),\r\n\t"
            + $"('Thị xã','District-level town','Thị xã','Town',6),\r\n\t"
            + $"('Huyện','District','Huyện','District',7),\r\n\t"
            + $"('Phường','Ward','Phường','Ward',8),\r\n\t"
            + $"('Thị trấn','Commune-level town','Thị trấn','Township',9),\r\n\t"
            + $"('Xã','Commune','Xã','Commune',10);";
    }

    private static string GenerateValueSqlQuery(
        string databaseType,
        PropertyInfo propertyInfo,
        object obj
    )
    {
        string sql = string.Empty;
        string? value = propertyInfo?.GetValue(obj, null)?.ToString();
        if (
            (
                propertyInfo?.Name == nameof(Region.Name)
                || propertyInfo?.Name == nameof(Region.FullName)
                || (
                    propertyInfo?.Name == nameof(Region.CustomName)
                    && !string.IsNullOrWhiteSpace(value)
                )
            )
            && (databaseType == DbSettingName.SqlServer || databaseType == DbSettingName.OracleSql)
        )
        {
            sql += "N";
        }
        return sql + $"'{value}',";
    }
}

public record SqlGenerationResponse<T>(string Sql, List<T> Data);
