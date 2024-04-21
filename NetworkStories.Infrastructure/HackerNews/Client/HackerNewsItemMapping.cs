using NetworkStories.Application.Story;

namespace NetworkStories.Infrastructure.HackerNews.Client;

public static class HackerNewsItemMapping
{
    public static Story ToStory(this HackerNewsItem @this)
    {
        return new Story
        {
            Title = @this.Title,
            Uri = @this.Url,
            PostedBy = @this.By,
            Time = DateTimeOffset.FromUnixTimeSeconds(@this.Time),
            Score = @this.Score,
            CommentCount = @this.Descendants
        };
    }
}