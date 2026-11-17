using Asp.Versioning;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.OpenApi;
using SFA.DAS.Api.Common.Infrastructure;
using SFA.DAS.PAS.Api.AppStart;
using SFA.DAS.PAS.Api.Authentication;
using SFA.DAS.PAS.Api.Extensions;
using SFA.DAS.PAS.Api.Infrastructure;
using SFA.DAS.PAS.Application.Extensions;
using SFA.DAS.PAS.Data.Extensions;

var builder = WebApplication.CreateBuilder(args);

IConfiguration _configuration = builder.Configuration.LoadConfiguration();

bool _isLocal = _configuration.IsLocalEnvironment();
string connectionString = _configuration["DatabaseConnectionString"]!;
string applicationInsightsConnectionString = _configuration["ApplicationInsightsConnectionString"]!;

// Add services to the container.

builder.Services
    .AddAuthentication(_configuration, _isLocal)
    .AddPasDataContext(connectionString)
    .AddApplicationServices();

builder.Services
    .AddLogging()
    .AddOpenTelemetryRegistration(applicationInsightsConnectionString);

builder.Services
    .AddOpenApi()
    .AddEndpointsApiExplorer()
    .AddSwaggerGen(options =>
    {
        options.SwaggerDoc(ApiTags.Integration, new OpenApiInfo
        {
            Title = "PAS API Integration Endpoints",
            Version = "v1"
        });
        options.SwaggerDoc(ApiTags.Management, new OpenApiInfo
        {
            Title = "PAS API Management Endpoints",
            Version = "v1"
        });
    });

builder.Services.AddControllers(options =>
    {
        options.Conventions.Add(new ApiExplorerGroupingByTagAttributeConvention());
    });

builder.Services.AddApiVersioning(opt =>
    {
        opt.ApiVersionReader = new HeaderApiVersionReader("X-Version");
        opt.DefaultApiVersion = new ApiVersion(1, 0);
    });


var app = builder.Build();


// Configure the HTTP request pipeline.

app.MapOpenApi();
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint($"swagger/{ApiTags.Integration}/swagger.json", ApiTags.Integration);
    options.SwaggerEndpoint($"swagger/{ApiTags.Management}/swagger.json", ApiTags.Management);
    options.RoutePrefix = string.Empty;
});

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseHealthChecks("/ping",
    new HealthCheckOptions
    {
        ResponseWriter = HealthCheckResponseWriter.WriteJsonResponse
    });

if (_isLocal)
{
    app.MapControllers().AllowAnonymous();
}
else
{
    app.MapControllers();
}

await app.RunAsync();
