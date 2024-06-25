using Microsoft.AspNetCore.Mvc;

namespace English.Net8.Api.Configuration
{
    public static class RegisterConfiguration
    {
        public static IServiceCollection RegisterConfigurations(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);
            services.Configure<EmailProviderSettings>(configuration.GetSection("EmailProviderSettings"));
            services.Configure<AuthSettings>(configuration.GetSection("AuthSettings"));

            return services;
        }
    }
}
