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

    // Added new method
    /// <summary>
    ///     Exports the datastore to a json file.
    /// </summary>
    Result<bool> ExportDatastore(string filePath);

    // Added new method
    /// <summary>
    ///     Imports the datastore from a json file.
    /// </summary>
    Result<bool> ImportDatastore(string filePath);
}
