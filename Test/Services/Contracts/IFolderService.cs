using Test.Models;

namespace Test.Services.Contracts;

public interface IFolderService
{
    Task<Folder> FindByPathAsync(string path);
}