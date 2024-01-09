using System.IO;
using Core;
using Moq;
using Xunit;

namespace Services.Tests;

public sealed class UrlServiceTests
{
    private const string TestUrl = "https://www.adroit-tt.com";
    private readonly Mock<IUrlDatastore> _mockDatastore = new();
    private readonly UrlService _urlService;

    public UrlServiceTests()
    {
        _urlService = new UrlService(_mockDatastore.Object);
    }

    [Fact]
    public void ShortenUrl_ReturnsFailResult_WhenLongUrlIsEmpty()
    {
        var result = _urlService.ShortenUrl(string.Empty);
        Assert.False(result.IsSuccess);
        Assert.Equal(Errors.EmptyUrl, result.ErrorMessage);
    }

    [Fact]
    public void ShortenUrl_ReturnsFailResult_WhenLongUrlIsInvalid()
    {
        var result = _urlService.ShortenUrl("foo");
        Assert.False(result.IsSuccess);
        Assert.Equal(Errors.InvalidUrl, result.ErrorMessage);
    }

    [Fact]
    public void ShortenUrl_ReturnsFailResult_WhenLongUrlIsShortened()
    {
        var result = _urlService.ShortenUrl("https://tinyurl.com/abcd1234");
        Assert.False(result.IsSuccess);
        Assert.Equal(Errors.UrlAlreadyShortened, result.ErrorMessage);
    }

    [Fact]
    public void ShortenUrl_ReturnsFailResult_WhenCustomCodeIsInvalid()
    {
        var result = _urlService.ShortenUrl(TestUrl, "foo");
        Assert.False(result.IsSuccess);
        Assert.Contains("Short URL code is invalid", result.ErrorMessage);
    }

    [Fact]
    public void ShortenUrl_ReturnsFailResult_WhenCustomCodeIsInUse()
    {
        _mockDatastore.Setup(ds => ds.IsShortUrlCodeInUse("abcd1234")).Returns(true);
        var result = _urlService.ShortenUrl(TestUrl, "abcd1234");
        Assert.False(result.IsSuccess);
        Assert.Equal("Short URL code is already in use.", result.ErrorMessage);
    }

    [Fact]
    public void ShortenUrl_ReturnsSuccessResult_WhenLongUrlIsValid()
    {
        _mockDatastore.Setup(ds => ds.CreateShortUrlCode(TestUrl, null)).Returns("abcd1234");
        var result = _urlService.ShortenUrl(TestUrl);
        Assert.True(result.IsSuccess);
        Assert.Equal("https://tinyurl.com/abcd1234", result.Value);
    }

    [Fact]
    public void ShortenUrl_ReturnsSuccessResult_WhenLongUrlAndCustomCodeAreValid()
    {
        _mockDatastore.Setup(ds => ds.CreateShortUrlCode(TestUrl, "abcd1234")).Returns("abcd1234");
        var result = _urlService.ShortenUrl(TestUrl, "abcd1234");
        Assert.True(result.IsSuccess);
        Assert.Equal("https://tinyurl.com/abcd1234", result.Value);
    }

    [Fact]
    public void GetOriginalUrl_ReturnsFailResult_WhenShortUrlIsEmpty()
    {
        var result = _urlService.GetOriginalUrl(string.Empty);
        Assert.False(result.IsSuccess);
        Assert.Equal(Errors.EmptyUrl, result.ErrorMessage);
    }

    [Fact]
    public void GetOriginalUrl_ReturnsFailResult_WhenShortUrlIsInvalid()
    {
        var result = _urlService.GetOriginalUrl("foo");
        Assert.False(result.IsSuccess);
        Assert.Equal(Errors.InvalidUrl, result.ErrorMessage);
    }

    [Fact]
    public void GetOriginalUrl_ReturnsFailResult_WhenShortUrlIsNotShortened()
    {
        var result = _urlService.GetOriginalUrl("https://example.com");
        Assert.False(result.IsSuccess);
        Assert.Equal(Errors.InvalidShortUrl, result.ErrorMessage);
    }

