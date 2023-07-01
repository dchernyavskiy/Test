namespace Test.Exceptions;

public class FolderNotFoundException : Exception
{
    public FolderNotFoundException() 
        : base("Folder not found.") { }
}