using System.Runtime.CompilerServices;
using UpSub.Abstractions;

namespace UpSub.Service.Services;

public class ConfigRequestService(Func<HttpClient> clientFactory)
{
    public async IAsyncEnumerable<(string url, Task<ConfigTestResult> waiter)> RequestAsync(SubConfig config,
        Func<DateTime> time,
        [EnumeratorCancellation] CancellationToken token = default)
    {
        var count = config.Count;
        while (count-- > 0)
        {
            var url = config.Url(time());
            if (string.IsNullOrWhiteSpace(url)) yield break;
            var client = clientFactory();
            var task = Factory();
            yield return (url, task);
            var response = await task;
            if (token.IsCancellationRequested) yield break;
            if (response.Response != null) yield break;
            continue;

            async Task<ConfigTestResult> Factory()
            {
                HttpResponseMessage? message;
                HttpRequestError?    error;
                try
                {
                    message = await client.GetAsync(url, token);
                    error   = null;
                }
                catch(Exception exception)
                {
                    error = exception is HttpRequestException requestException 
                        ? requestException.HttpRequestError 
                        : null;

                    //
                    message = null;
                }

                return new ConfigTestResult(message, error);
            }
        }
    }
}