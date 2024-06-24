using Microsoft.AspNetCore.Mvc;

namespace ResponseService.Controllers;

[ApiController]
[Route("api/v1/[Controller]")]
public class ResponseController : ControllerBase
{
    private readonly Random _random = new();

    // GET /api/response/33
    [Route("{id:int}")]
    [HttpGet]
    public ActionResult GetResponse(int id)
    {
        if (id <= _random.Next(1, 101))
        {
            Console.WriteLine($"{DateTime.Now} | Failure!");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        Console.WriteLine($"{DateTime.Now} | Success!");
        return Ok();
    }
}
