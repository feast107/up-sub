using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Text.Encodings.Web;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DynamicData;

namespace UpSub.UI.ViewModels;

public partial class SubConfigViewModel : ObservableObject
{
    private readonly Func<DateTime> time;
    private readonly MainViewModel  main;
    public SubConfigViewModel(Func<DateTime> time, MainViewModel main)
    {
        this.time = time;
        this.main = main;
        Add();
    }
    public ObservableCollection<UrlBlockViewModel> Blocks { get; init; } = [];

    public string Name
    {
        get => name;
        set
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                name = value;
            }
            OnPropertyChanged();
        }
    }

    private string name = DateTime.Now.ToString(CultureInfo.CurrentCulture);

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

    public ObservableCollection<TestResultViewModel> Tests { get; } = [];
    
    [ObservableProperty]
    private CancellationTokenSource? canceler;


    [RelayCommand]
    private void Cancel() => Canceler?.Cancel();

    [ObservableProperty]
    private int count = 10;
    
    [RelayCommand]
    private async Task Test()
    {
        var times = Count;
        Canceler = new CancellationTokenSource();
        TestResultViewModel? test;
        do
        {
            test = new TestResultViewModel(new HttpClient(), Preview, Canceler.Token);
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                Tests.Add(test);
            });
            await test.Start();
        } while (test.State == TestResultViewModel.TestState.Failed 
                 && times-- > 0
                 && !Canceler.IsCancellationRequested);
        
        Canceler = null;
    }
    
    [RelayCommand]
    public void Add()
    {
        var block = new UrlBlockViewModel(TimeHandler, this);
        block.PropertyChanged += BlockOnPropertyChanged;
        Blocks.Add(block);
    }

    [RelayCommand]
    private void Remove()
    {
        main.Configs.Remove(this);
    }
    
    public void Remove(UrlBlockViewModel block)
    {
        block.PropertyChanged -= BlockOnPropertyChanged;
        Blocks.Remove(block);
        OnPropertyChanged(nameof(Preview));
    }

    private DateTime TimeHandler() => time();
    
    private void BlockOnPropertyChanged(object? sender, PropertyChangedEventArgs e) => OnPropertyChanged(nameof(Preview));
}