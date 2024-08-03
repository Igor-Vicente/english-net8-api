using English.Net8.Api.Dtos.Account;
using System.Net.Http.Headers;

namespace English.Net8.Api.Services.RestRequest
{
    public class ApisGoogleService : Service, IApisGoogleService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ApisGoogleService> _logger;

        public ApisGoogleService(HttpClient httpClient, ILogger<ApisGoogleService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<GoogleTokenResponse> ExchangeCodeForTokenAsync(string code, string redirectUri, string clientId, string clientSecret)
        {
            var requestBody = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("code", code),
                new KeyValuePair<string, string>("client_id", clientId),
                new KeyValuePair<string, string>("client_secret", clientSecret),
                new KeyValuePair<string, string>("redirect_uri", redirectUri),
                new KeyValuePair<string, string>("grant_type", "authorization_code")
            });

            var response = await _httpClient.PostAsync("https://oauth2.googleapis.com/token", requestBody);
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException(content, null, response.StatusCode);

            return await DeserializarResponseAsync<GoogleTokenResponse>(response);
        }

        public async Task<GoogleUserInfoResponse> GetUserInfoAsync(string accessToken)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await _httpClient.GetAsync("https://www.googleapis.com/oauth2/v3/userinfo");
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException(content, null, response.StatusCode);
            return await DeserializarResponseAsync<GoogleUserInfoResponse>(response);
        }
    }
}
