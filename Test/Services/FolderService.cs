using Microsoft.EntityFrameworkCore;
using Test.Data.Contracts;
using Test.Data.Seeders;
using Test.Exceptions;
using Test.Models;
using Test.Services.Contracts;

namespace Test.Services;

public class FolderService : IFolderService
{
    private readonly ITestDbContext _context;

    public FolderService(ITestDbContext context)
    {
        _context = context;
    }

    public async Task<Folder> FindByPathAsync(string path)
    {
        var segments = path.Trim().Split("/");
        var folder = await _context.Folders
            .Include(x => x.Children)
            .FirstOrDefaultAsync(x => x.Name == segments[0]);
        return await FindByPathAsync(folder, segments);
    }
    
    private async Task<Folder> FindByPathAsync(Folder folder, string[] segments, int index = 1)
    {
        if (folder != null && segments.Length == index)
            return folder;

        if (folder == null || segments.Length == index)
            throw new FolderNotFoundException();

        var child = folder.Children.FirstOrDefault(x => x.Name == segments[index]);
        if (child != null)
        {
            child = await _context.Folders
                .Include(x => x.Children)
                .FirstOrDefaultAsync(x => x.Id == child.Id);
            return await FindByPathAsync(child, segments, index + 1);
        }

        return null!;
    }
}