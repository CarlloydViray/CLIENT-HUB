using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BPOI_HUB.modules.api
{
    [Route("api/[controller]")]
    [ApiController]
    public class testapi : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            // Your API logic here
            return Ok(new { Message = "Hello from your API!" });
        }
    }
}
