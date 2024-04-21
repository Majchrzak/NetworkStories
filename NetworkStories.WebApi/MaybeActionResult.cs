using FluentResults;
using Microsoft.AspNetCore.Mvc;

namespace NetworkStories.WebApi;

public class MaybeActionResult<T> : IActionResult
{
    public readonly Result<T> Result;

    public MaybeActionResult(Result<T> result)
    {
        Result = result;
    }

    public static implicit operator MaybeActionResult<T>(Result<T> result)
        => new MaybeActionResult<T>(result);

    public async Task ExecuteResultAsync(ActionContext context)
    {
        IActionResult result = Result switch
        {
            { IsSuccess: true } => new OkObjectResult(Result.Value),
            _ => new StatusCodeResult(StatusCodes.Status500InternalServerError)
        };

        await result.ExecuteResultAsync(context);
    }
}