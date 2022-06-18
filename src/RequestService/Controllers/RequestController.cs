using Microsoft.AspNetCore.Mvc;
//using RequestService.Policies;

namespace RequestService.Controllers
{
    [ApiController]
    [Route("api/v1/[Controller]")]
    public class RequestController : ControllerBase
    {
        private readonly IHttpClientFactory _clientFactory;
        //private readonly ClientPolicy _clientPolicy;

        public RequestController(IHttpClientFactory clientFactory/*, ClientPolicy clientPolicy*/)
        {
            _clientFactory = clientFactory;
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
            var client = _clientFactory.CreateClient("TestClient");
            var response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"{DateTime.Now} | ResponseService returned SUCCESS!");
                return Ok();
            }

            Console.WriteLine($"{DateTime.Now} | ResponseService returned FAILURE!");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
