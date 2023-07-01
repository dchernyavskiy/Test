using Microsoft.EntityFrameworkCore;
using Test.Data;

namespace Test.Extensions;

public static class ApplicationBuilderExtensions
{
    public static async Task ApplyDatabaseMigrations(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TestDbContext>();
        await context.Database.MigrateAsync();
    }
}