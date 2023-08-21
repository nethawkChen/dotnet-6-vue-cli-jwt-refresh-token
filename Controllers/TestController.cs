using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_6_vue_cli_jwt_refresh_token.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase {
        [HttpPost, Route("SayHello")]
        [Authorize]
        public IActionResult SayHello([FromBody] ClientVale clientValue) {
            ResultData result = new ResultData() {
                Data = $"{clientValue.name} said {clientValue.msg}"
            };
            return Ok(result);
        }
    }

    public class ResultData {
        public string? Code { get; set; } = "200";
        public string? Data { get; set; }
    }

    public class ClientVale {
        public string? name { get; set; }
        public string? msg { get; set; }
    }
}
