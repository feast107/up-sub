using UpSub.Abstractions;

namespace UpSub.Service.Services;

public class ConfigRequestService(HttpClient client)
{
    public async IAsyncEnumerable<(string url, HttpResponseMessage? response)> Request(SubConfig config,
        DateTime startTime)
    {
        var count = config.Count;
        while (count-- > 0)
        {
            var url = config.Url(startTime);

            HttpResponseMessage? response;
            try
            {
                response = await client.GetAsync(url);
            }
            catch
            {
                //
                response = null;
            }

            yield return (url, response);
            if (response != null) yield break;
            startTime = startTime.AddDays(-1);
        }
    }
}