using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.ApprenticeFeedback.Api.Configuration;
using SFA.DAS.ApprenticeFeedback.Data;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
using System;

namespace SFA.DAS.ApprenticeFeedback.Api.AppStart
{
    public static class AddDatabaseRegistrations
    {
        public static void AddDatabase(this IServiceCollection services, ApplicationSettings config, string environmentName)
        {
            if (environmentName.Equals("LOCAL", StringComparison.CurrentCultureIgnoreCase))
            {
                services.AddDbContext<ApprenticeFeedbackDataContext>(options => options.UseSqlServer(config.DbConnectionString).EnableSensitiveDataLogging(), ServiceLifetime.Transient);
            }
            else if (environmentName.Equals("IntegrationTests", StringComparison.CurrentCultureIgnoreCase))
            {
                services.AddDbContext<ApprenticeFeedbackDataContext>(options => options.UseSqlServer("Server=localhost;Database=SFA.DAS.ApprenticeFeedback.IntegrationTests.Database;Trusted_Connection=True;MultipleActiveResultSets=true").EnableSensitiveDataLogging(), ServiceLifetime.Transient);
            }
            else 
            { 
                services.AddSingleton(new AzureServiceTokenProvider());
                services.AddDbContext<ApprenticeFeedbackDataContext>(ServiceLifetime.Transient);
            }

            services.AddTransient<IApprenticeFeedbackDataContext, ApprenticeFeedbackDataContext>(provider => provider.GetService<ApprenticeFeedbackDataContext>());
            services.AddTransient(provider => new Lazy<ApprenticeFeedbackDataContext>(provider.GetService<ApprenticeFeedbackDataContext>()));
        }
    }
}
