using System.Text;
using CaseConverter;
using Microsoft.Extensions.Options;
using VnRegion.Regions.Settings;

namespace VnRegion.Regions.BackgroundJobs;

public class DatabaseStructureGeneration(IOptions<NameConfigurationSettings> options)
    : IHostedLifecycleService
{
    private readonly NameConfigurationSettings configurationSettings = options.Value;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        string rootPath = Directory.GetCurrentDirectory();
        string databasePath = Path.Join(rootPath, "Database", "create_table.sql");

        if (!File.Exists(databasePath))
        {
            Console.WriteLine("Generating database structure...");
            string sqlScript = GenerateSQL(configurationSettings);
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

    static string GenerateSQL(NameConfigurationSettings config)
    {
        var sb = new StringBuilder();
        sb.AppendLine("-- Auto-generated SQL script");
        sb.AppendLine("-- Database: " + config.DbSetting);

        string administrativeUnitFk =
            $"{config.AdministrativeUnitConfigs!.TableName}_id".ToSnakeCase();
        string provinceFk = $"{config.ProvinceConfigs!.TableName}_id".ToSnakeCase();
        string districtFk = $"{config.DistrictConfigs!.TableName}_id".ToSnakeCase();
        AppendCreateTableSQL(sb, config.AdministrativeUnitConfigs!, true);
        AppendCreateTableSQL(
            sb,
            config.ProvinceConfigs!,
            false,
            administrativeUnitFk,
            config.AdministrativeUnitConfigs!.TableName!.ToSnakeCase()
        );
        AppendCreateTableSQL(
            sb,
            config.DistrictConfigs!,
            false,
            administrativeUnitFk,
            config.AdministrativeUnitConfigs.TableName!.ToSnakeCase(),
            provinceFk,
            config.ProvinceConfigs!.TableName!.ToSnakeCase()
        );
        AppendCreateTableSQL(
            sb,
            config.WardConfigs!,
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
        Name tableConfig,
        bool isAdministrativeUnit,
        params string[] foreignKeys
    )
    {
        sb.AppendLine($"CREATE TABLE {tableConfig.TableName.ToSnakeCase()} (");
        foreach (var col in tableConfig.ColumnNames)
        {
            if (col.Key == "Id")
            {
                if (isAdministrativeUnit)
                {
                    sb.AppendLine($"    {col.Value.ToSnakeCase()} INT NOT NULL PRIMARY KEY,");
                }
                else
                {
                    sb.AppendLine($"    {col.Value.ToSnakeCase()} CHAR(26) NOT NULL PRIMARY KEY,");
                }
            }
            else if (col.Key == "AdministrativeUnitId")
            {
                sb.AppendLine($"    {col.Value.ToSnakeCase()} INT NOT NULL,");
            }
            else if (col.Key.EndsWith("Id"))
            {
                sb.AppendLine($"    {col.Value.ToSnakeCase()} CHAR(26),");
            }
            else
            {
                sb.AppendLine($"    {col.Value.ToSnakeCase()} VARCHAR(255),");
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
}
