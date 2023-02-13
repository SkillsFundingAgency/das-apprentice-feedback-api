using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.ApprenticeFeedback.Api.Authentication;

namespace SFA.DAS.ApprenticeFeedback.Api.Authorization
{
    public static class AuthorizationServiceCollectionExtensions
    {
        public static IServiceCollection AddApiAuthorization(this IServiceCollection services, bool IsDevelopment)
        {
            services.AddAuthorization(x =>
            {
                {
                    x.AddPolicy(PolicyNames.Default, policy =>
                    {
                        if (IsDevelopment)
                            policy.AllowAnonymousUser();
                        else
                        {
                            policy.RequireAuthenticatedUser();
                            policy.RequireRole(RoleNames.Default);
                        }
                    });


                    x.DefaultPolicy = x.GetPolicy(PolicyNames.Default);
                }
            });

            if (IsDevelopment)
                services.AddSingleton<IAuthorizationHandler, LocalAuthorizationHandler>();

            return services;
        }
    }
}
