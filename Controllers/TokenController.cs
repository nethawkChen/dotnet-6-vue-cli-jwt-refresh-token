using dotnet_6_vue_cli_jwt_refresh_token.Context;
using dotnet_6_vue_cli_jwt_refresh_token.Models;
using dotnet_6_vue_cli_jwt_refresh_token.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_6_vue_cli_jwt_refresh_token.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase {
        private readonly UserContext _userContext;
        private readonly ITokenService _tokenService;

        public TokenController(UserContext userContext, ITokenService tokenService) {
            _userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        }

        /// <summary>
        /// refresh token
        /// </summary>
        /// <param name="tokenApiModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("refresh")]
        public IActionResult Refresh(TokenApiModel tokenApiModel) {
            if (tokenApiModel is null) {
                return BadRequest("Invalid client request");
            }

            string accessToken = tokenApiModel.AccessToken;
            string refreshToken = tokenApiModel.RefreshToken;

            var principal = _tokenService.GetPrincipalFromExpiredToken(accessToken); //由原本的accessToken 取得user
            var username = principal.Identity.Name;

            var user = _userContext.LoginModels.SingleOrDefault(u => u.UserName == username);

            if (user is null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.Now) {
                return BadRequest("Invalid client request");
            }

            var newAccessToken = _tokenService.GenerateAccessToken(principal.Claims);
            var newRefreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            _userContext.SaveChanges();

            return Ok(new AuthenticatedResponse() { AccessToken = newAccessToken, RefreshToken = newRefreshToken, });
        }

        /// <summary>
        /// remove token
        /// </summary>
        /// <returns></returns>
        [HttpPost,Authorize]
        [Route("revoke")]
        public IActionResult Revoke() {
            var username = User.Identity.Name;
            var user = _userContext.LoginModels.SingleOrDefault(u => u.UserName == username);
            if (user == null) return BadRequest();

            user.RefreshToken = null;
            _userContext.SaveChanges();

            return NoContent();
        }
    }
}
