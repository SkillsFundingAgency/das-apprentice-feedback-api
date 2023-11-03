using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Options;
using SFA.DAS.ApprenticeFeedback.Data.Configuration;
using SFA.DAS.ApprenticeFeedback.Domain.Configuration;
using SFA.DAS.ApprenticeFeedback.Domain.Entities;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Data
{
    public class ApprenticeFeedbackDataContext : DbContext,
        IApprenticeFeedbackTargetContext,
        IApprenticeFeedbackResultContext,
        IProviderAttributeContext,
        IAttributeContext,
        IProviderRatingSummaryContext,
        IProviderAttributeSummaryContext,
        IProviderStarsSummaryContext,
        IExitSurveyContext,
        IFeedbackTransactionContext,
        IFeedbackTransactionClickContext,
        IEngagementEmailContext,
        IExclusionContext
    {
        private const string AzureResource = "https://database.windows.net/";
        private readonly ApplicationSettings _configuration;
        private readonly AzureServiceTokenProvider _azureServiceTokenProvider;

        public virtual DbSet<Domain.Entities.Attribute> Attributes { get; set; }
        public virtual DbSet<Domain.Entities.ApprenticeFeedbackTarget> ApprenticeFeedbackTargets { get; set; } = null!;
        public virtual DbSet<ApprenticeFeedbackResult> ApprenticeFeedbackResults { get; set; } = null!;
        public virtual DbSet<ProviderAttribute> ProviderAttributes { get; set; } = null!;
        public virtual DbSet<ProviderRatingSummary> ProviderRatingSummary { get; set; } = null!;
        public virtual DbSet<ProviderAttributeSummary> ProviderAttributeSummary { get; set; } = null!;
        public virtual DbSet<ProviderStarsSummary> ProviderStarsSummary { get; set; } = null!;
        public virtual DbSet<Domain.Entities.ApprenticeExitSurvey> ApprenticeExitSurveys { get; set; } = null!;
        public virtual DbSet<FeedbackTransaction> FeedbackTransactions { get; set; }
        public virtual DbSet<FeedbackTransactionClick> FeedbackTransactionClicks { get; set; }
        public virtual DbSet<EngagementEmail> EngagementEmails { get; set; }
        public virtual DbSet<Exclusion> Exclusions { get; set; }

        DbSet<ApprenticeFeedbackTarget> IEntityContext<Domain.Entities.ApprenticeFeedbackTarget>.Entities => ApprenticeFeedbackTargets;
        DbSet<ApprenticeFeedbackResult> IEntityContext<ApprenticeFeedbackResult>.Entities => ApprenticeFeedbackResults;
        DbSet<Domain.Entities.Attribute> IEntityContext<Domain.Entities.Attribute>.Entities => Attributes;
        DbSet<ProviderAttribute> IEntityContext<ProviderAttribute>.Entities => ProviderAttributes;
        DbSet<ProviderRatingSummary> IEntityContext<ProviderRatingSummary>.Entities => ProviderRatingSummary;
        DbSet<ProviderAttributeSummary> IEntityContext<ProviderAttributeSummary>.Entities => ProviderAttributeSummary;
        DbSet<ProviderStarsSummary> IEntityContext<ProviderStarsSummary>.Entities => ProviderStarsSummary;
        DbSet<ApprenticeExitSurvey> IEntityContext<ApprenticeExitSurvey>.Entities => ApprenticeExitSurveys;
        DbSet<FeedbackTransaction> IEntityContext<FeedbackTransaction>.Entities => FeedbackTransactions;
        DbSet<FeedbackTransactionClick> IEntityContext<FeedbackTransactionClick>.Entities => FeedbackTransactionClicks;
        DbSet<EngagementEmail> IEntityContext<EngagementEmail>.Entities => EngagementEmails;
        DbSet<Exclusion> IEntityContext<Exclusion>.Entities => Exclusions;


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
            modelBuilder.ApplyConfiguration(new ProviderRatingSummaryConfiguration());
            modelBuilder.ApplyConfiguration(new ProviderAttributeSummaryConfiguration());
            modelBuilder.ApplyConfiguration(new ProviderStarsSummaryConfiguration());
            modelBuilder.ApplyConfiguration(new FeedbackTransactionConfiguration());
            modelBuilder.ApplyConfiguration(new FeedbackTransactionClickConfiguration());
            modelBuilder.ApplyConfiguration(new ExitSurveyAttributeConfiguration());
            modelBuilder.ApplyConfiguration(new ExclusionsConfiguration());
            modelBuilder.ApplyConfiguration(new EngagementEmailConfiguration());
            modelBuilder.Entity<GenerateFeedbackTransactionsResult>().HasNoKey();
            base.OnModelCreating(modelBuilder);
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            OnBeforeSaving();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override async Task<int> SaveChangesAsync(
           bool acceptAllChangesOnSuccess,
           CancellationToken cancellationToken = default
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

        public async Task GenerateFeedbackSummaries(int minimumNumberOfResponses, int reportingFeedbackCutoffMonths)
        {
            var parameterRecentFeedbackMonths = new SqlParameter
            {
                ParameterName = "recentFeedbackMonths",
                SqlDbType = SqlDbType.Int,
                Value = reportingFeedbackCutoffMonths,
            };

            var parameterMinimumNumberOfReviews = new SqlParameter
            {
                ParameterName = "minimumNumberOfReviews",
                SqlDbType = SqlDbType.Int,
                Value = minimumNumberOfResponses,
            };

            await Database.ExecuteSqlRawAsync(
                "EXEC [dbo].[GenerateProviderAttributesSummary] @recentFeedbackMonths, @minimumNumberOfReviews",
                parameters: new[] { parameterRecentFeedbackMonths, parameterMinimumNumberOfReviews });

            await Database.ExecuteSqlRawAsync(
                "EXEC [dbo].[GenerateProviderRatingAndStarsSummary] @recentFeedbackMonths, @minimumNumberOfReviews",
                parameters: new[] { parameterRecentFeedbackMonths, parameterMinimumNumberOfReviews });
        }

        public async Task<IEnumerable<GenerateFeedbackTransactionsResult>> GenerateFeedbackTransactionsAsync(int feedbackTransactionSentDateAgeDays)
        {
            DbParameter parameterFeedbackTransactionSentDateAgeDays = new SqlParameter
            {
                ParameterName = "SentDateAgeDays",
                SqlDbType = SqlDbType.Int,
                Value = feedbackTransactionSentDateAgeDays
            };

            IEnumerable<GenerateFeedbackTransactionsResult> result =
                await Set<GenerateFeedbackTransactionsResult>().FromSqlRaw("EXEC dbo.GenerateFeedbackTransactions @SentDateAgeDays",
                                                                           new[] { parameterFeedbackTransactionSentDateAgeDays })
                                                               .ToListAsync();
            return result;
        }
    }
}
