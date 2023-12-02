using System.Diagnostics;
using Core.Constants;
using Xunit.Abstractions;

namespace Core.Tests;

public sealed class ShortUrlHelperTests(ITestOutputHelper testOutputHelper)
{
    [Fact]
    public void GetRandomCode_ReturnsRandomString()
    {
        var randomString = ShortUrlHelper.GetRandomCode();
        Debug.WriteLine(randomString);
        Assert.Equal(8, randomString.Length);
    }

    [Fact]
    public void GetRandomCode_ReturnsConsistentLength()
    {
        var length = ShortUrlHelper.GetRandomCode().Length;

        for (var i = 0; i < 10000; i++)
        {
            var randomString = ShortUrlHelper.GetRandomCode();
            testOutputHelper.WriteLine(randomString);
            Assert.Equal(length, randomString.Length);
        }
    }

    [Fact]
    public void GetRandomCode_ReturnsStringOfDefaultLength()
    {
        var result = ShortUrlHelper.GetRandomCode();
        Assert.Equal(8, result.Length);
    }

    [Fact]
    public void GetRandomCode_ReturnsUniqueValuesOnMultipleCalls()
    {
        var code1 = ShortUrlHelper.GetRandomCode();
        var code2 = ShortUrlHelper.GetRandomCode();
        Assert.NotEqual(code1, code2);
    }

    [Theory]
    [InlineData($"{Globals.BaseUrl}abcd1234")]
    public void GetCode_ReturnsCorrectCode_ShortUrlWithBase(string shortUrl)
    {
        var result = ShortUrlHelper.GetCode(shortUrl);
        Assert.Equal("abcd1234", result);
    }

    [Theory]
    [InlineData("code1234", $"{Globals.BaseUrl}code1234")]
    [InlineData("", Globals.BaseUrl)]
    public void BuildShortUrl_ReturnsExpectedShortUrl(string code, string expectedUrl)
    {
        var result = ShortUrlHelper.BuildShortUrl(code);
        Assert.Equal(expectedUrl, result);
    }

    [Theory]
    [InlineData(1000000)]
    public void GetRandomCode_LowCollisionRateSmokeTest(int numberOfTests)
    {
        var codes = new HashSet<string>();
        var duplicates = 0;

        for (var i = 0; i < numberOfTests; i++)
        {
            var code = ShortUrlHelper.GetRandomCode();
            if (!codes.Add(code))
            {
                duplicates++;
            }
        }

        var collisionRate = (double)duplicates / numberOfTests;
        testOutputHelper.WriteLine($"Collisions: {duplicates} out of {numberOfTests} ({collisionRate:P})");

        Assert.True(collisionRate < 0.0001, "Collision rate is higher than expected.");
    }

    [Theory]
    [InlineData("abcd1234")]
    [InlineData("ABCD1234")]
    [InlineData("aBcD1234")]
    public void ValidateCode_ReturnsTrue_WhenCodeIsValid(string code)
    {
        var result = ShortUrlHelper.ValidateCode(code);
        Assert.True(result);
    }

    [Theory]
    [InlineData("abcd123")]
    [InlineData("abcd12345")]
    [InlineData("abcd123!")]
    [InlineData("abcd1234 ")]
    [InlineData(" abcd1234")]
    [InlineData("abci1234")]
    [InlineData("abcdl234")]
    [InlineData("abcdO234")]
    [InlineData("abcd0234")]
    public void ValidateCode_ReturnsFalse_WhenCodeIsInvalid(string code)
    {
        var result = ShortUrlHelper.ValidateCode(code);
        Assert.False(result);
    }
}
