using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Serilog;
using Test.Data;
using Test.Data.Contracts;
using Test.Data.Options;
using Test.Data.Seeders;
using Test.Extensions;
using Test.Services;
using Test.Services.Contracts;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

builder.Services.AddControllersWithViews()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
    });
builder.Services.AddOptions<PostgresOptions>().BindConfiguration(nameof(PostgresOptions));
builder.Services.AddSingleton<PostgresOptions>(x => x.GetRequiredService<IOptions<PostgresOptions>>().Value);
builder.Services.AddDbContext<ITestDbContext, TestDbContext>((sp, options) =>
{
    var postgresOptions = sp.GetRequiredService<PostgresOptions>();

    options.UseNpgsql(
        postgresOptions.ConnectionString,
        sqlOptions =>
        {
            var name = Assembly.GetExecutingAssembly().GetName().Name;
            sqlOptions.MigrationsAssembly(name);
            sqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
        }
    );
});
builder.Services.AddScoped<IDataSeeder, FolderDataSeeder>();
builder.Services.AddScoped<IFolderService, FolderService>();

var app = builder.Build();
await app.ApplyDatabaseMigrations();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else if (!app.Environment.IsEnvironment("Test"))
{
    using var scope = app.Services.CreateScope();
    var seeders = scope.ServiceProvider.GetServices<IDataSeeder>();

    foreach (var seeder in seeders)
    {
        await seeder.SeedAllAsync();
    }
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.MapControllerRoute(
    name: "export",
    pattern: "Export",
    new { controller = "Folder", action = "Export" });

app.MapControllerRoute(
    name: "ImportFromOS",
    pattern: "ImportFromOS",
    new { controller = "Folder", action = "ImportFromOs" });

app.MapControllerRoute(
    name: "ImportFromFile",
    pattern: "ImportFromFile",
    new { controller = "Folder", action = "ImportFromFile" });

app.MapControllerRoute(
    name: "default",
    pattern: "{*segments}",
    new { controller = "Folder", action = "Index" });


app.Run();