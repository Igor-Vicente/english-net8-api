namespace English.Net8.Api.Configuration
{
    public static class CorsConfiguration
    {
        public static IServiceCollection AddCorsConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var authSection = configuration.GetSection("AuthSettings");
            var authSettings = authSection.Get<AuthSettings>();

            services.AddCors(options =>
            {
                options.AddPolicy("Production", builder =>
                builder
                .AllowAnyMethod()
                .AllowAnyHeader()
                .WithOrigins(authSettings.AcceptedDomainsCors.Split(','))
                .AllowCredentials());
            });

            return services;
        }
    }
}