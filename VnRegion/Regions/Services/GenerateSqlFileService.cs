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

public class GenerateSqlFileService(IOptions<NameConfigurationSettings> options) : IGenerate
{
    private readonly NameConfigurationSettings nameConfigurations = options.Value;

    public async Task<GenerateResponse> GenerateAsync(
        IFormFile sourceFile,
        IFormFile? updatedFile,
        string? output = null
    )
    {
        Stopwatch stopwatch = new();
        stopwatch.Start();
        Console.WriteLine("Starting generate......");
        using MemoryStream stream = new();
        await sourceFile!.CopyToAsync(stream);

        IEnumerable<ExcelRegionModel> sourceFileRows = MiniExcel.Query<ExcelRegionModel>(stream);
        string path = output ?? RegionName.OutputPath;
        Directory.CreateDirectory(path);

        string provinceSql = ProvinceSqlGeneration(sourceFileRows);
        string districtSql = DistrictSqlGeneration(sourceFileRows);
        string wardSql = WardSqlGeneration(sourceFileRows);
        string unitInsert = UnitSqlGenertation();
        string result =
            unitInsert + "\n\n" + provinceSql + "\n \n" + districtSql + "\n\n" + wardSql;

        string fullPath = Path.Combine(path, "sql_query.txt");
        File.WriteAllText(fullPath, result);

        stopwatch.Stop();
        await Console.Out.WriteLineAsync(
            "Generated in " + stopwatch.ElapsedMilliseconds / 1000 + "s"
        );

        return new() { SqlGenerationPath = fullPath };
    }

    private string ProvinceSqlGeneration(IEnumerable<ExcelRegionModel> sourceFileRows)
    {
        List<Province> provines = RegionMapping.MapFromIEnumrableExcelModelToListProvince(
            sourceFileRows.Where(x => x.ProvinceCode != null).DistinctBy(x => x.ProvinceCode)
        );

        string provinceTableName =
            nameConfigurations.ProvinceConfigs == null
                ? nameof(Province).ToSnakeCase()
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
            @$"INSERT INTO {provinceTableName} " + provinceColumNames + " VALUES\n";
        foreach (Province province in provines)
        {
            provinceSql += "\t" + $"(";
            foreach (PropertyInfo? propertyInfo in propertyInfos)
            {
                provinceSql += $"'{propertyInfo?.GetValue(province, null)?.ToString()}',";
            }
            provinceSql = provinceSql.Remove(provinceSql.Length - 1, 1);
            provinceSql += ")," + "\n";
        }

        return provinceSql.Remove(provinceSql.Length - 2, 2) + ";";
    }

    private string DistrictSqlGeneration(IEnumerable<ExcelRegionModel> sourceFileRows)
    {
        string districtTableName =
            nameConfigurations.DistrictConfigs == null
                ? nameof(District).ToSnakeCase()
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
                .DistinctBy(x => new { x.DistrictCode, x.ProvinceCode })
        );

        IEnumerable<PropertyInfo?> propertyInfos =
            nameConfigurations.DistrictConfigs == null
                ? typeof(District).GetProperties()
                : nameConfigurations.DistrictConfigs.ColumnNames.Keys.Select(key =>
                    typeof(District).GetProperty(key)
                );

        string districtSql =
            @$"INSERT INTO {districtTableName} " + districtColumNames + " VALUES\n";
        foreach (District district in districts)
        {
            districtSql += "\t" + $"(";
            foreach (PropertyInfo? propertyInfo in propertyInfos)
            {
                districtSql += $"'{propertyInfo?.GetValue(district, null)?.ToString()}',";
            }

            districtSql = districtSql.Remove(districtSql.Length - 1, 1);
            districtSql += ")," + "\n";
        }

        return districtSql.Remove(districtSql.Length - 2, 2) + ";";
    }

    private string WardSqlGeneration(IEnumerable<ExcelRegionModel> currentFileRows)
    {
        List<Ward> wards = RegionMapping.MapFromIEnumrableExcelModelToListWard(
            currentFileRows
                .Where(x => x.WardCode != null)
                .DistinctBy(x => new { x.WardCode, x.DistrictCode })
        );

        string wardTableName =
            nameConfigurations.WardConfigs == null
                ? nameof(Ward).ToSnakeCase()
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
                ? typeof(Ward).GetProperties().Select(p => CreatePropertyAccessor<Ward>(p))
                : nameConfigurations
                    .WardConfigs.ColumnNames.Keys.Select(key =>
                        CreatePropertyAccessor<Ward>(typeof(Ward).GetProperty(key)!)
                    )
                    .ToList();

        const int batchSize = 1000;
        var sqlBuilder = new StringBuilder();
        for (int i = 0; i < wards.Count; i += batchSize)
        {
            var batch = wards.Skip(i).Take(batchSize);

            // Start a new INSERT statement
            sqlBuilder.Append($"INSERT INTO {wardTableName} {wardColumNames} VALUES\n");

            foreach (Ward ward in batch)
            {
                sqlBuilder.Append("\t(");
                foreach (var accessor in propertyAccessors)
                {
                    var value = accessor(ward)?.ToString()?.Replace("'", "''"); // Escape single quotes
                    sqlBuilder.Append($"'{value}',");
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
                ? nameof(AdministrativeUnit).ToSnakeCase()
                : nameConfigurations.AdministrativeUnitConfigs!.TableName!;

        return $"INSERT INTO {tableName} (\"{nameof(AdministrativeUnit.FullName).ToSnakeCase()}\",{nameof(AdministrativeUnit.EnglishFullName).ToSnakeCase()}\",\"{nameof(AdministrativeUnit.ShortName).ToSnakeCase()}\",\"{nameof(AdministrativeUnit.EnglishShortName).ToSnakeCase()}\",\"{nameof(AdministrativeUnit.Id).ToSnakeCase()}\", \"{nameof(AdministrativeUnit.CreatedAt).ToSnakeCase()}\") VALUES\r\n\t"
            + $"('Thành phố trực thuộc trung ương','Municipality','Thành phố','City',1,null),\r\n\t"
            + $"('Tỉnh','Province','Tỉnh','Province',2,null),\r\n\t"
            + $"('Thành phố thuộc thành phố trực thuộc trung ương','Municipal city','Thành phố','City',3,null),\r\n\t"
            + $"('Thành phố thuộc tỉnh','Provincial city','Thành phố','City',4,null),\r\n\t"
            + $"('Quận','Urban district','Quận','District',5,null),\r\n\t"
            + $"('Thị xã','District-level town','Thị xã','Town',6,null),\r\n\t"
            + $"('Huyện','District','Huyện','District',7,null),\r\n\t"
            + $"('Phường','Ward','Phường','Ward',8,null),\r\n\t"
            + $"('Thị trấn','Commune-level town','Thị trấn','Township',9,null),\r\n\t"
            + $"('Xã','Commune','Xã','Commune',10,null);";
    }
}
