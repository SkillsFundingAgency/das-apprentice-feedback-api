using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NServiceBus;
using NServiceBus.ObjectBuilder.MSDependencyInjection;
using NServiceBus.Persistence;
using SFA.DAS.ApprenticeFeedback.Data;
using SFA.DAS.ApprenticeFeedback.Domain.Configuration;
using SFA.DAS.NServiceBus.Configuration;
using SFA.DAS.NServiceBus.Configuration.AzureServiceBus;
using SFA.DAS.NServiceBus.Configuration.MicrosoftDependencyInjection;
using SFA.DAS.NServiceBus.Configuration.NewtonsoftJsonSerializer;
using SFA.DAS.NServiceBus.Hosting;
using SFA.DAS.NServiceBus.SqlServer.Configuration;
using SFA.DAS.NServiceBus.SqlServer.Data;
using SFA.DAS.UnitOfWork.Context;
using SFA.DAS.UnitOfWork.NServiceBus.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Api.AppStart
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEntityFrameworkApprenticeFeedback(this IServiceCollection services, ApplicationSettings config, string environmentName)
        {
            return services.AddScoped(p =>
            {
                var unitOfWorkContext = p.GetService<IUnitOfWorkContext>();
                ApprenticeFeedbackDataContext dbContext;
                try
                {
                    if (environmentName.Equals("LOCAL", StringComparison.CurrentCultureIgnoreCase))
                    {
                        var localDbOptions = new DbContextOptionsBuilder<ApprenticeFeedbackDataContext>()
                        .UseSqlServer(config.DbConnectionString)
                        .EnableSensitiveDataLogging();
                        dbContext = new ApprenticeFeedbackDataContext(localDbOptions.Options);
                    }
                    else if (environmentName.Equals("IntegrationTests", StringComparison.CurrentCultureIgnoreCase))
                    {
                        var integrationTestOptions =
                            new DbContextOptionsBuilder<ApprenticeFeedbackDataContext>()
                            .UseSqlServer("Server=localhost;Database=SFA.DAS.ApprenticeFeedback.IntegrationTests.Database;Trusted_Connection=True;MultipleActiveResultSets=true")
                            .EnableSensitiveDataLogging();
                        dbContext = new ApprenticeFeedbackDataContext(integrationTestOptions.Options);
                    }
                    else
                    {
                        var synchronizedStorageSession = unitOfWorkContext.Get<SynchronizedStorageSession>();
                        var sqlStorageSession = synchronizedStorageSession.GetSqlStorageSession();
                        var optionsBuilder = new DbContextOptionsBuilder<ApprenticeFeedbackDataContext>().UseSqlServer(sqlStorageSession.Connection);
                        dbContext = new ApprenticeFeedbackDataContext(optionsBuilder.Options);
                        dbContext.Database.UseTransaction(sqlStorageSession.Transaction);
                    }
                }
                catch (KeyNotFoundException)
                {
                    var settings = p.GetService<IOptions<ApplicationSettings>>();
                    var optionsBuilder = new DbContextOptionsBuilder<ApprenticeFeedbackDataContext>().UseSqlServer(settings.Value.DbConnectionString);
                    dbContext = new ApprenticeFeedbackDataContext(optionsBuilder.Options);
                }

                return dbContext;
            });

        }

        public static async Task<UpdateableServiceProvider> StartNServiceBus(this UpdateableServiceProvider serviceProvider, ApplicationSettings appSettings)
        {
            var endpointName = "SFA.DAS.ApprenticeFeedback.Api";
            
            var endpointConfiguration = new EndpointConfiguration(endpointName)
                .UseMessageConventions()
                .UseNewtonsoftJsonSerializer()
                .UseOutbox(true)
                .UseServicesBuilder(serviceProvider)
                .UseSqlServerPersistence(() => new SqlConnection(appSettings.DbConnectionString))
                .UseUnitOfWork()
                .UseSendOnly();

            if (appSettings.NServiceBusConnectionString.Equals("UseLearningEndpoint=true", StringComparison.CurrentCultureIgnoreCase))
            {
                endpointConfiguration.UseTransport<LearningTransport>();
                endpointConfiguration.UseLearningTransport(s => s.AddRouting());
            }
            else
            {
                endpointConfiguration.UseAzureServiceBusTransport(appSettings.NServiceBusConnectionString, r => r.AddRouting());
            }

            if (!string.IsNullOrEmpty(appSettings.NServiceBusLicense))
            {
                endpointConfiguration.License(appSettings.NServiceBusLicense);
            }

            var endpoint = await Endpoint.Start(endpointConfiguration);

            serviceProvider.AddSingleton(p => endpoint)
                .AddSingleton<IMessageSession>(p => p.GetService<IEndpointInstance>())
                .AddHostedService<NServiceBusHostedService>();

            return serviceProvider;
        }
    }
}
