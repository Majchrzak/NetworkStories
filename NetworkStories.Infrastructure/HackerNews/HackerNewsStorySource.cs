using FluentResults;
using NetworkStories.Application.Story;
using NetworkStories.Infrastructure.HackerNews.Client;

namespace NetworkStories.Infrastructure.HackerNews;

public class HackerNewsStorySource : IStorySource
{
    private readonly IHackerNewsClient _client;

    public HackerNewsStorySource(IHackerNewsClient client)
    {
        _client = client;
    }

    public async Task<Result<IEnumerable<Story>>> GetBestStories(int count)
    {
        var index = await _client.GetIndex();
        if (index.IsFailed)
        {
            return IStorySource.Errored;
        }

        var stories = await Task.WhenAll(index.Value.Take(count)
            .Select(GetStory));

        return Result.Ok(stories.Where(it => it.IsSuccess)
            .Select(it => it.Value)
            .Take(count));
    }

    private async Task<Result<Story>> GetStory(int id)
    {
        var item = await _client.GetItem(id);
        if (item.IsFailed)
        {
            return IStorySource.Errored;
        }

        return item.Value.ToStory();
    }
}
