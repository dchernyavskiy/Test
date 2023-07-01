using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Test.Data.Contracts;
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

    public async Task ImportFromOS()
    {
        var path = DriveInfo.GetDrives().First().Name;
        var osFolder = new Folder() { Name = "OS", Children = FillFolder(path) };

        await _context.Folders.AddAsync(osFolder);
        await _context.SaveChangesAsync();
    }

    private ICollection<Folder> FillFolder(string path)
    {
        ICollection<Folder> folders = new List<Folder>();
        if (Directory.Exists(path))
        {
            folders = Directory.GetDirectories(path)
                .Select(x =>
                    new Folder() { Name = Path.GetFileName(x) })
                .ToList();
            foreach (var folder in folders)
            {
                try
                {
                    folder.Children = FillFolder(path + "/" + folder.Name);
                }
                catch (Exception e)
                {
                }
            }
        }

        return folders;
    }


    public async Task ImportFromFile(string json)
    {
        var folders = JsonConvert.DeserializeObject<ICollection<Folder>>(json);

        if (folders != null) await _context.Folders.AddRangeAsync(folders);
        await _context.SaveChangesAsync();
    }

    public async Task<string> ExportToFileAsync()
    {
        var folders = await _context.Folders
            .FromSqlInterpolated($@"
                WITH RECURSIVE RecursiveFolders AS (
                    SELECT
                        ""Id"", ""Name"", ""ParentId""
                    FROM
                        public.""Folders""
                    UNION ALL
                    SELECT
                        c.""Id"",
                        c.""Name"",
                        c.""ParentId""
                    FROM
                        public.""Folders"" c
                    INNER JOIN
                        RecursiveFolders rc ON c.""ParentId"" = rc.""Id""
                )
                SELECT
                    ""Id"",
                    ""Name"",
                    ""ParentId""
                FROM
                    RecursiveFolders;")
            .ToListAsync();
        folders = folders.Where(x => x.ParentId == null).ToList();
        return JsonConvert.SerializeObject(folders, new JsonSerializerSettings()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            Formatting = Formatting.Indented
        });
    }

    public async Task<ICollection<Folder>> GetAllParentFoldersAsync()
    {
        return await _context.Folders.Where(x => x.ParentId == null).ToListAsync();
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