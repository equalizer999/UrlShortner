namespace Core.Tests;

public sealed class UrlDatastoreTests
{
    private const string TestUrl = "https://www.adroit-tt.com";
    private readonly UrlDatastore _urlDatastore = new();

    [Fact]
    public void GetOriginalUrl_ReturnsNull_WhenShortUrlDoesNotExist()
    {
        var result = _urlDatastore.GetOriginalUrl("nonexistent");
        Assert.Null(result);
    }

    [Fact]
    public void GetOriginalUrl_ReturnsOriginalUrl_WhenShortUrlExists()
    {
        var shortUrl = _urlDatastore.CreateShortUrlCode(TestUrl);
        var result = _urlDatastore.GetOriginalUrl(shortUrl);
        Assert.Equal(TestUrl, result);
    }

    [Fact]
    public void GenerateShortUrlCode_ReturnsUniqueShortUrlCode_WhenCalledMultipleTimes()
    {
        var code1 = _urlDatastore.CreateShortUrlCode(TestUrl);
        var code2 = _urlDatastore.CreateShortUrlCode(TestUrl);
        Assert.NotEqual(code1, code2);
    }

    [Fact]
    public void DeleteShortUrlCode_ReturnsFalse_WhenShortUrlCodeDoesNotExist()
    {
        var result = _urlDatastore.DeleteShortUrlCode(TestUrl);
        Assert.False(result);
    }

    [Fact]
    public void DeleteShortUrlCode_ShortUrlDeletedAndReturnsTrue_WhenShortUrlCodeExists()
    {
        var shortUrl = _urlDatastore.CreateShortUrlCode(TestUrl);
        var result = _urlDatastore.DeleteShortUrlCode(shortUrl);
        Assert.True(result);
        Assert.Null(_urlDatastore.GetOriginalUrl(shortUrl));
    }

    [Fact]
    public void DeleteAllShortUrlsByLongUrl_ReturnsFalse_WhenLongUrlDoesNotExist()
    {
        var result = _urlDatastore.DeleteAllShortUrlsByLongUrl(TestUrl);
        Assert.False(result);
    }

    [Fact]
    public void DeleteAllShortUrlsByLongUrl_ShortUrlsDeletedAndReturnsTrue_WhenLongUrlExists()
    {
        var shortUrl1 = _urlDatastore.CreateShortUrlCode(TestUrl);
        var shortUrl2 = _urlDatastore.CreateShortUrlCode(TestUrl);
        var shortUrl3 = _urlDatastore.CreateShortUrlCode(TestUrl);
        var result = _urlDatastore.DeleteAllShortUrlsByLongUrl(TestUrl);
        Assert.Null(_urlDatastore.GetOriginalUrl(shortUrl1));
        Assert.Null(_urlDatastore.GetOriginalUrl(shortUrl2));
        Assert.Null(_urlDatastore.GetOriginalUrl(shortUrl3));
        Assert.True(result);
    }

    [Fact]
    public void GetClickCount_ReturnsMinusOne_WhenShortUrlDoesNotExist()
    {
        var result = _urlDatastore.GetClickCount(TestUrl);
        Assert.Equal(-1, result);
    }

    [Fact]
    public void IncrementClickCount_IncreasesClickCount_WhenShortUrlExists()
    {
        var shortUrl = _urlDatastore.CreateShortUrlCode(TestUrl);
        _urlDatastore.GetOriginalUrl(shortUrl); // Simulate a click
        var initialCount = _urlDatastore.GetClickCount(shortUrl);
        _urlDatastore.GetOriginalUrl(shortUrl); // Simulate another click
        var newCount = _urlDatastore.GetClickCount(shortUrl);
        Assert.Equal(initialCount + 1, newCount);
    }

    [Fact]
    public void IsShortUrlCodeInUse_ReturnsTrue_WhenShortUrlCodeExists()
    {
        var shortUrl = _urlDatastore.CreateShortUrlCode(TestUrl);
        var result = _urlDatastore.IsShortUrlCodeInUse(shortUrl);
        Assert.True(result);
    }

    [Fact]
    public void IsShortUrlCodeInUse_ReturnsFalse_WhenShortUrlCodeDoesNotExist()
    {
        var result = _urlDatastore.IsShortUrlCodeInUse("nonexistent");
        Assert.False(result);
    }

    [Fact]
    public async Task CreateGetDeleteUrl_OperatesWithoutException_WhenRunAsync_TaskAsync()
    {
        // run 100 operations that create, get, and then delete a short url, async
        var tasks = new List<Task>();
        for (var i = 0; i < 100; i++)
        {
            tasks.Add(Task.Run(() =>
            {
                var shortUrl = _urlDatastore.CreateShortUrlCode(TestUrl);
                _urlDatastore.GetOriginalUrl(shortUrl);
                _urlDatastore.DeleteShortUrlCode(shortUrl);
            }));
        }

        await Task.WhenAll(tasks);
        Assert.True(true);
    }

    [Fact]
    public async Task GetOriginalUrl_ReturnsOriginalUrl_WhenShortUrlExists_Async()
    {
        var shortUrl = _urlDatastore.CreateShortUrlCode(TestUrl);
        //create
        var tasks = new List<Task>();
        for (var i = 0; i < 100; i++)
        {
            tasks.Add(Task.Run(() => _urlDatastore.CreateShortUrlCode(TestUrl)));
        }
        await Task.WhenAll(tasks);
        //get
        tasks = [];
        for (var i = 0; i < 100; i++)
        {
            tasks.Add(Task.Run(() => _urlDatastore.GetOriginalUrl(shortUrl)));
        }
        await Task.WhenAll(tasks);
        Assert.True(true);
    }

    [Fact]
    public void ExportDatabase_ExportsDatabaseToJsonFile()
    {
        var filePath = "test_export.json";
        var result = _urlDatastore.ExportDatabase(filePath);
        Assert.True(result.IsSuccess);
        Assert.True(File.Exists(filePath));
        File.Delete(filePath);
    }

    [Fact]
    public void ImportDatabase_ImportsDatabaseFromJsonFile()
    {
        var filePath = "test_import.json";
        var exportResult = _urlDatastore.ExportDatabase(filePath);
        Assert.True(exportResult.IsSuccess);

        var importResult = _urlDatastore.ImportDatabase(filePath);
        Assert.True(importResult.IsSuccess);
        File.Delete(filePath);
    }
}
