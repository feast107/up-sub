using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace UpSub.UI.ViewModels;

public partial class UrlBlockViewModel(Func<DateTime> time, SubConfigViewModel config) : ObservableObject
{
    [RelayCommand]
    private void Add()
    {
        IsAdder = false;
        config.Add();
    }

    [ObservableProperty]
    private bool isAdder = true;

    
    [ObservableProperty] [NotifyPropertyChangedFor(nameof(Text))]
    private string template = string.Empty;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(Text))]
    private bool isTemplate;

    public bool Focused
    {
        get => config.Selected == this;
        set
        {
            if (value && config.Selected != this)
            {
                config.Selected = this;
            }
            OnPropertyChanged();
        }
    }

    public string Text
    {
        get
        {
            try
            {
                return IsTemplate ? time().ToString((string?)Template) : Template;
            }
            catch
            {
                //
            }
            return Template;
        }
    }

    [RelayCommand]
    private void Remove() => config.Remove(this);
}