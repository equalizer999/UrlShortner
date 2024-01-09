using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Core;

/// <summary>
///     Defines the json file format for the datastore.
/// </summary>
public sealed class UrlDatastoreJsonFormat
{
    /// <summary>
    ///     A dictionary that maps long urls to a list of short urls.
    /// </summary>
    [JsonPropertyName("longToShortUrlMap")]
    public Dictionary<string, List<string>> LongToShortUrlMap { get; set; } = new();

    /// <summary>
    ///     A dictionary that maps short urls to long urls.
    /// </summary>
    [JsonPropertyName("shortToLongUrlMap")]
    public Dictionary<string, string> ShortToLongUrlMap { get; set; } = new();

    /// <summary>
    ///     A dictionary that maps short urls to click counts.
    /// </summary>
    [JsonPropertyName("shortUrlClickCountMap")]
    public Dictionary<string, int> ShortUrlClickCountMap { get; set; } = new();
}
