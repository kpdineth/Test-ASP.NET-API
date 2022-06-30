using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace CoreCodeCamp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OperationsController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public OperationsController(IConfiguration Configuration)
        {
            _configuration = Configuration;
        }

        [HttpOptions("reloadconfig")]
        public IActionResult ReloadConfig()
        {
            var root = (IConfigurationRoot)_configuration;
            root.Reload();
            return Ok();
        }
    }
}
