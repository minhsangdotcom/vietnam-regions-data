using System.ComponentModel.DataAnnotations;

namespace VnRegion.Regions.Settings;

public class DatabaseConfigurationSettings
{
    public TableConfiguration? ProvinceConfigs { get; set; }

    public TableConfiguration? DistrictConfigs { get; set; }

    public TableConfiguration? WardConfigs { get; set; }

    public TableConfiguration? AdministrativeUnitConfigs { get; set; }

    [Required]
    public string? DbSetting { get; set; }
}

public class TableConfiguration
{
    public string? TableName { get; set; }

    public Dictionary<string, string> ColumnNames { get; set; } = [];
}

public static class DbSettingName
{
    public const string Mysql = nameof(Mysql);

    public const string PostgreSql = nameof(PostgreSql);

    public const string SqlServer = nameof(SqlServer);

    public const string OracleSql = nameof(OracleSql);
}
