namespace Core;

public interface IUrlDatastore
{
    /// <summary>
    ///     Retrieves the original URL associated with a shortened URL and increments the click count.
    /// </summary>
    string? GetOriginalUrl(string shortUrl);

    /// <summary>
    ///     Stores a randomly generated short URL code, or a custom one if provided, and associates it with a long URL.
    /// </summary>
    string CreateShortUrlCode(string longUrl, string? customCode = null);

    /// <summary>
    ///     Deletes a short URL and its associated long URL and statistics.
    /// </summary>
    bool DeleteShortUrlCode(string shortUrlCode);

    /// <summary>
    ///     Deletes all short URLs and statistics associated with a long URL.
    /// </summary>
    bool DeleteAllShortUrlsByLongUrl(string longUrl);

    /// <summary>
    ///     Retrieves the number of times a shortened URL has been clicked.
    /// </summary>
    int GetClickCount(string shortUrl);

    /// <summary>
    ///     Checks if a short URL code is already in use.
    /// </summary>
    public bool IsShortUrlCodeInUse(string shortUrlCode);

    // Added new method
    /// <summary>
    ///     Serializes the datastore to a json file.
    /// </summary>
    void ExportDatastore(string filePath);

    // Added new method
    /// <summary>
    ///     Deserializes a json file and updates the datastore.
    /// </summary>
    void ImportDatastore(string filePath);
}
