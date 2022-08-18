using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using NServiceBus.ObjectBuilder.MSDependencyInjection;
using SFA.DAS.ApprenticeFeedback.Api.StartupExtensions;
using SFA.DAS.ApprenticeFeedback.Api.Authentication;
using SFA.DAS.ApprenticeFeedback.Api.Authorization;
using SFA.DAS.ApprenticeFeedback.Api.Configuration;
using SFA.DAS.ApprenticeFeedback.Data;
using SFA.DAS.ApprenticeFeedback.Domain.Configuration;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.UnitOfWork.NServiceBus.Features.ClientOutbox.DependencyResolution.Microsoft;
using System;
using System.IO;
using SFA.DAS.UnitOfWork.EntityFrameworkCore.DependencyResolution.Microsoft;

namespace SFA.DAS.ApprenticeFeedback.Api
{
    public class Startup
    {
        private readonly IWebHostEnvironment Environment;
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            var config = new ConfigurationBuilder()
                .AddConfiguration(configuration)
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddEnvironmentVariables();

            config.AddAzureTableStorage(options =>
            {
                options.ConfigurationKeys = configuration["ConfigNames"].Split(",");
                options.StorageConnectionString = configuration["ConfigurationStorageConnectionString"];
                options.EnvironmentName = configuration["EnvironmentName"];
                options.PreFixConfigurationKeys = false;
            });
#if DEBUG
            config.AddJsonFile($"appsettings.Development.json", optional: true);
#endif

            Configuration = config.Build();
            Environment = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplicationInsightsTelemetry();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "SFA.DAS.ApprenticeFeedback.Api", Version = "v1" });
            });

            services.Configure<ApplicationSettings>(Configuration.GetSection("ApplicationSettings"));
            services.AddSingleton(s => s.GetRequiredService<IOptions<ApplicationSettings>>().Value);
            var appSettings = Configuration.GetSection("ApplicationSettings").Get<ApplicationSettings>();

            services.Configure<AzureActiveDirectoryApiConfiguration>(Configuration.GetSection("AzureAd"));
            services.AddSingleton(cfg => cfg.GetService<IOptions<AzureActiveDirectoryApiConfiguration>>().Value);
            var azureAdConfiguration = Configuration.GetSection("AzureAd").Get<AzureActiveDirectoryApiConfiguration>();

            var isDevelopment = Environment.IsDevelopment();
            services.AddApiAuthentication(azureAdConfiguration, isDevelopment)
                .AddApiAuthorization(isDevelopment);

            services.AddEntityFrameworkApprenticeFeedback(appSettings, Configuration)
                .AddEntityFrameworkUnitOfWork<ApprenticeFeedbackDataContext>()
                .AddNServiceBusClientUnitOfWork();

            services.AddHealthChecks()
                .AddCheck<ApprenticeFeedbackHealthCheck>(nameof(ApprenticeFeedbackHealthCheck));

            services
                .AddControllers(o =>
                {
                    if (!isDevelopment)
                    {
                        o.Filters.Add(new AuthorizeFilter(PolicyNames.Default));
                    }
                }).SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.AddServices();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "SFA.DAS.ApprenticeFeedback.Api v1"));

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/ping");
            });
        }

        public void ConfigureContainer(UpdateableServiceProvider serviceProvider)
        {
            var appSettings = Configuration.GetSection("ApplicationSettings").Get<ApplicationSettings>();

            if (!Configuration["EnvironmentName"].Equals("IntegrationTests", StringComparison.CurrentCultureIgnoreCase))
                serviceProvider.StartNServiceBus(appSettings).GetAwaiter().GetResult();
        }
    }
}
