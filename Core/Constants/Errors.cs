namespace Core.Constants;

public static class Errors
{
    public const string EmptyInput = "Input was empty.";
    public const string EmptyUrl = "URL cannot be empty.";
    public const string UrlTooLarge = "URL is too large.";
    public const string InvalidUrl = "Invalid URL. Please provide an absolute URL (e.g. https://example.com).";
    public const string InvalidShortUrl = "Invalid short URL.";
    public const string UrlAlreadyShortened = "Invalid URL. Provided URL is shortened.";
}
