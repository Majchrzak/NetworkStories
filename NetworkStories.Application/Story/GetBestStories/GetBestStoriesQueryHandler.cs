using FluentResults;
using MediatR;

namespace NetworkStories.Application.Story.GetBestStories;

public class GetBestStoriesQueryHandler : IRequestHandler<GetBestStoriesQuery, Result<Story[]>>
{
    private readonly IStorySource _source;

    public GetBestStoriesQueryHandler(IStorySource source)
    {
        _source = source;
    }

    public async Task<Result<Story[]>> Handle(GetBestStoriesQuery request, CancellationToken cancellationToken)
    {
        var stories = await _source.GetBestStories(request.Count);
        if (stories.IsFailed)
        {
            return GetBestStoriesQuery.InvalidSourceError;
        }

        return stories.Value.OrderByDescending(it => it.Score)
            .Take(request.Count)
            .ToArray();
    }
}