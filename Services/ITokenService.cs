using dotnet_6_vue_cli_jwt_refresh_token.Models;
using dotnet_6_vue_cli_jwt_refresh_token.Models.Entities;
using System.Security.Claims;

namespace dotnet_6_vue_cli_jwt_refresh_token.Services {
    public interface ITokenService {
        //驗證登入者,並回傳accessToken & refreshToken
        AuthenticatedResponse Authenticate(LoginModel loginModel);
        //產生 access Token
        string GenerateAccessToken(IEnumerable<Claim> claims);
        //生成 refresh token
        string GenerateRefreshToken();
        //從過期的token中取得用戶主體
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}
