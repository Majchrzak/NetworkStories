namespace NetworkStories.Application.Tests.HackerNews.Client;

using NetworkStories.Infrastructure.HackerNews.Client;
using NetworkStories.Application.Story;

public class HackerNewsItemMappingTests
{
    private readonly Fixtures _fixtures = new Fixtures();

    [Fact]
    public void HappyPath()
    {
        var item = _fixtures.GivenItem("hello-world", 123);

        var story = _fixtures.WhenMapped(item);

        Assert.Equivalent(_fixtures.GivenStory("hello-world", 123), story);
    }

    class Fixtures
    {
        public HackerNewsItem GivenItem(string title, int score)
        {
            return new HackerNewsItem
            {
                Title = title,
                Url = "dummy",
                By = "me",
                Time = 1,
                Score = score,
            };
        }

        public Story GivenStory(string title, int score)
        {
            return new Story
            {
                Title = title,
                Uri = "dummy",
                PostedBy = "me",
                Time = DateTimeOffset.FromUnixTimeSeconds(1),
                Score = score,
            };
        }

        public Story WhenMapped(HackerNewsItem item)
        {
            return item.ToStory();
        }
    }
}