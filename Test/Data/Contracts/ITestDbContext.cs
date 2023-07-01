using Microsoft.EntityFrameworkCore;
using Test.Models;

namespace Test.Data.Contracts;

public interface ITestDbContext
{
    DbSet<Folder> Folders { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}