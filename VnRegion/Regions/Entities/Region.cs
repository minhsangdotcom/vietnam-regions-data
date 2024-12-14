using System.Text.Json.Serialization;
using VnRegion.Common;

namespace VnRegion.Regions.Entities;

public class Region : BaseEntity<Ulid>
{
    public override Ulid Id { get; protected set; } = Ulid.NewUlid();

    [JsonPropertyName("code")]
    public string? Code { get; set; }

    public string? Name { get; set; }

    public string? EnglishName { get; set; }

    public string? FullName { get; set; }

    public string? EnglishFullName { get; set; }

    public string? CustomName { get; set; }

    public int AdministrativeUnitId { get; set; }
}
