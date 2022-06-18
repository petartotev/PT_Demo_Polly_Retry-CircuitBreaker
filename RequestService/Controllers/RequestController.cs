using Microsoft.AspNetCore.Mvc;

namespace RequestService.Controllers
{
    [ApiController]
    [Route("api/v1/[Controller]")]
    public class RequestController : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult> MakeRequest()
        {
            var id = 50;
            var url = $"https://localhost:6000/api/v1/response/{id}";
            var client = new HttpClient();

            var response = await client.GetAsync(url);

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
