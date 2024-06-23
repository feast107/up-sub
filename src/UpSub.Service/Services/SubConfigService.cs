using System.Text.Json;
using UpSub.Abstractions;

namespace UpSub.Service.Services;

public class SubConfigService(ConfigRequestService request, ConfigIOService configIo)
{
    public async Task ExecuteAsync(string trail, HttpContext context)
    {
        var config = (await Configs()).FirstOrDefault(x => x.Name == trail);
        if (config == null)
        {
            await Results.NotFound().ExecuteAsync(context);
            return;
        }

        await foreach (var (_, task) in request.Request(config, DateTime.Today))
        {
            var response = await task;
            if (response == null) continue;
            await response.Content.CopyToAsync(context.Response.BodyWriter.AsStream());
            return;
        }

        await Results.NotFound().ExecuteAsync(context);
    }

    public async Task<List<SubConfig>> Configs()
    {
        if (configs != null) return configs;
        try
        {
            configs = JsonSerializer.Deserialize(await configIo.LoadAsync(),
                AppJsonSerializerContext.Default.ListSubConfig);
        }
        catch
        {
            configs = [];
            await Save();
        }

        return configs!;
    }

    private List<SubConfig>? configs;

    public async Task Save() =>
        await configIo.SaveAsync(JsonSerializer.Serialize(await Configs(),
            AppJsonSerializerContext.Default.ListSubConfig));
}