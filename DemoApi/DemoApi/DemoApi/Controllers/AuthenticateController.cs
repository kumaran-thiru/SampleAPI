using DemoApi.Authorization;
using DemoApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace DemoApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private readonly IUserActions userActions;
        private readonly IJWTActions jwtactions;

        public AuthenticateController(IUserActions userActions, IJWTActions jwtactions)
        {
            this.userActions = userActions;
            this.jwtactions = jwtactions;
        }

        [AllowAnonymous]
        [HttpPost("/Authenticate")]
        public ActionResult Authenticate([FromBody] User user)
        {
            var stat = userActions.IsValidUser(user);
            if (!stat)
            {
                return Unauthorized();
            }
            var token = jwtactions.GenerateToken(user);
            return Ok(new { username = user.Username, status = "Authorized", token = token });            
        }
    }
}
