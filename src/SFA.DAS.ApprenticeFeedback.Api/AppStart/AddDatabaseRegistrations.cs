using Azure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.ApprenticeFeedback.Data;
using SFA.DAS.ApprenticeFeedback.Domain.Configuration;
using System;

namespace SFA.DAS.ApprenticeFeedback.Api.AppStart
{
    public static class AddDatabaseRegistrations
    {
        public static void AddDatabaseRegistration(this IServiceCollection services, IConfiguration configuration)
        {
            var appSettings = configuration.GetSection("ApplicationSettings").Get<ApplicationSettings>();
            if (configuration.IsLocalAcceptanceOrDev())
            {
                services.AddDbContext<ApprenticeFeedbackDataContext>(options => options.UseSqlServer(appSettings.DbConnectionString).EnableSensitiveDataLogging(), ServiceLifetime.Transient);
            }
            else if (configuration.IsIntegrationTests())
            {
                services.AddDbContext<ApprenticeFeedbackDataContext>(options => options.UseSqlServer("Server=localhost;Database=SFA.DAS.ApprenticeFeedback.IntegrationTests.Database;Trusted_Connection=True;MultipleActiveResultSets=true").EnableSensitiveDataLogging(), ServiceLifetime.Transient);
            }
            else
            {
                services.AddSingleton(new DefaultAzureCredential());
                services.AddDbContext<ApprenticeFeedbackDataContext>(ServiceLifetime.Transient);
            }

            services.AddTransient(provider => new Lazy<ApprenticeFeedbackDataContext>(provider.GetService<ApprenticeFeedbackDataContext>()));
        }
    }
}
