using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson.Serialization.Conventions;

namespace English.Net8.Api.Configuration
{
    public static class RegisterConfiguration
    {
        public static IServiceCollection RegisterConfigurations(this IServiceCollection services, IConfiguration configuration)
        {
            var conventionPack = new ConventionPack { new CamelCaseElementNameConvention() };
            ConventionRegistry.Register("CamelCaseElementNameConvention", conventionPack, t => true);

            services.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);
            services.Configure<EmailProviderSettings>(configuration.GetSection("EmailProviderSettings"));
            services.Configure<AuthSettings>(configuration.GetSection("AuthSettings"));
            services.Configure<MongoSettings>(configuration.GetSection("MongoSettings"));

            return services;
        }
    }
}
