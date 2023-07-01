using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Test.Data.Contracts;
using Test.Models;

namespace Test.Data;

public class TestDbContext : DbContext, ITestDbContext
{
    public TestDbContext(DbContextOptions<TestDbContext> options) 
        : base(options) { }
    
    public DbSet<Folder> Folders => Set<Folder>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        
        base.OnModelCreating(modelBuilder);
    }
}