using System.Reflection;
using VnRegion.Common.Extensions;
using VnRegion.Regions.Services;
using VnRegion.Regions.Settings;

var builder = WebApplication.CreateBuilder(args);
builder
    .Services.Configure<NameConfigurationSettings>(
        builder.Configuration.GetSection(nameof(NameConfigurationSettings))
    )
    .AddSingleton<GenerateJsonFileService>()
    .AddSingleton<IGenerate, GenerateJsonFileService>(x => x.GetService<GenerateJsonFileService>()!)
    .AddSingleton<GenerateSqlFileService>()
    .AddSingleton<IGenerate, GenerateSqlFileService>(x => x.GetService<GenerateSqlFileService>()!)
    .AddSingleton<IGenerator, Generator>();

var currentAssembly = Assembly.GetExecutingAssembly();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddEndpoints(currentAssembly);
builder.Services.AddSwaggerGen();
builder.Services.AddAntiforgery();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(x =>
    {
        x.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        x.RoutePrefix = "docs";
        x.ConfigObject.PersistAuthorization = true;
    });
}

app.UseAntiforgery();
app.MapEndpoints();
app.Run();
