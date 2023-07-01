using Microsoft.EntityFrameworkCore;
using Test.Data.Contracts;
using Test.Models;

namespace Test.Data.Seeders;

public class FolderDataSeeder : IDataSeeder
{
    private readonly ITestDbContext _context;

    public FolderDataSeeder(ITestDbContext context)
    {
        _context = context;
    }

    public async Task SeedAllAsync()
    {
        if (!await _context.Folders.AnyAsync())
        {
            var folder = new Folder()
            {
                Name = "Creating Digital Images",
                Children = new List<Folder>()
                {
                    new()
                    {
                        Name = "Resources",
                        Children = new List<Folder>()
                        {
                            new(){ Name = "Primary Sources"},
                            new(){ Name = "Secondary Sources"},
                        }
                    },
                    new(){Name = "Evidence"},
                    new()
                    {
                        Name = "Graphic Products",
                        Children = new List<Folder>()
                        {
                            new(){ Name = "Process"},
                            new(){ Name = "Final Product"},
                        }
                    }
                }
            };

            await _context.Folders.AddAsync(folder);
            await _context.SaveChangesAsync();
        }
    }
}