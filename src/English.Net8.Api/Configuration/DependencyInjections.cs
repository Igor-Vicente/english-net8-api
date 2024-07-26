using English.Net8.Api.Repository;
using English.Net8.Api.Repository.Interfaces;
using English.Net8.Api.Services.Mailing;

namespace English.Net8.Api.Configuration
{
    public static class DependencyInjections
    {
        public static IServiceCollection ResolveDependencies(this IServiceCollection services)
        {
            services.AddSingleton<IUserRepository, UserRepository>();
            services.AddSingleton<IQuestionRepository, QuestionRepository>();
            services.AddSingleton<IEmailSender, EmailSender>();

            return services;
        }
    }
}
