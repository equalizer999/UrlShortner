using System.Net;
using Core;

namespace Services;

public sealed class UrlService(IUrlDatastore urlDatastore) : IUrlService
{
    public Result<string> ShortenUrl(string longUrl, string? customCode = null)
    {
        var validationError = UrlValidator.Original(longUrl);
        if (validationError is not null)
        {
            return Result<string>.Fail(validationError);
        }
        if (customCode is not null)
        {
            if (!ShortUrlHelper.ValidateCode(customCode))
            {
                return Result<string>.Fail(
                    "Short URL code is invalid. Codes must be 8 characters long and contain only alphanumeric characters excluding 0, O, I, and l.");
            }

            if (urlDatastore.IsShortUrlCodeInUse(customCode))
            {
                return Result<string>.Fail("Short URL code is already in use.");
            }
        }
        var shortUrlCode = urlDatastore.CreateShortUrlCode(WebUtility.UrlEncode(longUrl), customCode);
        return ShortUrlHelper.BuildShortUrl(shortUrlCode);
    }

    public Result<string> GetOriginalUrl(string shortUrl)
    {
        var validationError = UrlValidator.Shortened(shortUrl);
        if (validationError is not null)
        {
            return Result<string>.Fail(validationError);
        }
        var shortUrlCode = ShortUrlHelper.GetCode(shortUrl);
        var originalUrl = urlDatastore.GetOriginalUrl(shortUrlCode);
        return WebUtility.UrlDecode(originalUrl) ?? Result<string>.Fail("Short URL is not recognized.");
    }

    public Result<bool> DeleteShortUrl(string shortUrl)
    {
        var validationError = UrlValidator.Shortened(shortUrl);
        if (validationError is not null)
        {
            return Result<bool>.Fail(validationError);
        }
        var shortUrlCode = ShortUrlHelper.GetCode(shortUrl);
        var res = urlDatastore.DeleteShortUrlCode(shortUrlCode);
        return Result<bool>.Success(res);
    }

    public Result<bool> DeleteAllShortUrlsByOriginalUrl(string longUrl)
    {
        var validationError = UrlValidator.Original(longUrl);
        if (validationError is not null)
        {
            return Result<bool>.Fail(validationError);
        }
        var res = urlDatastore.DeleteAllShortUrlsByLongUrl(WebUtility.UrlEncode(longUrl));
        return Result<bool>.Success(res);
    }

    public Result<int> GetClickCount(string shortUrl)
    {
        var validationError = UrlValidator.Shortened(shortUrl);
        if (validationError is not null)
        {
            return Result<int>.Fail(validationError);
        }
        var shortUrlCode = ShortUrlHelper.GetCode(shortUrl);
        var count = urlDatastore.GetClickCount(shortUrlCode);
        return count is not -1
            ? count
            : Result<int>.Fail("URL is not recognized.");
    }
}
