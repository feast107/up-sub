namespace UpSub.Abstractions;

public class Global
{
    public static DateTime DateTime => DateTime.Today;

    // ReSharper disable once StringLiteralTypo
    private static string Chars => "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

    public static string RandomString
    {
        get
        {
            var arr    = new char[8];
            var random = new Random();
            for (var i = 0; i < arr.Length; i++)
                arr[i] = Chars[random.Next(Chars.Length)];

            return new string(arr);
        }
    }
}