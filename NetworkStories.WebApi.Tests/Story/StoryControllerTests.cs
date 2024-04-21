namespace NetworkStories.WebApi.Tests.Story;

using System.Net;
using System.Text.Json;
using FluentResults;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using NetworkStories.Application.Story;
using Xunit;

public class StoryControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly Fixtures _fixtures;

    public StoryControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _fixtures = new Fixtures(factory);
    }

    [Fact]
    public async Task Get_HappyPath()
    {
        _fixtures.GivenStorySourceResult(new Story[] {
            _fixtures.GivenStory("story-1", 1),
            _fixtures.GivenStory("story-2", 2),
            _fixtures.GivenStory("story-3", 3),
            _fixtures.GivenStory("story-4", 4),
        });

        var response = await _fixtures.WhenGetStories(3);

        _fixtures.ThenResponseStatusCode(response, HttpStatusCode.OK);
        await _fixtures.ThenResponseSucceed(response, [
            _fixtures.GivenStory("story-4", 4),
            _fixtures.GivenStory("story-3", 3),
            _fixtures.GivenStory("story-2", 2)
        ]);
    }

    [Fact]
    public async Task Get_StorySource_Fails()
    {
        _fixtures.GivenStorySourceResult(Result.Fail("unknown-error"));

        var response = await _fixtures.WhenGetStories(3);

        _fixtures.ThenResponseStatusCode(response, HttpStatusCode.InternalServerError);
    }
}

class Fixtures
{
    private readonly Mock<IStorySource> _storySource = new Mock<IStorySource>();
    private readonly HttpClient _client;

    public Fixtures(CustomWebApplicationFactory<Program> factory)
    {
        _storySource = factory.StorySource;
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
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

    public void GivenStorySourceResult(Result<IEnumerable<Story>> result)
    {
        _storySource.Setup(it => it.GetBestStories(It.IsAny<int>()))
            .ReturnsAsync(result);
    }

    public async Task<HttpResponseMessage> WhenGetStories(int count)
    {
        return await _client.GetAsync($"/story?count={count}");
    }

    public async Task ThenResponseSucceed(HttpResponseMessage response, Story[] stories)
    {
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<Story[]>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.False(result is null);

        for (var i = 0; i < stories.Length; i++)
        {
            Assert.Equivalent(stories[i], result[i]);
        }
    }

    public void ThenResponseStatusCode(HttpResponseMessage response, HttpStatusCode statusCode)
    {
        Assert.Equal(statusCode, response.StatusCode);
    }
}