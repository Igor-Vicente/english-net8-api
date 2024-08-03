﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using Store.MongoDb.Identity.Extensions;
using Store.MongoDb.Identity.Models;
using System.Text;

namespace English.Net8.Api.Configuration
{
    public static class IdentityConfiguration
    {
        public static IServiceCollection AddIdentityConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var authSection = configuration.GetSection("AuthSettings");
            var mongoSection = configuration.GetSection("MongoSettings");
            var mongoSettings = mongoSection.Get<MongoSettings>();
            var authSettings = authSection.Get<AuthSettings>();

            services.AddIdentity<MongoUser, MongoRole>(SetIdentityOptions)
                .AddDefaultTokenProviders()
                .AddIdentityMongoDbStores<MongoUser, MongoRole, ObjectId>(o =>
                {
                    o.ConnectionString = mongoSettings.ConnectionString;
                    o.DatabaseName = mongoSettings.DatabaseName;
                    o.UsersCollection = mongoSettings.AccountsCollection;
                    o.RolesCollection = mongoSettings.RolesCollection;
                });

            services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(o => SetJwtOptions(o, authSettings));

            return services;
        }

        private static void SetIdentityOptions(IdentityOptions options)
        {
            options.SignIn.RequireConfirmedAccount = false;

            options.Password.RequiredLength = 8;
            options.Password.RequiredUniqueChars = 0;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireDigit = false;
            options.Password.RequireNonAlphanumeric = false;

            options.Lockout.MaxFailedAccessAttempts = 6;
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(20);

            options.User.RequireUniqueEmail = true;

            //By default, the token's lifetime is set to 1 day (24 hours).
            options.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultProvider;
            options.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultProvider;
        }

        private static void SetJwtOptions(JwtBearerOptions options, AuthSettings authSettings)
        {
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(authSettings.Secret)),
                ValidAudience = authSettings.Audience,
                ValidIssuer = authSettings.Issuer,
            };
            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    context.Token = context.Request.Cookies[authSettings.AuthCookieName];
                    return Task.CompletedTask;
                }
            };
        }
    }
}
