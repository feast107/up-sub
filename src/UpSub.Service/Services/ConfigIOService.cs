namespace UpSub.Service.Services;

public class ConfigIOService(string filePath)
{
    public async Task<string> LoadAsync() => await File.ReadAllTextAsync(filePath);

    public async Task SaveAsync(string content) => await File.WriteAllTextAsync(filePath, content);
}