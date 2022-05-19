using Microsoft.EntityFrameworkCore;
using SFA.DAS.ApprenticeFeedback.Domain.Entities;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
using SFA.DAS.ApprenticeFeedback.Data.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Data.SqlClient;
using Microsoft.Azure.Services.AppAuthentication;
using SFA.DAS.ApprenticeFeedback.Domain.Configuration;

namespace SFA.DAS.ApprenticeFeedback.Data
{
    public class ApprenticeFeedbackDataContext : DbContext, IApprenticeFeedbackDataContext
    {
        private const string AzureResource = "https://database.windows.net/";
        private readonly ApplicationSettings _configuration;
        private readonly AzureServiceTokenProvider _azureServiceTokenProvider;

        public DbSet<Attribute> Attributes { get; set; }
        public DbSet<ApprenticeFeedbackTarget> ApprenticeFeedbackTargets { get; set; }
        public DbSet<ApprenticeFeedbackResult> ApprenticeFeedbackResults { get; set; }
        public DbSet<ProviderAttribute> ProviderAttributes { get; set; }

        public ApprenticeFeedbackDataContext(DbContextOptions<ApprenticeFeedbackDataContext> options) : base(options)
        {
        }

        public ApprenticeFeedbackDataContext(IOptions<ApplicationSettings> config, DbContextOptions<ApprenticeFeedbackDataContext> options, AzureServiceTokenProvider azureServiceTokenProvider) : base(options)
        {
            _configuration = config.Value;
            _azureServiceTokenProvider = azureServiceTokenProvider;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (_configuration == null || _azureServiceTokenProvider == null)
            {
                return;
            }

            var connection = new SqlConnection
            {
                ConnectionString = _configuration.DbConnectionString,
                AccessToken = _azureServiceTokenProvider.GetAccessTokenAsync(AzureResource).Result
            };
            optionsBuilder.UseSqlServer(connection);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new AttributeConfiguration());
            modelBuilder.ApplyConfiguration(new ApprenticeFeedbackTargetConfiguration());
            modelBuilder.ApplyConfiguration(new ApprenticeFeedbackResultConfiguration());
            modelBuilder.ApplyConfiguration(new ProviderAttributeConfiguration());
            base.OnModelCreating(modelBuilder);
        }

    }
}
