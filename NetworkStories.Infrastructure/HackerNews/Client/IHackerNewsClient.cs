using FluentResults;

namespace NetworkStories.Infrastructure.HackerNews.Client;

public interface IHackerNewsClient
{
    Task<Result<IEnumerable<int>>> GetIndex();

    Task<Result<HackerNewsItem>> GetItem(int id);
}