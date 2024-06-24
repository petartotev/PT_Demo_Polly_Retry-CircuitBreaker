using DkWebService.EndPoint.Clients.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DkWebService.EndPoint.Controllers;

[ApiController]
[Route("[controller]")]
public class StringyController : ControllerBase
{
    private readonly ILogger<StringyController> _logger;
    private readonly IOdysseusClient _odysseusClient;

    public StringyController(
        ILogger<StringyController> logger,
        IOdysseusClient odysseusClient)
    {
        _logger = logger;
        _odysseusClient = odysseusClient;
    }

    [HttpGet(Name = "GetStringyStrings")]
    public async Task<IEnumerable<string>> Get(int userId)
    {
        var strings = await _odysseusClient.GetSomethingFromTheOutsideWorld(userId);

        return strings;
    }
}
