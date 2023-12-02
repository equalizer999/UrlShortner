using Core.Constants;

namespace Core.Tests;

public sealed class ValidationTests
{
    [Theory]
    [InlineData("https://www.adroit-tt.com")]
    [InlineData("https://adroit-tt.com")]
    [InlineData("http://example.com")]
    [InlineData("https://example.com")]
    [InlineData("http://www.example.com")]
    [InlineData("https://www.example.com")]
    [InlineData("https://www.example.com/")]
    [InlineData("https://www.example.com/tinyurl")]
    public void ValidateOriginalUrl_ReturnsNull_WhenUrlIsValid(string url)
    {
        var result = UrlValidator.Original(url);
        Assert.Null(result);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void ValidateOriginalUrl_ReturnsEmptyUrlError_WhenUrlIsNullOrEmpty(string url)
    {
        var result = UrlValidator.Original(url);
        Assert.Equal(Errors.EmptyUrl, result);
    }

    [Theory]
    [InlineData("foo")]
    [InlineData("example.com")]
    public void ValidateOriginalUrl_ReturnsInvalidUrlError_WhenUrlIsNotWellFormed(string url)
    {
        var result = UrlValidator.Original(url);
        Assert.Equal(Errors.InvalidUrl, result);
    }

    [Theory]
    [InlineData($"{Globals.BaseUrl}example")]
    [InlineData($"{Globals.BaseUrl}tinyurl")]
    public void ValidateOriginalUrl_ReturnsUrlAlreadyShortenedError_WhenUrlIsShortened(string url)
    {
        var result = UrlValidator.Original(url);
        Assert.Equal(Errors.UrlAlreadyShortened, result);
    }

    [Theory]
    [InlineData($"{Globals.BaseUrl}example")]
    [InlineData($"{Globals.BaseUrl}tinyurl")]
    public void ValidateShortUrl_ReturnsNull_WhenUrlIsValid(string url)
    {
        var result = UrlValidator.Shortened(url);
        Assert.Null(result);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void ValidateShortUrl_ReturnsEmptyUrlError_WhenUrlIsNullOrEmpty(string url)
    {
        var result = UrlValidator.Shortened(url);
        Assert.Equal(Errors.EmptyUrl, result);
    }

    [Theory]
    [InlineData("foo")]
    [InlineData("example.com")]
    public void ValidateShortUrl_ReturnsInvalidUrlError_WhenUrlIsNotWellFormed(string url)
    {
        var result = UrlValidator.Shortened(url);
        Assert.Equal(Errors.InvalidUrl, result);
    }

    [Theory]
    [InlineData("https://example.com")]
    [InlineData("https://example.com/tinyurl")]
    public void ValidateShortUrl_ReturnsInvalidShortUrlError_WhenUrlIsNotShortened(string url)
    {
        var result = UrlValidator.Shortened(url);
        Assert.Equal(Errors.InvalidShortUrl, result);
    }
}
