namespace NetworkStories.WebApi.Story;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using NetworkStories.Application.Story;
using NetworkStories.Application.Story.GetBestStories;

[ApiController]
[Route("[controller]")]
public class StoryController : ControllerBase
{
    private readonly IMediator _mediator;

    public StoryController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<MaybeActionResult<Story[]>> Get([FromQuery] GetBestStoriesQuery query)
    {
        return await _mediator.Send(query);
    }
}
