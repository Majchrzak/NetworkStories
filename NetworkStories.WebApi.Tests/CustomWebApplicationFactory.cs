using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using NetworkStories.Application.Story;
using Microsoft.Extensions.DependencyInjection;

namespace NetworkStories.WebApi.Tests;

public class CustomWebApplicationFactory<TProgram>
    : WebApplicationFactory<TProgram> where TProgram : class
{
    public Mock<IStorySource> StorySource { get; } = new Mock<IStorySource>();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var storySource = services.SingleOrDefault(it => it.ServiceType == typeof(IStorySource));
            if (storySource is not null)
            {
                services.Remove(storySource);
            }

            services.AddSingleton(StorySource.Object);
        });

        builder.UseEnvironment("Development");
    }
}