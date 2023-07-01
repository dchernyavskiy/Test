using Newtonsoft.Json;

namespace Test.Models;

public class Folder
{
    public Folder()
    {
        Id = Guid.NewGuid();
    }

    [JsonIgnore] public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    [JsonIgnore] public Guid? ParentId { get; set; }
    public Folder? Parent { get; set; }

    public ICollection<Folder> Children { get; set; } = Enumerable.Empty<Folder>().ToList();
}