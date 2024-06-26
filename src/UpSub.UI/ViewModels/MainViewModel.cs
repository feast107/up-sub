using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using UpSub.Abstractions;
using UpSub.Service;
using UpSub.Service.Services;

namespace UpSub.UI.ViewModels;

public partial class MainViewModel : ObservableObject
{
    public required ConfigRequestService RequestService { get; init; }
    public required SubConfigService     ConfigService  { get; init; }

    public required Core Core { get; init; }


    [ObservableProperty]
    private int port = 6043;
    
    public  ObservableCollection<SubConfigViewModel> Configs { get; set; } = [];
    private List<SubConfig>                          configs { get; set; } = [];
    
    public async Task Load()
    {
        configs = await ConfigService.Configs();
        foreach (var config in configs)
        {
            Add(config, true);
        }
    }

    public string Url(string name) => Core.Url(name);

    [RelayCommand]
    private void Add() => Add(new SubConfig
    {
        Name = Global.RandomString
    });

    private void Add(SubConfig config, bool isLoad = false)
    {
        if(!isLoad) configs.Add(config);
        Configs.Add(new SubConfigViewModel(config)
        {
            RequestService = RequestService,
            Time           = () => DateTime.Today,
            Main           = this
        });
    }
 

    public void Remove(SubConfigViewModel config)
    {
        Configs.Remove(config);
        configs.Remove(config.Config);
    }

    [RelayCommand]
    private async Task Save()
    {
        foreach (var config in Configs) config.Save();
        await ConfigService.Save();
    }
}