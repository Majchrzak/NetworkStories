using Firebase.Database;
using Firebase.Database.Query;
using FluentResults;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetworkStories.Application.Story;
using Polly;
using Polly.Retry;

namespace NetworkStories.Infrastructure.HackerNews.Client;

public class HackerNewsClient : IHackerNewsClient
{
    private readonly HackerNewsOptions _options;
    private readonly FirebaseClient _client;
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger _logger;

    private readonly SemaphoreSlim _indexLimiter = new SemaphoreSlim(1, 1);
    private readonly SemaphoreSlim _itemLimiter = new SemaphoreSlim(3, 3);
    private readonly AsyncRetryPolicy _policy;

    public HackerNewsClient(
        IOptions<HackerNewsOptions> options,
        IMemoryCache memoryCache,
        ILogger<HackerNewsClient> logger
    )
    {
        _options = options.Value;
        _client = new FirebaseClient(_options.Url);
        _memoryCache = memoryCache;
        _logger = logger;

        _policy = Policy
              .Handle<Exception>()
              .WaitAndRetryAsync(_options.RetryPolicyCount, _ => _options.RetryPolicyTimeout);
    }

    public async Task<Result<IEnumerable<int>>> GetIndex()
    {
        await _indexLimiter.WaitAsync();

        try
        {
            var index = await _memoryCache.GetOrCreateAsync<IEnumerable<int>>("hn-index", async (entry) =>
            {
                entry.AbsoluteExpirationRelativeToNow = _options.CacheAbsoluteExpiration;

                _logger.LogInformation("Fetching new HackerNews index");

                return await _policy.ExecuteAsync(() =>
                {
                    return _client.Child("beststories.json?print=pretty")
                        .OnceSingleAsync<int[]>();
                });
            });

            if (index is null)
            {
                throw new Exception($"Missing index");
            }

            return Result.Ok(index);
        }
        catch (Exception e)
        {
            _logger.LogError($"Unable to get index: {e}");

            return IStorySource.Errored;
        }
        finally
        {
            _indexLimiter.Release();
        }
    }

    public async Task<Result<HackerNewsItem>> GetItem(int id)
    {
        await _itemLimiter.WaitAsync();

        try
        {
            var item = await _memoryCache.GetOrCreateAsync($"hn-item-{id}", async (entry) =>
            {
                entry.AbsoluteExpirationRelativeToNow = _options.CacheAbsoluteExpiration;

                _logger.LogInformation($"Fetching new HackerNews item: {id}");

                return await _policy.ExecuteAsync(() =>
                {
                    return _client.Child("item").Child(id.ToString())
                        .Child(".json?print=pretty").OnceSingleAsync<HackerNewsItem>();
                });
            });

            if (item is null)
            {
                throw new Exception($"Missing item: {id}");
            }

            return Result.Ok(item);
        }
        catch (Exception e)
        {
            _logger.LogError($"Unable to get item: {e}");

            return IStorySource.Errored;
        }
        finally
        {
            _itemLimiter.Release();
        }
    }
}