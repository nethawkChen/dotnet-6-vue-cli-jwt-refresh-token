using dotnet_6_vue_cli_jwt_refresh_token.Context;
using dotnet_6_vue_cli_jwt_refresh_token.Models;
using dotnet_6_vue_cli_jwt_refresh_token.Models.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace dotnet_6_vue_cli_jwt_refresh_token.Services {
    public class TokenService : ITokenService {
        private readonly UserContext _userContext;

        public TokenService(UserContext userContext) {
            _userContext = userContext;
        }

        /// <summary>
        /// 驗證登入者,並回傳accessToken & refreshToken
        /// </summary>
        /// <param name="loginModel"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public AuthenticatedResponse Authenticate(LoginModel loginModel) {
            //查詢使用者是否符合
            var user = _userContext.LoginModels.FirstOrDefault(u =>
                (u.UserName == loginModel.UserName) && (u.Password == loginModel.Password));

            if (user is null) {
                throw new Exception("未授權");
            }

            //建立使用者資料 Claim
            var claims = new List<Claim> {
                new Claim(ClaimTypes.Name, loginModel.UserName),
                new Claim(ClaimTypes.Role,"Manager"),
                new Claim("ClientIp",loginModel.ClientIp)
            };

            var accessToken = GenerateAccessToken(claims);
            var refreshToken = GenerateRefreshToken();

            user.ClientIp = loginModel.ClientIp;
            user.RefreshToken = refreshToken;
            //user.RefreshTokenExpiryTime = DateTime.Now.AddSeconds(300);   // 設定 refresh token 期限 300 秒
            user.RefreshTokenExpiryTime = DateTime.Now.AddSeconds(60);   // 設定 refresh token 期限 60 秒

            _userContext.SaveChanges();

            return new AuthenticatedResponse {
                username = loginModel.UserName,
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }
        /// <summary>
        /// 產生 access Token
        /// </summary>
        /// <param name="claims">使用者資訊</param>
        /// <returns></returns>
        public string GenerateAccessToken(IEnumerable<Claim> claims) {
            //獲取 SecurityKey
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("0FBCF3C1-0818-42EB-83B2-B3E15FC16C2C"));
            var tokenOptions = new JwtSecurityToken(
                issuer: "MyWebApi.home",
                audience: "MyWebApiTokenCenter",
                claims: claims,
                //expires: DateTime.Now.AddSeconds(60),  //設定 JWT token 有效時間 60 秒
                expires: DateTime.Now.AddSeconds(30),  //設定 JWT token 有效時間 30 秒
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)  //用於簽發秘鑰的算法
                );
            var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

            return tokenString;
        }
        /// <summary>
        /// 生成 refresh token
        /// </summary>
        /// <returns></returns>
        public string GenerateRefreshToken() {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create()) {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        /// <summary>
        /// 從過期的token中取得用戶主體
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token) {
            var tokenValidationParameters = new TokenValidationParameters() {
                ValidateAudience = false,
                ValidateIssuer = true,
                ValidIssuer = "MyWebApi.home",
                ValidateLifetime = false,
                ValidateIssuerSigningKey = false,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("0FBCF3C1-0818-42EB-83B2-B3E15FC16C2C")),
                ClockSkew = TimeSpan.Zero
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null
                || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase)) {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }
    }
}
