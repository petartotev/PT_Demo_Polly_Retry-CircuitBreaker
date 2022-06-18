using Microsoft.AspNetCore.Mvc;
using RequestService.Policies;

namespace RequestService.Controllers
{
    [ApiController]
    [Route("api/v1/[Controller]")]
    public class RequestController : ControllerBase
    {
        private readonly ClientPolicy _clientPolicy;

        public RequestController(ClientPolicy clientPolicy)
        {
            _clientPolicy = clientPolicy;
        }

        [HttpGet]
        public async Task<ActionResult> MakeRequest()
        {
            var id = 25;
            var url = $"https://localhost:7000/api/v1/response/{id}";
            var client = new HttpClient();

            //var response = await client.GetAsync(url);

            var response = await _clientPolicy.ImmediateHttpRetry.ExecuteAsync(() => client.GetAsync(url));

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("ResponseService returned SUCCESS!");
                return Ok();
            }

            Console.WriteLine("ResponseService returned FAILURE!");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