    [Fact]
    public void GetOriginalUrl_ReturnsFailResult_WhenShortUrlIsNotRecognized()
    {
        _mockDatastore.Setup(ds => ds.GetOriginalUrl("abcd1234")).Returns((string?)null);
        var result = _urlService.GetOriginalUrl("https://tinyurl.com/abcd1234");
        Assert.False(result.IsSuccess);
        Assert.Equal("Short URL is not recognized.", result.ErrorMessage);
    }

    [Fact]
    public void GetOriginalUrl_ReturnsSuccessResult_WhenShortUrlIsValid()
    {
        _mockDatastore.Setup(ds => ds.GetOriginalUrl("abcd1234")).Returns(TestUrl);
        var result = _urlService.GetOriginalUrl("https://tinyurl.com/abcd1234");
        Assert.True(result.IsSuccess);
        Assert.Equal(TestUrl, result.Value);
    }

    [Fact]
    public void DeleteShortUrl_ReturnsFailResult_WhenShortUrlIsEmpty()
    {
        var result = _urlService.DeleteShortUrl(string.Empty);
        Assert.False(result.IsSuccess);
        Assert.Equal(Errors.EmptyUrl, result.ErrorMessage);
    }

    [Fact]
    public void DeleteShortUrl_ReturnsFailResult_WhenShortUrlIsInvalid()
    {
        var result = _urlService.DeleteShortUrl("foo");
        Assert.False(result.IsSuccess);
        Assert.Equal(Errors.InvalidUrl, result.ErrorMessage);
    }

    [Fact]
    public void DeleteShortUrl_ReturnsFailResult_WhenShortUrlIsNotShortened()
    {
        var result = _urlService.DeleteShortUrl("https://example.com");
        Assert.False(result.IsSuccess);
        Assert.Equal(Errors.InvalidShortUrl, result.ErrorMessage);
    }

    [Fact]
    public void DeleteShortUrl_ReturnsSuccessResult_WhenShortUrlIsValid()
    {
        _mockDatastore.Setup(ds => ds.DeleteShortUrlCode("abcd1234")).Returns(true);
        var result = _urlService.DeleteShortUrl("https://tinyurl.com/abcd1234");
        Assert.True(result.IsSuccess);
        Assert.True(result.Value);
    }

    [Fact]
    public void DeleteAllShortUrlsByOriginalUrl_ReturnsFailResult_WhenLongUrlIsEmpty()
    {
        var result = _urlService.DeleteAllShortUrlsByOriginalUrl(string.Empty);
        Assert.False(result.IsSuccess);
        Assert.Equal(Errors.EmptyUrl, result.ErrorMessage);
    }

    [Fact]
    public void DeleteAllShortUrlsByOriginalUrl_ReturnsFailResult_WhenLongUrlIsInvalid()
    {
        var result = _urlService.DeleteAllShortUrlsByOriginalUrl("foo");
        Assert.False(result.IsSuccess);
        Assert.Equal(Errors.InvalidUrl, result.ErrorMessage);
    }

    [Fact]
    public void DeleteAllShortUrlsByOriginalUrl_ReturnsFailResult_WhenLongUrlIsShortened()
    {
        var result = _urlService.DeleteAllShortUrlsByOriginalUrl("https://tinyurl.com/abcd1234");
        Assert.False(result.IsSuccess);
        Assert.Equal(Errors.UrlAlreadyShortened, result.ErrorMessage);
    }

    [Fact]
    public void DeleteAllShortUrlsByOriginalUrl_ReturnsSuccessResult_WhenLongUrlIsValid()
    {
        _mockDatastore.Setup(ds => ds.DeleteAllShortUrlsByLongUrl(TestUrl)).Returns(true);
        var result = _urlService.DeleteAllShortUrlsByOriginalUrl(TestUrl);
        Assert.True(result.IsSuccess);
        Assert.True(result.Value);
    }

    [Fact]
    public void GetClickCount_ReturnsFailResult_WhenShortUrlIsEmpty()
    {
        var result = _urlService.GetClickCount(string.Empty);
        Assert.False(result.IsSuccess);
        Assert.Equal(Errors.EmptyUrl, result.ErrorMessage);
    }

