using dotnet_6_vue_cli_jwt_refresh_token.Models.Entities;
using dotnet_6_vue_cli_jwt_refresh_token.Services;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_6_vue_cli_jwt_refresh_token.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase {
        private readonly ITokenService _tokenService;

        public AuthController(ITokenService tokenService) {
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        }

        /// <summary>
        /// 登入
        /// </summary>
        /// <param name="loginModel"></param>
        /// <returns></returns>
        [HttpPost, Route("login")]
        public IActionResult Login([FromBody] LoginModel loginModel) {
            try {
                var response = _tokenService.Authenticate(loginModel);  //驗證使用者及取得token
                if (response == null) {
                    return Unauthorized();
                } else {
                    return Ok(response);
                }
            } catch (Exception er) {
                return Unauthorized();
            }

        }
    }
}
