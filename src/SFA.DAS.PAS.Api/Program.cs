using Asp.Versioning;
using Microsoft.AspNetCore.Mvc.Authorization;
using SFA.DAS.PAS.Api.AppStart;
using SFA.DAS.PAS.Api.Authentication;
using SFA.DAS.PAS.Api.Extensions;
using SFA.DAS.PAS.Application.Extensions;
using SFA.DAS.PAS.Data.Extensions;

var builder = WebApplication.CreateBuilder(args);

IConfiguration _configuration = builder.Configuration.LoadConfiguration();

bool _isLocal = _configuration.IsLocalEnvironment();

// Add services to the container.

builder.Services.AddControllers(options =>
{
    if (_isLocal)
    {
        options.Filters.Add(new AllowAnonymousFilter());
    }
});
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.OpenApiInfo
    {
        Title = "SFA.DAS.PAS.Api",
        Version = "v1"
    });
});

string connectionString = _configuration["DatabaseConnectionString"]!;

builder
    .Services
    .AddAuthentication(_configuration, _isLocal)
    .AddPasDataContext(connectionString)
    .AddApplicationServices()
    .AddApiVersioning(opt =>
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
    options.SwaggerEndpoint("swagger/v1/swagger.json", "v1");
    options.RoutePrefix = string.Empty;
});

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
