namespace NetworkStories.Infrastructure.HackerNews;

public class HackerNewsOptions
{
    public string Url { get; set; } = string.Empty;
    public TimeSpan CacheAbsoluteExpiration { get; set; } = TimeSpan.FromSeconds(60);
    public int RetryPolicyCount { get; set; } = 3;
    public TimeSpan RetryPolicyTimeout { get; set; } = TimeSpan.FromSeconds(1);
}