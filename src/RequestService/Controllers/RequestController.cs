using Microsoft.AspNetCore.Mvc;
//using RequestService.Policies;

namespace RequestService.Controllers;

[ApiController]
[Route("api/v1/[Controller]")]
public class RequestController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    //private readonly ClientPolicy _clientPolicy;

    public RequestController(IHttpClientFactory httpClientFactory/*, ClientPolicy clientPolicy*/)
    {
        _httpClientFactory = httpClientFactory;
        //_clientPolicy = clientPolicy;
    }

    [HttpGet]
    public async Task<ActionResult> MakeRequest()
    {
        var id = 25;
        var url = $"https://localhost:7000/api/v1/response/{id}";

        // 1 NO POLLY
        //var client = new HttpClient();
        //var response = await client.GetAsync(url);

        // 2 DIRECTLY INTRODUCED IN CONTROLLER
        //var client = new HttpClient();
        //var response = await _clientPolicy.ExponentialHttpRetry.ExecuteAsync(() => client.GetAsync(url));

        // 3 CLEAN METHOD
        //var client = _httpClientFactory.CreateClient("TestClient");
        //var response = await client.GetAsync(url);

        //if (response.IsSuccessStatusCode)
        //{
        //    Console.WriteLine($"{DateTime.Now} | ResponseService returned SUCCESS!");
        //    return Ok();
        //}

        //Console.WriteLine($"{DateTime.Now} | ResponseService returned FAILURE!");
        //return StatusCode(StatusCodes.Status500InternalServerError);

        // 4 CIRCUIT BREAKER
        var client = _httpClientFactory.CreateClient("TestClient");

        try
        {
            var response = await client.GetAsync(url);

            // Process the response as needed

            return Ok(response);
        }
        catch (Exception ex)
        {
            // Handle the exception or return an error response
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }
}
