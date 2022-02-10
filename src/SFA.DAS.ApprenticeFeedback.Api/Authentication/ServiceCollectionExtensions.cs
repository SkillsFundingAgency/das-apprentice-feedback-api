using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.ApprenticeFeedback.Api.Configuration;
using System.Collections.Generic;

namespace SFA.DAS.ApprenticeFeedback.Api.Authentication
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApiAuthentication(this IServiceCollection services, AzureActiveDirectoryApiConfiguration config, bool isDevelopment)
        {
            if (isDevelopment)
            {
                services.AddAuthentication("DevelopmentAuthentication")
                       .AddScheme<AuthenticationSchemeOptions, LocalAuthenticationHandler>("DevelopmentAuthentication", null);
            }
            else
            {
                services.AddAuthentication(auth =>
                {
                    auth.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                }).AddJwtBearer(auth =>
                {
                    auth.Authority =
                        $"https://login.microsoftonline.com/{config.Tenant}";
                    auth.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                    {
                        ValidAudiences = new List<string>
                    {
                        config.Identifier
                    }
                    };
                });
                services.AddSingleton<IClaimsTransformation, AzureAdScopeClaimTransformation>();
            }
            
            return services;
        }
    }
}
