namespace VnRegion.Regions.Entities;

public class AdministrativeUnit
{
    public int Id { get; set; }

    public string? FullName { get; set; }

    public string? EnglishFullName { get; set; }

    public string? ShortName { get; set; }

    public string? EnglishShortName { get; set; }

    public DateTimeOffset? CreatedAt { get; set; }
}
