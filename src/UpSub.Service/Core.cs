using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using UpSub.Abstractions;
using UpSub.Service.Services;

namespace UpSub.Service;

public class Core
{
    public  IServiceProvider? ServiceProvider { get; set; }
    private WebApplication?   app;

    public bool IsRunning { get; private set; }

    public string Url(string name) => $"http://localhost:{port}/sub/{name}";

    private int port;
    
    [MemberNotNull(nameof(ServiceProvider))]
    public async Task Build(int port)
    {
        if (IsRunning) throw new InvalidOperationException("App is running , stop first");
        if (app != null) await app.DisposeAsync();
        this.port = port;
        var builder = WebApplication.CreateSlimBuilder();
        builder.WebHost.ConfigureKestrel(x => x.ListenLocalhost(port));
        builder.Services.AddSingleton(new ConfigIOService(Path.Combine(AppContext.BaseDirectory, "subconfig.json")));
        builder.Services.AddSingleton<Func<HttpClient>>(() => new HttpClient());
        builder.Services.AddSingleton<ConfigRequestService>();
        builder.Services.AddSingleton<SubConfigService>();
        builder.Services.ConfigureHttpJsonOptions(options =>
            options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default));

        app = builder.Build();
        app.MapGet("/sub/{**trail}",
            async (HttpContext context, [FromRoute] string trail, [FromServices] SubConfigService service) =>
            await service.ExecuteAsync(trail, context));
        ServiceProvider = app.Services;
    }

    public Task Start()
    {
        if (IsRunning)  throw new InvalidOperationException("Already in running state");
        if (app is null) throw new InvalidOperationException("App haven't been built");
        IsRunning = true;
        return app.StartAsync();
    }

    public Task Stop()
    {
        if (!IsRunning || app is null) return Task.CompletedTask;
        IsRunning = false;
        return app.StopAsync();
    }
}

[JsonSerializable(typeof(List<SubConfig>))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{
    public static AppJsonSerializerContext Intend { get; } = new(new JsonSerializerOptions
    {
        WriteIndented = true
    });
}
