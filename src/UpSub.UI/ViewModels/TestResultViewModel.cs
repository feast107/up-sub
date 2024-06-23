using CommunityToolkit.Mvvm.ComponentModel;

namespace UpSub.UI.ViewModels;

public partial class TestResultViewModel(HttpClient client, string url, CancellationToken token) : ObservableObject
{
    public enum TestState
    {
        Success,
        Pending,
        Failed
    }

    [ObservableProperty] private TestState state = TestState.Pending;

    public string Url => url;

    public async Task<TestState> Start()
    {
        try
        {
            await client.GetAsync(url, token);
            return State = TestState.Success;
        }
        catch
        {
            return State = TestState.Failed;
        }
    }
}