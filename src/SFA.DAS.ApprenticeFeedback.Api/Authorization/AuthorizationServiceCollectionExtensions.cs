using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace SFA.DAS.ApprenticeFeedback.Api.Authorization
{
    public static class AuthorizationServiceCollectionExtensions
    {
        public static IServiceCollection AddApiAuthorization(this IServiceCollection services, IWebHostEnvironment environment)
        {
            var isDevelopment = environment.IsDevelopment();

            services.AddAuthorization(x =>
            {
                {
                    x.AddPolicy("default", policy =>
                    {
                        if (isDevelopment)
                            policy.AllowAnonymousUser();
                        else
                            policy.RequireAuthenticatedUser();

                    });


                    x.DefaultPolicy = x.GetPolicy("default");
                }
            });

            if (isDevelopment)
                services.AddSingleton<IAuthorizationHandler, LocalAuthorizationHandler>();

            return services;
        }
    }
}
