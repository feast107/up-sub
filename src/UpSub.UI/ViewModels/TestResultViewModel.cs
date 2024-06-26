using CommunityToolkit.Mvvm.ComponentModel;
using UpSub.Abstractions;

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

    public required Task<ConfigTestResult> Task
    {
        init
        {
            value.ContinueWith(t =>
                State = t.Result.ErrorKind is ErrorKind.NoError
                    ? TestState.Success
                    : TestState.Failed);
        }
    }
}