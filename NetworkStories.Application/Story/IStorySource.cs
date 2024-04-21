using FluentResults;

namespace NetworkStories.Application.Story;

public interface IStorySource
{
    public static Error Errored = new Error("Source errored");

    Task<Result<IEnumerable<Story>>> GetBestStories(int count);
}