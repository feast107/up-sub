using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using UpSub.Abstractions;
using UpSub.Service.Services;

namespace UpSub.Service;

public class Core
{
    public  IServiceProvider? ServiceProvider { get; set; }
    private WebApplication?   app;

    [MemberNotNull(nameof(ServiceProvider))]
    public async Task Build(int port)
    {
        if (app != null) await app.DisposeAsync();
        var builder = WebApplication.CreateSlimBuilder();
        builder.WebHost.ConfigureKestrel(x => x.ListenLocalhost(port));
        builder.Services.AddSingleton(new ConfigIOService(Path.Combine(AppContext.BaseDirectory, "subconfig.json")));
        builder.Services.AddSingleton(new HttpClient());
        builder.Services.AddSingleton<ConfigurationRoot>();
        builder.Services.AddSingleton<SubConfigService>();
        builder.Services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
        });

        app = builder.Build();
        app.MapGet("/sub/{**trail}",
            async (HttpContext context, [FromRoute] string trail, [FromServices] SubConfigService service) =>
            {
                await service.ExecuteAsync(trail, context);
            });
        ServiceProvider = app.Services;
    }

    public Task Start() => app?.StartAsync() ?? Task.CompletedTask;

    public void Stop() => app?.StopAsync();
}

[JsonSerializable(typeof(SubConfig[]))]
internal partial class AppJsonSerializerContext : JsonSerializerContext;
