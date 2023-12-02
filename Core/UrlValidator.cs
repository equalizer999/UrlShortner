using Core.Constants;

namespace Core;

public static class UrlValidator
{
    /// <summary>
    ///     Checks if URL is not null, whitespace, or empty, well-formed, and not a shortened URL.
    /// </summary>
    /// <param name="url"></param>
    public static string? Original(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return Errors.EmptyUrl;
        }

        if (url.Length > 2_000_000)
        {
            return Errors.UrlTooLarge;
        }

        if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
        {
            return Errors.InvalidUrl;
        }

        if (url.Contains(Globals.BaseUrl, StringComparison.OrdinalIgnoreCase) ||
            url.Contains(Globals.BaseDomain, StringComparison.OrdinalIgnoreCase))
        {
            return Errors.UrlAlreadyShortened;
        }

        return null;
    }

    /// <summary>
    ///     Checks if URL is not null, whitespace, or empty, well-formed, and a shortened URL.
    /// </summary>
    /// <param name="url"></param>
    public static string? Shortened(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return Errors.EmptyUrl;
        }

        if (url.Length > 2_000_000)
        {
            return Errors.UrlTooLarge;
        }

        if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
        {
            return Errors.InvalidUrl;
        }

        for (var i = 0; i < Globals.BaseUrl.Length; i++)
        {
            if (url[i] != Globals.BaseUrl[i])
            {
                return Errors.InvalidShortUrl;
            }
        }
        return null;
    }
}
