using FluentResults;
using MediatR;

namespace NetworkStories.Application.Story.GetBestStories;

public class GetBestStoriesQuery : IRequest<Result<Story[]>>
{
    public static Error InvalidSourceError = new Error("Invalid source");

    public int Count { get; set; }
}
