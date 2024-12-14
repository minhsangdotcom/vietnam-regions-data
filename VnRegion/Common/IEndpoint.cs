namespace VnRegion.Common;

public interface IEndpoint
{
    string GroupTag { get; }
    void MapEndpoint(IEndpointRouteBuilder app);
}
