namespace Test.Services.Contracts;

public interface IService<T>
{
    Task AddAsync();
    Task RemoveAsync();
    Task UpdateAsync();
    Task GetAsync();
}