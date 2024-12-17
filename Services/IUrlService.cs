using Core;

namespace Services;

/// <summary>
///     Handles URL shortening related services using an <see cref="IUrlDatastore" />.
/// </summary>
public interface IUrlService
{
    /// <summary>
    ///     Returns a short URL for a long URL.
    /// </summary>
    Result<string> ShortenUrl(string longUrl, string? customCode = null);

    /// <summary>
    ///     Returns the long URL for a short URL.
    /// </summary>
    Result<string> GetOriginalUrl(string shortUrl);

    /// <summary>
    ///     Deletes a short URL.
    /// </summary>
    Result<bool> DeleteShortUrl(string shortUrl);

    /// <summary>
    ///     Deletes all short URLs associated with a long URL.
    /// </summary>
    Result<bool> DeleteAllShortUrlsByOriginalUrl(string longUrl);

    /// <summary>
    ///     Gets the number of clicks for a short URL.
    /// </summary>
    Result<int> GetClickCount(string shortUrl);

    /// <summary>
    ///     Exports the database to a JSON file.
    /// </summary>
    Result<string> ExportDatabase(string filePath);

    /// <summary>
    ///     Imports the database from a JSON file.
    /// </summary>
    Result<string> ImportDatabase(string filePath);
}
