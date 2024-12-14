using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using VnRegion.Common;
using VnRegion.Regions.Constants;
using VnRegion.Regions.Services;

namespace VnRegion.Regions.Endpoints.Generate;

public class GenertateSqlQueryEndpoint : IEndpoint
{
    public string GroupTag => "regions";

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/regions/generateSqlQuery", Handle).DisableAntiforgery();
    }

    public class GenerateSqlQueryFileRequest
    {
        public IFormFile? File { get; set; }
    }

    private static async Task<Ok<GenerateResponse>> Handle(
        [FromForm] GenerateSqlQueryFileRequest request,
        [FromServices] IGenerator generator
    )
    {
        return TypedResults.Ok(
            await generator
                .GetGenerationService(GenerationType.Sql)
                .GenerateAsync(request.File!, null)
        );
    }
}
