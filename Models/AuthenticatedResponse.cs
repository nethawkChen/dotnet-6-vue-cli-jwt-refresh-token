namespace dotnet_6_vue_cli_jwt_refresh_token.Models {
    public class AuthenticatedResponse {
        public string username { get; set; }
        /// <summary>
        /// access token
        /// </summary>
        public string? AccessToken { get; set; }

        /// <summary>
        /// refresh token
        /// </summary>
        public string? RefreshToken { get; set; }
    }
}
