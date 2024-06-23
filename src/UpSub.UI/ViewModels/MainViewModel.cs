using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using UpSub.Abstractions;

namespace UpSub.UI.ViewModels;

public partial class MainViewModel : ObservableObject
{
    public MainViewModel(SubConfig[] configs)
    {
        Add();
    }

    public ObservableCollection<SubConfigViewModel> Configs { get; set; } = [];
  

    [RelayCommand]
    private void Add()
    {
        Configs.Add(new SubConfigViewModel(() => DateTime.Today, this));
    }
}