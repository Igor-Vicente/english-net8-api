namespace English.Net8.Api.Configuration
{
    public static class CorsConfiguration
    {
        public static IServiceCollection AddCorsConfiguration(this IServiceCollection services)
        {
            var authSection = configuration.GetSection("AuthSettings");
            var authSettings = authSection.Get<AuthSettings>();

            services.AddCors(options =>
            {
                options.AddPolicy("Production", builder =>
                builder
                .AllowAnyMethod()
                .AllowAnyHeader()
                .WithOrigins(authSettings.AcceptedDomainsCors.split(','))
                .AllowCredentials());
            });

            return services;
        }
    }
}