namespace VnRegion.Regions.Settings;

public class NameConfigurationSettings
{
    public Name? ProvinceConfigs { get; set; }

    public Name? DistrictConfigs { get; set; }

    public Name? WardConfigs { get; set; }

    public string? DbSetting { get; set; }
}

public class Name
{
    public string? TableName { get; set; }

    public Dictionary<string, string> ColumnNames { get; set; } = [];
}

public static class DbSettingName
{
    public const string Mysql = nameof(Mysql);
    public const string PostgreSql = nameof(PostgreSql);
}
