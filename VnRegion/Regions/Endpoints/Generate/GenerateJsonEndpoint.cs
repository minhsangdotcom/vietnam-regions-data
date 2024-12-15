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
        app.MapPost("api/regions/generateJson", Handle)
            .AddEndpointFilter<ValidateGenerateJsonFileRequest>()
            .ProducesValidationProblem()
            .DisableAntiforgery();
    }

    public class GenerateJsonFileRequest
    {
        public IFormFile? File { get; set; }
        public IFormFile? UpdatedFile { get; set; }
    }

    public class ValidateGenerateJsonFileRequest : IEndpointFilter
    {
        public async ValueTask<object?> InvokeAsync(
            EndpointFilterInvocationContext context,
            EndpointFilterDelegate next
        )
        {
            var request = context.GetArgument<GenerateJsonFileRequest>(0);
            if (request == null)
            {
                return TypedResults.ValidationProblem(
                    new Dictionary<string, string[]>()
                    {
                        { nameof(request.File), ["File is required"] },
                    }
                );
            }

            if (
                request!.File!.ContentType
                != "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
            )
            {
                return TypedResults.ValidationProblem(
                    new Dictionary<string, string[]>()
                    {
                        { nameof(request.File), ["File must be xlsx type"] },
                    }
                );
            }

            if (
                request.UpdatedFile != null
                && request.UpdatedFile!.ContentType
                    != "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
            )
            {
                return TypedResults.ValidationProblem(
                    new Dictionary<string, string[]>()
                    {
                        { nameof(request.UpdatedFile), ["UpdatedFile must be xlsx type"] },
                    }
                );
            }

            return await next(context);
        }
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
