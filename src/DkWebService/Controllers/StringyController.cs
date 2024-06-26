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
    private readonly IPerseusClient _perseusClient;

    public StringyController(
        ILogger<StringyController> logger,
        IOdysseusClient odysseusClient,
        IAgamemnonClient agamemnonClient,
        IPerseusClient perseusClient)
    {
        _logger = logger;
        _odysseusClient = odysseusClient;
        _perseusClient = perseusClient;

    }

    [HttpGet("GetStringyStrings")]
    public async Task<IEnumerable<string>> GetSomeStrings(int userId)
    {
        var strings = await _odysseusClient.GetSomethingFromTheOutsideWorldAsync(userId);

        return strings;
    }

    [HttpGet("GetRandomInteger")]
    public async Task<int> GetSomeInteger(int eventId)
    {
        var inty = await _agamemnonClient.GetCalculationsByAgamemnonApiAsync(eventId);

        return inty;
    }
}