    [Fact]
    public void GetClickCount_ReturnsFailResult_WhenShortUrlIsInvalid()
    {
        var result = _urlService.GetClickCount("foo");
        Assert.False(result.IsSuccess);
        Assert.Equal(Errors.InvalidUrl, result.ErrorMessage);
    }

    [Fact]
    public void GetClickCount_ReturnsFailResult_WhenShortUrlIsNotShortened()
    {
        var result = _urlService.GetClickCount("https://example.com");
        Assert.False(result.IsSuccess);
        Assert.Equal(Errors.InvalidShortUrl, result.ErrorMessage);
    }

    [Fact]
    public void GetClickCount_ReturnsFailResult_WhenShortUrlIsNotRecognized()
    {
        _mockDatastore.Setup(ds => ds.GetClickCount("abcd1234")).Returns(-1);
        var result = _urlService.GetClickCount("https://tinyurl.com/abcd1234");
        Assert.False(result.IsSuccess);
        Assert.Equal("URL is not recognized.", result.ErrorMessage);
    }

    [Fact]
    public void GetClickCount_ReturnsSuccessResult_WhenShortUrlIsValid()
    {
        _mockDatastore.Setup(ds => ds.GetClickCount("abcd1234")).Returns(42);
        var result = _urlService.GetClickCount("https://tinyurl.com/abcd1234");
        Assert.True(result.IsSuccess);
        Assert.Equal(42, result.Value);
    }

    // Added new test
    [Fact]
    public void ExportDatastore_ReturnsFailResult_WhenFilePathIsEmpty()
    {
        var result = _urlService.ExportDatastore(string.Empty);
        Assert.False(result.IsSuccess);
        Assert.Equal("File path cannot be empty.", result.ErrorMessage);
    }

    // Added new test
    [Fact]
    public void ExportDatastore_ReturnsFailResult_WhenDatastoreThrowsException()
    {
        _mockDatastore.Setup(ds => ds.ExportDatastore(It.IsAny<string>())).Throws(new IOException("File error"));
        var result = _urlService.ExportDatastore("test.json");
        Assert.False(result.IsSuccess);
        Assert.Equal("File error", result.ErrorMessage);
    }

    // Added new test
    [Fact]
    public void ExportDatastore_ReturnsSuccessResult_WhenFilePathIsValid()
    {
        _mockDatastore.Setup(ds => ds.ExportDatastore(It.IsAny<string>()));
        var result = _urlService.ExportDatastore("test.json");
        Assert.True(result.IsSuccess);
        Assert.True(result.Value);
    }

    // Added new test
    [Fact]
    public void ImportDatastore_ReturnsFailResult_WhenFilePathIsEmpty()
    {
        var result = _urlService.ImportDatastore(string.Empty);
        Assert.False(result.IsSuccess);
        Assert.Equal("File path cannot be empty.", result.ErrorMessage);
    }

    // Added new test
    [Fact]
    public void ImportDatastore_ReturnsFailResult_WhenFileDoesNotExist()
    {
        var result = _urlService.ImportDatastore("test.json");
        Assert.False(result.IsSuccess);
        Assert.Equal("File does not exist.", result.ErrorMessage);
    }

    // Added new test
    [Fact]
    public void ImportDatastore_ReturnsFailResult_WhenDatastoreThrowsException()
    {
        File.WriteAllText("test.json", "invalid json");
        _mockDatastore.Setup(ds => ds.ImportDatastore(It.IsAny<string>())).Throws(new JsonException("Json error"));
        var result = _urlService.ImportDatastore("test.json");
        Assert.False(result.IsSuccess);
        Assert.Equal("Json error", result.ErrorMessage);
        File.Delete("test.json");
    }

    // Added new test
    [Fact]
    public void ImportDatastore_ReturnsSuccessResult_WhenFileIsValid()
    {
        File.WriteAllText("test.json", "valid json");
        _mockDatastore.Setup(ds => ds.ImportDatastore(It.IsAny<string>()));
        var result = _urlService.ImportDatastore("test.json");
        Assert.True(result.IsSuccess);
        Assert.True(result.Value);
        File.Delete("test.json");
    }
}
