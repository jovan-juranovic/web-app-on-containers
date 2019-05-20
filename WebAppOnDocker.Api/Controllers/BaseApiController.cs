using Microsoft.AspNetCore.Mvc;
using WebAppOnDocker.Api.Utils;

namespace WebAppOnDocker.Api.Controllers
{
    [ApiController]
    [Route("v{version:apiVersion}/[controller]/[action]")]
    public class BaseApiController : Controller
    {
        protected new IActionResult Ok()
        {
            return base.Ok(Envelope.Ok());
        }

        protected IActionResult Ok<T>(T result)
        {
            return base.Ok(Envelope.Ok(result));
        }

        protected IActionResult Error(string errorMessage)
        {
            return BadRequest(Envelope.Error(errorMessage));
        }
    }
}