using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace DemoApi.Controllers
{
    [ApiController]
    public class ErrorController : ControllerBase
    {
        [Route("/error")]
        public IActionResult Error()
        {
            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
            var con = HttpContext;
            con.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            return Problem(
                title: context.Error.Message,
                detail: "Error from " + 
                context.Error.TargetSite.DeclaringType.Name.Replace("Controller", "") +" -> "+
                context.Error.TargetSite.Name
                );
        }
    }
}
