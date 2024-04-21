namespace NetworkStories.Infrastructure.HackerNews.Client;

public class HackerNewsItem
{
    public string Title { get; init; } = string.Empty;
    public string Url { get; init; } = string.Empty;
    public string By { get; init; } = string.Empty;
    public long Time { get; init; }
    public int Score { get; init; }
    public int Descendants { get; init; }
}
