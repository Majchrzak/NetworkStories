using Microsoft.Extensions.DependencyInjection;
using NetworkStories.Application.Story.GetBestStories;

namespace NetworkStories.Application;

public static class ApplicationModule
{
    public static void AddApplication(this IServiceCollection @this)
    {
        @this.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<GetBestStoriesQuery>());
    }
}