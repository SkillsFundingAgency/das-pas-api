using SFA.DAS.PAS.Api.AppStart;
using SFA.DAS.PAS.Api.Extensions;
using SFA.DAS.PAS.Application.Extensions;
using SFA.DAS.PAS.Data.Extensions;

var builder = WebApplication.CreateBuilder(args);

IConfiguration _configuration = builder.Configuration.LoadConfiguration();

bool _isLocal = _configuration.IsLocalEnvironment();

// Add services to the container.

builder.Services.AddControllers();
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
    .AddPasDataContext(connectionString)
    .AddApplicationServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
