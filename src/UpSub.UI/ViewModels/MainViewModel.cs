using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DynamicData;
using UpSub.Abstractions;
using UpSub.Service.Services;

namespace UpSub.UI.ViewModels;

public partial class MainViewModel : ObservableObject
{
    public required ConfigRequestService RequestService { get; init; }

    private readonly List<SubConfig> configs;
    
    public MainViewModel(List<SubConfig> configs)
    {
        this.configs = configs;
    }

    public ObservableCollection<SubConfigViewModel> Configs { get; set; } = [];
  

    [RelayCommand]
    private void Add()
    {
        var config = new SubConfig
        {
            Name = DateTime.Now.ToString(CultureInfo.CurrentCulture)
        };
        configs.Add(config);
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
    private void Save()
    {
        foreach (var config in Configs) config.Save();
    }
}