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

        var day = DateTime.Today;
        await foreach (var (_, task) in request.RequestAsync(config, () => day))
        {
            var (response, error, errorKind) = await task;
            if (response == null)
            {
                switch (errorKind)
                {
                    case ErrorKind.NotFound:
                        day -= TimeSpan.FromDays(1); //try yesterday
                        break;
                    case ErrorKind.Cancelled:
                    case ErrorKind.NameResolutionError:
                    case ErrorKind.Unknown:
                        goto failed;
                }
                continue;
            }
            await response.Content.CopyToAsync(context.Response.BodyWriter.AsStream());
            return;
        }
        failed:
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