using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using VnRegion.Common;
using VnRegion.Regions.Constants;
using VnRegion.Regions.Services;

namespace VnRegion.Regions.Endpoints.Generate;

public class GenerateJsonEndpoint : IEndpoint
{
    public string GroupTag => "regions";

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/regions/generateJson", Handle).DisableAntiforgery();
    }

    public class GenerateJsonFileRequest
    {
        public IFormFile? File { get; set; }
        public IFormFile? UpdatedFile { get; set; }
    }

    private static async Task<Ok<GenerateResponse>> Handle(
        [FromForm] GenerateJsonFileRequest request,
        [FromServices] IGenerator generator
    )
    {
        return TypedResults.Ok(
            await generator
                .GetGenerationService(GenerationType.Json)
                .GenerateAsync(request.File!, request.UpdatedFile)
        );
    }
}
