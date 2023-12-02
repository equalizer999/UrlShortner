using System.Buffers;
using System.Security.Cryptography;
using Core.Constants;

namespace Core;

public static class ShortUrlHelper
{
    // Alphanumeric characters excluding 0, O, I, and l
    private const string _choices = "ABCDEFGHJKMNPQRSTUVWXYZabcdefghjkmnpqrstuvwxyz123456789";
    private static readonly char[] _choicesArr = _choices.ToCharArray();
    private static readonly SearchValues<char> _choicesSet = SearchValues.Create(_choices);

    /// <summary>
    ///     Generates a random alphanumeric string of length 8.
    /// </summary>
    public static string GetRandomCode(int length = 8)
    {
        return string.Create(length,
            _choicesArr,
            static (span, choices) =>
            {
                for (var i = 0; i < span.Length; i++)
                {
                    var index = RandomNumberGenerator.GetInt32(0, choices.Length);
                    span[i] = choices[index];
                }
            });
    }

    /// <summary>
    ///     Extracts the code from a valid shortened URL.
    /// </summary>
    public static string GetCode(string shortUrl)
    {
        return string.Create(shortUrl.Length - Globals.BaseUrl.Length,
            shortUrl,
            static (span, url) =>
            {
                for (var i = 0; i < span.Length; i++)
                {
                    span[i] = url[i + Globals.BaseUrl.Length];
                }
            });
    }

    /// <summary>
    ///     Builds a shortened URL from a code.
    /// </summary>
    public static string BuildShortUrl(string shortUrlCode)
    {
        return string.Create(Globals.BaseUrl.Length + shortUrlCode.Length,
            shortUrlCode,
            static (span, code) =>
            {
                for (var i = 0; i < span.Length; i++)
                {
                    span[i] = i < Globals.BaseUrl.Length
                        ? Globals.BaseUrl[i]
                        : code[i - Globals.BaseUrl.Length];
                }
            });
    }

    /// <summary>
    ///     Validates a custom short URL code.
    /// </summary>
    public static bool ValidateCode(string shortUrlCode)
    {
        if (shortUrlCode.Length is < 8 or > 8)
        {
            return false;
        }

        foreach (var c in shortUrlCode)
        {
            if (!_choicesSet.Contains(c))
            {
                return false;
            }
        }

        return true;
    }
}
