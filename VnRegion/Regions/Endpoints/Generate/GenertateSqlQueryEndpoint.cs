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
        app.MapPost("api/regions/generateSqlQuery", Handle)
            .AddEndpointFilter<ValidateGenerateSqlQueryFileRequest>()
            .ProducesValidationProblem()
            .DisableAntiforgery();
    }

    public class GenerateSqlQueryFileRequest
    {
        public IFormFile? File { get; set; }
    }

    public class ValidateGenerateSqlQueryFileRequest : IEndpointFilter
    {
        public async ValueTask<object?> InvokeAsync(
            EndpointFilterInvocationContext context,
            EndpointFilterDelegate next
        )
        {
            var request = context.Arguments.OfType<GenerateSqlQueryFileRequest>().FirstOrDefault();
            if (request != null)
            {
                return Results.Problem("File is required");
            }

            if (
                request!.File!.ContentType
                != "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
            )
            {
                return Results.Problem("File must be xlsx type");
            }

            return await next(context);
        }
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
