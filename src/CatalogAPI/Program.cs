using CatalogAPI.Extensions;
using CatalogAPI.Extensions.ExtensionsLogs;
using CatalogAPI.Infrastructure.Connections;
using Microsoft.EntityFrameworkCore;
using CatalogAPI.Messaging;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

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
var loggerSerialLog = LogExtension.GetLogger();
loggerSerialLog.Information("Logging initialized.");

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "FCG Catalog API v1");
    });
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseRouting();

app.UseCors("CorsPolicy");

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

var runMigrations = builder.Configuration.GetValue<bool>("RunMigrations");

if (runMigrations)
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;

    try
    {
        var context = services.GetRequiredService<DataContext>();
        context.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred during migration!");
    }
}

app.Run();