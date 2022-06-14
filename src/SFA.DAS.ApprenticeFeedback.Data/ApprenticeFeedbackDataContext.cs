using Microsoft.EntityFrameworkCore;
using SFA.DAS.ApprenticeFeedback.Domain.Entities;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
using SFA.DAS.ApprenticeFeedback.Data.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Data.SqlClient;
using Microsoft.Azure.Services.AppAuthentication;
using SFA.DAS.ApprenticeFeedback.Domain.Configuration;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.ApprenticeFeedback.Data
{
    public class ApprenticeFeedbackDataContext : DbContext, IApprenticeFeedbackDataContext, IApprenticeFeedbackTargetContext
    {
        private const string AzureResource = "https://database.windows.net/";
        private readonly ApplicationSettings _configuration;
        private readonly AzureServiceTokenProvider _azureServiceTokenProvider;

        public DbSet<Domain.Entities.Attribute> Attributes { get; set; }
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

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            OnBeforeSaving();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override async Task<int> SaveChangesAsync(
           bool acceptAllChangesOnSuccess,
           CancellationToken cancellationToken = default(CancellationToken)
        )
        {
            OnBeforeSaving();
            return (await base.SaveChangesAsync(acceptAllChangesOnSuccess,
                          cancellationToken));
        }

        private void OnBeforeSaving()
        {
            var entries = ChangeTracker.Entries();
            var utcNow = DateTime.UtcNow;

            foreach (var entry in entries)
            {
                // for entities that inherit from BaseEntity,
                // set UpdatedOn / CreatedOn appropriately
                if (entry.Entity is EntityBase trackable)
                {
                    switch (entry.State)
                    {
                        case EntityState.Modified:
                            // set the updated date to "now"
                            trackable.UpdatedOn = utcNow;

                            // mark property as "don't touch"
                            // we don't want to update on a Modify operation
                            entry.Property("CreatedOn").IsModified = false;
                            break;

                        case EntityState.Added:
                            // set both updated and created date to "now"
                            trackable.CreatedOn = utcNow;
                            trackable.UpdatedOn = utcNow;
                            break;
                    }

                }
            }
        }

        public async Task<ApprenticeFeedbackTarget> GetApprenticeFeedbackTargetByIdAsync(Guid apprenticeFeedbackTargetId)
            => await ApprenticeFeedbackTargets.Include(s => s.ApprenticeFeedbackResults)
                .SingleOrDefaultAsync(aft => aft.Id == apprenticeFeedbackTargetId);

        public async Task<IEnumerable<ApprenticeFeedbackTarget>> GetApprenticeFeedbackTargetsAsync(Guid apprenticeId)
            => await ApprenticeFeedbackTargets.Include(s => s.ApprenticeFeedbackResults)
            .Where(aft => aft.ApprenticeId == apprenticeId).ToListAsync();

        public async Task<IEnumerable<ApprenticeFeedbackTarget>> GetApprenticeFeedbackTargetsAsync(Guid apprenticeId, long ukprn)
            => await ApprenticeFeedbackTargets.Include(s => s.ApprenticeFeedbackResults)
            .Where(aft => aft.ApprenticeId == apprenticeId && aft.Ukprn == ukprn).ToListAsync();

        public async Task<ApprenticeFeedbackTarget> GetApprenticeFeedbackTargetAsync(Guid apprenticeId, long commitmentApprenticeshipId)
        => await ApprenticeFeedbackTargets.Include(s => s.ApprenticeFeedbackResults).
            FirstOrDefaultAsync(aft => aft.ApprenticeId == apprenticeId && aft.ApprenticeshipId == commitmentApprenticeshipId);
    }
}
