using Microsoft.Extensions.DependencyInjection;
using NetworkStories.Application.Story;
using NetworkStories.Infrastructure.HackerNews;
using NetworkStories.Infrastructure.HackerNews.Client;

namespace NetworkStories.Infrastructure;

public static class InfrastructureModule
{
    public static void AddInfrastructure(this IServiceCollection @this)
    {
        @this.AddTransient<IStorySource, HackerNewsStorySource>()
            .AddSingleton<IHackerNewsClient, HackerNewsClient>()
            .AddMemoryCache();
    }
}