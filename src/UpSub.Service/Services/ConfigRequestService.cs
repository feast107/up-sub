using System.Runtime.CompilerServices;
using UpSub.Abstractions;

namespace UpSub.Service.Services;

public class ConfigRequestService(Func<HttpClient> clientFactory)
{
    public async IAsyncEnumerable<(string url, Task<HttpResponseMessage?> waiter)> Request(SubConfig config,
        DateTime startTime,
        [EnumeratorCancellation] CancellationToken token = default)
    {
        var count = config.Count;
        while (count-- > 0)
        {
            var url    = config.Url(startTime);
            if (string.IsNullOrWhiteSpace(url)) yield break;
            var client = clientFactory();
            var task = Factory();
            yield return (url, task);
            var response = await task;
            if (token.IsCancellationRequested) yield break;
            if (response != null) yield break;
            startTime = startTime.AddDays(-1);
            continue;

            async Task<HttpResponseMessage?> Factory()
            {
                HttpResponseMessage? message;
                try
                {
                    message = await client.GetAsync(url, token);
                }
                catch
                {
                    //
                    message = null;
                }

                return message;
            }
        }
    }
}