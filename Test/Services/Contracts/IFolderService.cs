using Test.Models;

namespace Test.Services.Contracts;

public interface IFolderService
{
    Task<Folder> FindByPathAsync(string path);
    Task ImportFromOS();
    Task ImportFromFile(string json);
    Task<string> ExportToFileAsync();
    Task<ICollection<Folder>> GetAllParentFoldersAsync();
}