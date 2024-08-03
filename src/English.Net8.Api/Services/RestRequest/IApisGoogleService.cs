using English.Net8.Api.Dtos.Account;

namespace English.Net8.Api.Services.RestRequest
{
    public interface IApisGoogleService
    {
        Task<GoogleTokenResponse> ExchangeCodeForTokenAsync(string code, string redirectUri, string clientId, string clientSecret);
        Task<GoogleUserInfoResponse> GetUserInfoAsync(string accessToken);
    }
}
