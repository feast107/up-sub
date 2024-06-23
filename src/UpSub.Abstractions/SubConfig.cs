using System.Text.Encodings.Web;

namespace UpSub.Abstractions;

public class SubConfig
{
    public required string Name { get; set; }
    public List<UrlBlock> Blocks { get; set; } = [];
    
    public int Count { get; set; }
    
    public bool Encode { get; set; }

    public string Url(DateTime time)
    {
        var fin = string.Join(string.Empty, Blocks.Select(x =>
        {
            if (!x.IsTemplate) return x.Template;
            try
            {
                return time.ToString(x.Template);
            }
            catch
            {
                //
            }

            return x.Template;
        }));
        return Encode ? UrlEncoder.Default.Encode(fin) : fin;
    }
}