using CatalogAPI.Application.Abstractions.Persistence;
using CatalogAPI.Extensions;
using CatalogAPI.Extensions.ExtensionsLogs;
using CatalogAPI.Infrastructure.Connections;
using CatalogAPI.Infrastructure.MongoDB;
using CatalogAPI.Messaging;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings"));

builder.Services.AddSingleton<MongoDbContext>();
builder.Services.AddScoped<MongoDbInitializer>();
builder.Services.AddScoped<IGameCatalogRepository, GameCatalogRepository>();

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration =
        builder.Configuration["Redis:ConnectionString"];

    options.InstanceName = "FCG:Catalog:";
});

builder.Services.AddControllersWithViews();
builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddScoped<OrderPlacedEventPublisher>();

var rabbitMqHost =
    builder.Configuration["RabbitMq:Host"]
    ?? "localhost";

var rabbitMqUsername =
    builder.Configuration["RabbitMq:Username"]
    ?? "guest";

var rabbitMqPassword =
    builder.Configuration["RabbitMq:Password"]
    ?? "guest";

builder.Services.AddMassTransit(configuration =>
{
    configuration.UsingRabbitMq((context, rabbitMq) =>
    {
        rabbitMq.Host(rabbitMqHost, "/", host =>
        {
            host.Username(rabbitMqUsername);
            host.Password(rabbitMqPassword);
        });
    });
});

LogExtension.InitializeLogger();

var loggerSerialLog =
    LogExtension.GetLogger();

loggerSerialLog.Information(
    "Logging initialized.");

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var mongoInitializer =
        scope.ServiceProvider
            .GetRequiredService<MongoDbInitializer>();

    await mongoInitializer.InitializeAsync();
}

app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint(
            "/swagger/v1/swagger.json",
            "FCG Catalog API v1");
    });
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseRouting();

app.UseHttpMetrics();

app.UseCors("CorsPolicy");

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapMetrics();

var runMigrations =
    builder.Configuration.GetValue<bool>(
        "RunMigrations");

if (runMigrations)
{
    using var scope = app.Services.CreateScope();

    var services =
        scope.ServiceProvider;

    try
    {
        var context =
            services.GetRequiredService<DataContext>();

        context.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger =
            services.GetRequiredService<
                ILogger<Program>>();

        logger.LogError(
            ex,
            "An error occurred during migration!");
    }
}

app.Run();