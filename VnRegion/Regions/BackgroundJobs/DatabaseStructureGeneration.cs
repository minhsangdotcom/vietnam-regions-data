using System.Text;
using CaseConverter;
using Microsoft.Extensions.Options;
using VnRegion.Regions.Entities;
using VnRegion.Regions.Settings;

namespace VnRegion.Regions.BackgroundJobs;

public class DatabaseStructureGeneration : IHostedLifecycleService
{
    private readonly DatabaseConfigurationSettings configurationSettings;

    public DatabaseStructureGeneration(IOptions<DatabaseConfigurationSettings> options)
    {
        configurationSettings = options.Value;
        configurationSettings ??= new();
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        string rootPath = Directory.GetCurrentDirectory();
        string databasePath = Path.Join(rootPath, "Database", "create_table.sql");

        if (!File.Exists(databasePath))
        {
            Console.WriteLine("Generating database structure...");
            string sqlScript = GenerateSQL(PrepareSettings(configurationSettings));
            await File.WriteAllTextAsync(databasePath, sqlScript, cancellationToken);
            Console.WriteLine("Generating has finished.");
        }
    }

    public async Task StartedAsync(CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }

    public async Task StartingAsync(CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }

    public async Task StoppedAsync(CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }

    public async Task StoppingAsync(CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }

    static string GenerateSQL(DatabaseConfigurationSettings config)
    {
        var sb = new StringBuilder();
        sb.AppendLine("-- Auto-generated SQL script");
        sb.AppendLine("-- Database: " + config.DbSetting);
        sb.AppendLine("CREATE DATABASE vietnamese_region_data;");
        sb.AppendLine("USE vietnamese_region_data;\n");

        string administrativeUnitFk =
            $"{config.AdministrativeUnitConfigs!.TableName}_id".ToSnakeCase();
        string provinceFk = $"{config.ProvinceConfigs!.TableName}_id".ToSnakeCase();
        string districtFk = $"{config.DistrictConfigs!.TableName}_id".ToSnakeCase();
        AppendCreateTableSQL(sb, config.AdministrativeUnitConfigs!, config.DbSetting!, true);
        AppendCreateTableSQL(
            sb,
            config.ProvinceConfigs!,
            config.DbSetting!,
            false,
            administrativeUnitFk,
            config.AdministrativeUnitConfigs!.TableName!.ToSnakeCase()
        );
        AppendCreateTableSQL(
            sb,
            config.DistrictConfigs!,
            config.DbSetting!,
            false,
            administrativeUnitFk,
            config.AdministrativeUnitConfigs.TableName!.ToSnakeCase(),
            provinceFk,
            config.ProvinceConfigs!.TableName!.ToSnakeCase()
        );
        AppendCreateTableSQL(
            sb,
            config.WardConfigs!,
            config.DbSetting!,
            false,
            administrativeUnitFk,
            config.AdministrativeUnitConfigs!.TableName!.ToSnakeCase(),
            districtFk,
            config.DistrictConfigs!.TableName!.ToSnakeCase()
        );

        return sb.ToString();
    }

    static void AppendCreateTableSQL(
        StringBuilder sb,
        TableConfiguration tableConfig,
        string dbSetting,
        bool isAdministrativeUnit,
        params string[] foreignKeys
    )
    {
        sb.AppendLine($"CREATE TABLE {tableConfig.TableName.ToSnakeCase()} (");
        foreach (var col in tableConfig.ColumnNames)
        {
            if (col.Key == nameof(Region.CreatedAt))
            {
                continue;
            }

            switch (true)
            {
                case bool when col.Key == nameof(Region.Id):
                {
                    if (isAdministrativeUnit)
                    {
                        sb.AppendLine($"    {col.Value.ToSnakeCase()} INT NOT NULL PRIMARY KEY,");
                    }
                    else
                    {
                        sb.AppendLine(
                            $"    {col.Value.ToSnakeCase()} CHAR(26) NOT NULL PRIMARY KEY,"
                        );
                    }
                    break;
                }
                case bool when col.Key == nameof(Region.AdministrativeUnitId):
                {
                    sb.AppendLine($"    {col.Value.ToSnakeCase()} INT NOT NULL,");
                    break;
                }
                case bool when col.Key.EndsWith("Id"):
                {
                    sb.AppendLine($"    {col.Value.ToSnakeCase()} CHAR(26),");
                    break;
                }
                default:
                    sb.AppendLine(CreateColumn(col.Key, col.Value, dbSetting));
                    break;
            }
        }

        if (foreignKeys.Length > 0)
        {
            for (int i = 0; i < foreignKeys.Length; i += 2)
            {
                sb.AppendLine(
                    $"    FOREIGN KEY ({foreignKeys[i]}) REFERENCES {foreignKeys[i + 1]}(id),"
                );
            }
        }

        sb.AppendLine(");\n");
    }

    private static string CreateColumn(string property, string value, string dbSetting)
    {
        string dataType = string.Empty;
        if (
            (
                property == nameof(Region.Name)
                || property == nameof(Region.FullName)
                || property == nameof(Region.CustomName)
            ) && (dbSetting == DbSettingName.OracleSql || dbSetting == DbSettingName.SqlServer)
        )
        {
            dataType += "N";
        }
        return $"    {value.ToSnakeCase()} {dataType}VARCHAR(255),";
    }

    private DatabaseConfigurationSettings PrepareSettings(
        DatabaseConfigurationSettings configurationSettings
    )
    {
        if (
            configurationSettings.AdministrativeUnitConfigs == null
            || configurationSettings.AdministrativeUnitConfigs is { }
        )
        {
            configurationSettings.AdministrativeUnitConfigs = new()
            {
                TableName = nameof(AdministrativeUnit),
                ColumnNames = typeof(AdministrativeUnit)
                    .GetProperties()
                    .ToDictionary(property => property.Name, property => property.Name),
            };
        }

        if (
            configurationSettings.ProvinceConfigs == null
            || configurationSettings.ProvinceConfigs is { }
        )
        {
            configurationSettings.ProvinceConfigs = new()
            {
                TableName = nameof(Province),
                ColumnNames = typeof(Province)
                    .GetProperties()
                    .ToDictionary(property => property.Name, property => property.Name),
            };
        }

        if (
            configurationSettings.DistrictConfigs == null
            || configurationSettings.DistrictConfigs is { }
        )
        {
            configurationSettings.DistrictConfigs = new()
            {
                TableName = nameof(District),
                ColumnNames = typeof(District)
                    .GetProperties()
                    .ToDictionary(property => property.Name, property => property.Name),
            };
        }

        if (configurationSettings.WardConfigs == null || configurationSettings.WardConfigs is { })
        {
            configurationSettings.WardConfigs = new()
            {
                TableName = nameof(Ward),
                ColumnNames = typeof(Ward)
                    .GetProperties()
                    .ToDictionary(property => property.Name, property => property.Name),
            };
        }

        return configurationSettings;
    }
}
