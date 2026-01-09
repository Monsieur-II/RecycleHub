using Microsoft.AspNetCore.HttpLogging;
using RecycleHub.Api;
using RecycleHub.Pg.Sdk;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

services.AddControllers();

services.AddHttpLogging(options =>
{
    options.LoggingFields = HttpLoggingFields.All;
    options.RequestBodyLogLimit = 4096;
    options.ResponseBodyLogLimit = 4096;
});

services.AddRouting(x => x.LowercaseUrls = true);

services.AddPostgres(builder.Configuration, "RecycleHubDb");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

await app.ApplyPendingMigrations();

app.UseRouting();
app.UseCors();
app.MapControllers();


await app.RunAsync();
