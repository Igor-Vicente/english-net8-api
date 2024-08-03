using Newtonsoft.Json;

namespace English.Net8.Api.Services.RestRequest
{
    public abstract class Service
    {
        protected async Task<T> DeserializarResponseAsync<T>(HttpResponseMessage response)
        {
            return JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync());
        }
    }
}
