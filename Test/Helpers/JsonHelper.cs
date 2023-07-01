namespace Test.Helpers;

public static class JsonHelper
{
    public static async Task<string> ExtractJson(IFormFile file)
    {
        using var streamReader = new StreamReader(file.OpenReadStream());
        return await streamReader.ReadToEndAsync();
    }
}