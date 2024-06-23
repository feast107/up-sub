using CommunityToolkit.Mvvm.ComponentModel;

namespace UpSub.UI.ViewModels;

public enum TestState
{
    Success,
    Pending,
    Failed
}
public partial class TestResultViewModel : ObservableObject
{
    
  
    [ObservableProperty] private TestState state = TestState.Pending;

    public required string Url
    {
        get => url;
        init => url = string.IsNullOrWhiteSpace(value) ? "No URL" : value;
    }

    private         string url;

    public required Task<HttpResponseMessage?> Task
    {
        init
        {
            value.ContinueWith(t =>
            {
                State = t.Result is null ? TestState.Failed : TestState.Success;
            });
        }
    }
}