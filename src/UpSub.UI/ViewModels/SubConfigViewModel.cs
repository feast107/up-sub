using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Text.Encodings.Web;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using UpSub.Abstractions;
using UpSub.Service.Services;

namespace UpSub.UI.ViewModels;

public partial class SubConfigViewModel : ObservableObject
{
    public required ConfigRequestService RequestService { get; init; }
    public required Func<DateTime> Time { get; init; }
    public required MainViewModel  Main { get; init; }

    public readonly SubConfig Config;
    
    public SubConfigViewModel(SubConfig config)
    {
        Config = config;
        Load();
    }

    public void Load()
    {
        Name = Config.Name;
        Count = Config.Count;
        Encode = Config.Encode;
        Blocks.Clear();
        foreach (var model in Config.Blocks.Select(x =>
                 {
                     var ret = new UrlBlockViewModel(TimeHandler, this)
                     {
                         IsAdder = false,
                         Block   = x
                     };
                     ret.PropertyChanged += BlockOnPropertyChanged;
                     return ret;
                 })) Blocks.Add(model);
        Add();
    }
    
    
    public ObservableCollection<UrlBlockViewModel> Blocks { get; init; } = [];

    public string Name
    {
        get => name;
        set
        {
            if (!string.IsNullOrWhiteSpace(value)) name = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(Url));
        }
    }

    private string name = string.Empty;

    [ObservableProperty]
    private bool editingName;
    
    public UrlBlockViewModel? Selected
    {
        get => selected;
        set
        {
            if (selected == value) return;
            if (selected != null)
            {
                var temp = selected;
                selected         = null;
                temp.Focused = false;
            }
            selected = value;
            if (selected != null) selected.Focused = true;
            OnPropertyChanged();
        }
    }

    private UrlBlockViewModel? selected;

    [ObservableProperty][NotifyPropertyChangedFor(nameof(Preview))]
    private bool encode;
    
    public string Preview
    {
        get
        {
            var fin = string.Join(string.Empty, Blocks.SkipLast(1).Select(x => x.Text));
            return Encode ? UrlEncoder.Default.Encode(fin) : fin;
        }
    }
    
    public string Url => Main.Url(Name);

    public ObservableCollection<TestResultViewModel> Tests { get; } = [];
    
    [ObservableProperty][NotifyPropertyChangedFor(nameof(CanClean))]
    private CancellationTokenSource? canceler;

    public bool CanClean => Canceler is null && Tests.Count > 0;

    [RelayCommand]
    private void CleanTest()
    {
        Tests.Clear();
        OnPropertyChanged(nameof(CanClean));
    }

    [RelayCommand]
    private void Cancel() => Canceler?.Cancel();

    [ObservableProperty]
    private int count = 10;
    
    [RelayCommand]
    private async Task Test()
    {
        await Dispatcher.UIThread.InvokeAsync(() => Tests.Clear());
        Canceler = new CancellationTokenSource();
        await foreach (var (url, task) in RequestService.RequestAsync(Config, Time(), Canceler.Token))
        {
            await Dispatcher.UIThread.InvokeAsync(() => Tests.Add(new TestResultViewModel
            {
                Url  = url,
                Task = task
            }));
        }
        
        Canceler = null;
    }

    public void Save()
    {
        Config.Name = Name;
        Config.Blocks = Blocks
            .SkipLast(1)
            .Select(x => x.Block)
            .ToList();
        Config.Encode = Encode;
        Config.Count  = Count;
    }

    [RelayCommand]
    public void Add()
    {
        var block = new UrlBlockViewModel(TimeHandler, this)
        {
            Block = new UrlBlock()
        };
        block.PropertyChanged += BlockOnPropertyChanged;
        Blocks.Add(block);
    }

    [RelayCommand]
    private void Remove() => Main.Remove(this);

    public void Remove(UrlBlockViewModel block)
    {
        block.PropertyChanged -= BlockOnPropertyChanged;
        Blocks.Remove(block);
        OnPropertyChanged(nameof(Preview));
    }

    private DateTime TimeHandler() => Time();
    
    private void BlockOnPropertyChanged(object? sender, PropertyChangedEventArgs e) => OnPropertyChanged(nameof(Preview));
}