using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace SoftwarePackageManager.Controllers.v1
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class RegistrationController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public RegistrationController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // GET api/<RegistrationController>/
        [HttpGet("Register")]
        public string Register()
        {           
            var apiKey = _configuration.GetValue<string>("API_KEY");
            return "API_KEY is " + apiKey;
        }

    }
}
