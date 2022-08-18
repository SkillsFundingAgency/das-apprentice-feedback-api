using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NServiceBus;
using NServiceBus.ObjectBuilder.MSDependencyInjection;
using NServiceBus.Persistence;
using SFA.DAS.ApprenticeFeedback.Data;
using SFA.DAS.ApprenticeFeedback.Domain.Configuration;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
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
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Api.StartupExtensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEntityFrameworkApprenticeFeedback(this IServiceCollection services, ApplicationSettings applicationSettings, IConfiguration configuration)
        {
            return services.AddScoped(p =>
            {
                var unitOfWorkContext = p.GetRequiredService<IUnitOfWorkContext>();
                var connectionFactory = p.GetRequiredService<IConnectionFactory>();
                var loggerFactory = p.GetRequiredService<ILoggerFactory>();
                ApprenticeFeedbackDataContext dbContext;
                try
                {
                    if (configuration.IsIntegrationTests())
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
                        var optionsBuilder = new DbContextOptionsBuilder<ApprenticeFeedbackDataContext>()
                            .UseDataStorage(connectionFactory, sqlStorageSession.Connection)
                            .UseLocalSqlLogger(loggerFactory, configuration);
                        if (configuration.IsLocalAcceptanceOrDev())
                        {
                            optionsBuilder.EnableSensitiveDataLogging().UseLoggerFactory(loggerFactory);
                        }
                        dbContext = new ApprenticeFeedbackDataContext(optionsBuilder.Options);
                        dbContext.Database.UseTransaction(sqlStorageSession.Transaction);
                    }
                }
                catch (KeyNotFoundException)
                {
                    var optionsBuilder = new DbContextOptionsBuilder<ApprenticeFeedbackDataContext>()
                        .UseDataStorage(connectionFactory, applicationSettings.DbConnectionString)
                        .UseLocalSqlLogger(loggerFactory, configuration);
                    dbContext = new ApprenticeFeedbackDataContext(optionsBuilder.Options);
                }

                return dbContext;
            });

        }

        public static async Task<UpdateableServiceProvider> StartNServiceBus(this UpdateableServiceProvider serviceProvider, ApplicationSettings appSettings)
        {
            var connectionFactory = serviceProvider.GetRequiredService<IConnectionFactory>();

            var endpointConfiguration = new EndpointConfiguration("SFA.DAS.ApprenticeFeedback.Api")
                .UseMessageConventions()
                .UseNewtonsoftJsonSerializer()
                .UseOutbox(true)
                .UseServicesBuilder(serviceProvider)
                .UseSqlServerPersistence(() => connectionFactory.CreateConnection(appSettings.DbConnectionString))
                .UseUnitOfWork();

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
