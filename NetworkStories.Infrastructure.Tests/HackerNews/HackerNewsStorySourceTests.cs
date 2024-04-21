namespace NetworkStories.Application.Tests.HackerNews;

using FluentResults;
using Microsoft.Extensions.Logging;
using Moq;
using NetworkStories.Application.Story;
using NetworkStories.Infrastructure.HackerNews;
using NetworkStories.Infrastructure.HackerNews.Client;
using Xunit;

public class HackerNewsStorySourceTests
{
    private readonly Fixtures _fixtures = new Fixtures();

    [Fact]
    public async Task HappyPath()
    {
        _fixtures.GivenClientIndex([1, 2, 3, 4]);
        _fixtures.GivenClientItem(1, _fixtures.GivenItem("item-1", 1));
        _fixtures.GivenClientItem(2, _fixtures.GivenItem("item-2", 2));
        _fixtures.GivenClientItem(3, _fixtures.GivenItem("item-3", 3));
        _fixtures.GivenClientItem(4, _fixtures.GivenItem("item-4", 4));

        var result = await _fixtures.WhenBestStories(3);

        Assert.True(result.IsSuccess);
        Assert.Collection(result.Value,
            (it) => Assert.Equivalent(it, _fixtures.GivenStory("item-1", 1)),
            (it) => Assert.Equivalent(it, _fixtures.GivenStory("item-2", 2)),
            (it) => Assert.Equivalent(it, _fixtures.GivenStory("item-3", 3))
        );
    }

    [Fact]
    public async Task DoNotMindAboutErroredItems()
    {
        _fixtures.GivenClientIndex([1, 2, 3, 4]);
        _fixtures.GivenClientItem(1, _fixtures.GivenItem("item-1", 1));
        _fixtures.GivenClientItemErrored(2);
        _fixtures.GivenClientItem(3, _fixtures.GivenItem("item-3", 3));
        _fixtures.GivenClientItem(4, _fixtures.GivenItem("item-4", 4));

        var result = await _fixtures.WhenBestStories(3);

        Assert.True(result.IsSuccess);
        Assert.Collection(result.Value,
            (it) => Assert.Equivalent(it, _fixtures.GivenStory("item-1", 1)),
            (it) => Assert.Equivalent(it, _fixtures.GivenStory("item-3", 3))
        );
    }

    class Fixtures
    {
        private readonly Mock<IHackerNewsClient> _client = new Mock<IHackerNewsClient>();
        private readonly Mock<ILogger<HackerNewsStorySource>> _logger = new Mock<ILogger<HackerNewsStorySource>>();
        private readonly HackerNewsStorySource _sut;

        public Fixtures()
        {
            _sut = new HackerNewsStorySource(_client.Object);
        }

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

        public void GivenClientIndex(int[] ids)
        {
            _client.Setup(it => it.GetIndex())
                .ReturnsAsync(ids);
        }

        public void GivenClientItem(int id, HackerNewsItem item)
        {
            _client.Setup(it => it.GetItem(It.Is<int>((value) => value == id)))
                .ReturnsAsync(item);
        }

        public void GivenClientItemErrored(int id)
        {
            _client.Setup(it => it.GetItem(It.Is<int>((value) => value == id)))
                .ReturnsAsync(IStorySource.Errored);
        }

        public Task<Result<IEnumerable<Story>>> WhenBestStories(int count)
        {
            return _sut.GetBestStories(count);
        }
    }
}