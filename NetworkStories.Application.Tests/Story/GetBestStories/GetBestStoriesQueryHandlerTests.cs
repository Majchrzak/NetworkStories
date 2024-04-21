namespace NetworkStories.Application.Tests.Story.GetBestStories;

using FluentResults;
using Moq;
using NetworkStories.Application.Story;
using NetworkStories.Application.Story.GetBestStories;
using Xunit;

public class GetBestStoriesQueryHandlerTests
{
    private readonly Fixtures _fixtures = new Fixtures();

    [Fact]
    public async Task HappyPath()
    {
        _fixtures.GivenBestStories([
            _fixtures.GivenStory("story-3", 3),
            _fixtures.GivenStory("story-1", 1),
            _fixtures.GivenStory("story-4", 4),
            _fixtures.GivenStory("story-2", 2),
        ]);

        var query = _fixtures.GivenQuery(3);
        var result = await _fixtures.WhenRequesting(query);

        Assert.True(result.IsSuccess);
        Assert.Collection(result.Value,
            (it) => Assert.Equivalent(it, _fixtures.GivenStory("story-4", 4)),
            (it) => Assert.Equivalent(it, _fixtures.GivenStory("story-3", 3)),
            (it) => Assert.Equivalent(it, _fixtures.GivenStory("story-2", 2))
        );
    }

    [Fact]
    public async Task InvalidSourceError()
    {
        _fixtures.GivenBestStoriesFails();

        var query = _fixtures.GivenQuery(3);
        var result = await _fixtures.WhenRequesting(query);

        Assert.True(result.IsFailed);
        Assert.True(result.HasError(e => e == GetBestStoriesQuery.InvalidSourceError));
    }

    class Fixtures
    {
        private readonly Mock<IStorySource> _source = new Mock<IStorySource>();

        public Story GivenStory(string title, int score)
        {
            return new Story
            {
                Title = title,
                Uri = "dummy",
                PostedBy = "me",
                Time = DateTimeOffset.FromUnixTimeSeconds(0),
                Score = score,
            };
        }

        public void GivenBestStories(Story[] stories)
        {
            _source.Setup(it => it.GetBestStories(It.IsAny<int>()))
                .ReturnsAsync(stories);
        }

        public void GivenBestStoriesFails()
        {
            _source.Setup(it => it.GetBestStories(It.IsAny<int>()))
                .ReturnsAsync(GetBestStoriesQuery.InvalidSourceError);
        }

        public GetBestStoriesQuery GivenQuery(int count)
        {
            return new GetBestStoriesQuery
            {
                Count = count
            };
        }

        public Task<Result<Story[]>> WhenRequesting(GetBestStoriesQuery query)
        {
            return new GetBestStoriesQueryHandler(_source.Object)
                .Handle(query, CancellationToken.None);
        }
    }
}