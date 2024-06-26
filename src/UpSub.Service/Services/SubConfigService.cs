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

        await foreach (var (_, task) in request.RequestAsync(config,  DateTime.Today))
        {
            var (response, error, errorKind) = await task;

            Console.WriteLine(errorKind);
            switch (errorKind)
            {
                case ErrorKind.NotFound: continue;
                case ErrorKind.Cancelled:
                case ErrorKind.NameResolutionError:
                case ErrorKind.Unknown:
                    goto failed;
                case ErrorKind.NoError:
                    if (response != null)
                    {
                        await response.Content.CopyToAsync(context.Response.BodyWriter.AsStream());
                    }
                    return;
            }

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
            AppJsonSerializerContext.Intend.ListSubConfig));
}