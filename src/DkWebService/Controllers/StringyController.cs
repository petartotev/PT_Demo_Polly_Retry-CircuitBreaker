using DkWebService.EndPoint.Clients.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DkWebService.EndPoint.Controllers;

[ApiController]
[Route("[controller]")]
public class StringyController : ControllerBase
{
    private readonly ILogger<StringyController> _logger;
    private readonly IOdysseusClient _odysseusClient;
    private readonly IAgamemnonClient _agamemnonClient;

    public StringyController(
        ILogger<StringyController> logger,
        IOdysseusClient odysseusClient,
        IAgamemnonClient agamemnonClient)
    {
        _logger = logger;
        _odysseusClient = odysseusClient;
        _agamemnonClient = agamemnonClient;
    }

    [HttpGet("GetStringyStrings")]
    public async Task<IEnumerable<string>> GetSomeStrings(int userId)
    {
        var strings = await _odysseusClient.GetSomethingFromTheOutsideWorld(userId);

        return strings;
    }

    [HttpGet("GetRandomInteger")]
    public async Task<int> GetSomeInteger(int eventId)
    {
        var inty = await _agamemnonClient.GetCalculationsByAgamemnonApi(eventId);

        return inty;
    }
}
