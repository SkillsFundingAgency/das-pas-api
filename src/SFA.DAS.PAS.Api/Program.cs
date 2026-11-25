using FluentValidation;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.OpenApi;
using NServiceBus.ObjectBuilder.MSDependencyInjection;
using SFA.DAS.Api.Common.Infrastructure;
using SFA.DAS.NServiceBus.Configuration.MicrosoftDependencyInjection;
using SFA.DAS.PAS.Api.AppStart;
using SFA.DAS.PAS.Api.Authentication;
using SFA.DAS.PAS.Api.Extensions;
using SFA.DAS.PAS.Api.Infrastructure;
using SFA.DAS.PAS.Api.Validators;
using SFA.DAS.PAS.Application.Extensions;
using SFA.DAS.PAS.Data.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Host.UseServiceProviderFactory(new NServiceBusServiceProviderFactory());

IConfiguration _configuration = builder.Configuration.LoadConfiguration();

builder.Host.ConfigureContainer<UpdateableServiceProvider>(containerBuilder =>
{
    Task.FromResult(containerBuilder.StartNServiceBus(_configuration, "SFA.DAS.PAS.Api"));
});

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
    .AddOpenApi(ApiTags.Integration, options =>
    {
        options.ShouldInclude = description => description.GroupName == ApiTags.Integration;
        options.AddDocumentTransformer((document, context, cancellationToken) =>
        {
            document.Info = new OpenApiInfo
            {
                Title = "PAS API Integration Endpoints",
                Version = "v1"
            };
            return Task.CompletedTask;
        });
        options.AddOperationTransformer<VersionHeaderTransformer>();
        options.AddOperationTransformer<AuthorizationHeaderTransformer>();
    })
    .AddOpenApi(ApiTags.Management, options =>
    {
        options.ShouldInclude = description => description.GroupName == ApiTags.Management;
        options.AddDocumentTransformer((document, context, cancellationToken) =>
        {
            document.Info = new OpenApiInfo
            {
                Title = "PAS API Management Endpoints",
                Version = "v1"
            };
            return Task.CompletedTask;
        });
        options.AddOperationTransformer<VersionHeaderTransformer>();
        options.AddOperationTransformer<AuthorizationHeaderTransformer>();
    })
    .AddEndpointsApiExplorer();

builder.Services.AddControllers(options =>
    {
        options.Conventions.Add(new ApiExplorerGroupingByTagAttributeConvention());
    });

builder.Services.AddApiVersioningRegistration();
builder.Services.AddValidatorsFromAssemblyContaining<ProviderEmailModelValidator>();


var app = builder.Build();


// Configure the HTTP request pipeline.

app.MapOpenApi("/swagger/{documentName}/swagger.json");
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint($"/swagger/{ApiTags.Integration}/swagger.json", ApiTags.Integration);
    options.SwaggerEndpoint($"/swagger/{ApiTags.Management}/swagger.json", ApiTags.Management);
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

app.MapControllers();

await app.RunAsync();
