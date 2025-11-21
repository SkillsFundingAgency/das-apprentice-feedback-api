using System;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Azure.Core;
using Azure.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Options;
using SFA.DAS.ApprenticeFeedback.Data.Configuration;
using SFA.DAS.ApprenticeFeedback.Domain.Configuration;
using SFA.DAS.ApprenticeFeedback.Domain.Entities;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;

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
        IExclusionContext,
        IFeedbackTargetVariantContext,
        IFeedbackTargetVariant_StagingContext
    {
        private const string AzureResource = "https://database.windows.net/";
        private readonly ApplicationSettings _configuration;
        private readonly ChainedTokenCredential _chainedTokenCredentialProvider;

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
        public virtual DbSet<FeedbackTargetVariant> FeedbackTargetVariants { get; set; }
        public virtual DbSet<FeedbackTargetVariant_Staging> FeedbackTargetVariants_Staging { get; set; }

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
        DbSet<FeedbackTargetVariant> IEntityContext<FeedbackTargetVariant>.Entities => FeedbackTargetVariants;
        DbSet<FeedbackTargetVariant_Staging> IEntityContext<FeedbackTargetVariant_Staging>.Entities => FeedbackTargetVariants_Staging;


        public ApprenticeFeedbackDataContext(DbContextOptions<ApprenticeFeedbackDataContext> options) : base(options)
        {
        }

        public ApprenticeFeedbackDataContext(IOptions<ApplicationSettings> config, DbContextOptions<ApprenticeFeedbackDataContext> options, ChainedTokenCredential chainedTokenCredentialProvider) : base(options)
        {
            _configuration = config.Value;
            _chainedTokenCredentialProvider = chainedTokenCredentialProvider;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (_configuration == null)
            {
                return;
            }

            optionsBuilder.UseSqlServer(new SqlConnection
            {
                ConnectionString = _configuration.DbConnectionString,
                AccessToken = GetAccessToken()
            });
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
            modelBuilder.ApplyConfiguration(new FeedbackTargetVariantConfiguration());
            modelBuilder.ApplyConfiguration(new FeedbackTargetVariant_StagingConfiguration());
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

        public DbContext DbContext => this;

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

        public async Task GenerateFeedbackSummaries(int minimumNumberOfResponses)
        {
            var originalTimeout = Database.GetCommandTimeout();

            try
            {
                // set command timeout to 20 minutes (1200 seconds)
                Database.SetCommandTimeout(1200);

                var parameterMinimumNumberOfReviews = new SqlParameter
                {
                    ParameterName = "minimumNumberOfReviews",
                    SqlDbType = SqlDbType.Int,
                    Value = minimumNumberOfResponses,
                };

                await Database.ExecuteSqlRawAsync(
                    "EXEC [dbo].[GenerateProviderAttributesSummary] @minimumNumberOfReviews", parameterMinimumNumberOfReviews );

                await Database.ExecuteSqlRawAsync(
                    "EXEC [dbo].[GenerateProviderRatingAndStarsSummary] @minimumNumberOfReviews", parameterMinimumNumberOfReviews);
            }
            finally
            {
                // reset the command timeout to its original value
                Database.SetCommandTimeout(originalTimeout);
            }
        }

        public async Task<GenerateFeedbackTransactionsResult> GenerateFeedbackTransactionsAsync(int feedbackTransactionSentDateAgeDays, DateTime? specifiedUtcDate, CancellationToken cancellationToken)
        {
            var originalTimeout = Database.GetCommandTimeout();

            try
            {
                // set command timeout to 30 minutes (1800 seconds) usually this operation would only 
                // take seconds however when it first runs it will take considerably longer 
                Database.SetCommandTimeout(1800);

                DbParameter parameterSentDateAgeDays = new SqlParameter
                {
                    ParameterName = "sentDateAgeDays",
                    SqlDbType = SqlDbType.Int,
                    Value = feedbackTransactionSentDateAgeDays
                };

                DbParameter parameterSpecifiedUtcDate = new SqlParameter
                {
                    ParameterName = "specifiedUtcDate",
                    SqlDbType = SqlDbType.DateTime,
                    Value = specifiedUtcDate.HasValue ? specifiedUtcDate : DBNull.Value
                };

                var result =
                await Set<GenerateFeedbackTransactionsResult>()
                    .FromSqlRaw("EXEC dbo.GenerateFeedbackTransactions @sentDateAgeDays, @specifiedUtcDate",
                        parameterSentDateAgeDays, parameterSpecifiedUtcDate)
                    .ToListAsync(cancellationToken);

                return result.FirstOrDefault();
            }
            finally
            {
                // reset the command timeout to its original value
                Database.SetCommandTimeout(originalTimeout);
            }
        }

        public async Task ImportIntoFeedbackTargetVariantFromStaging()
        {
            var originalTimeout = Database.GetCommandTimeout();

            try
            {
                // set command timeout to 20 minutes (1200 seconds)
                Database.SetCommandTimeout(1200);

                await Database.ExecuteSqlRawAsync("EXEC [dbo].[ImportIntoFeedbackTargetVariant_FromFeedbackTargetVariant_Staging]");
            }
            finally
            {
                // reset the command timeout to its original value
                Database.SetCommandTimeout(originalTimeout);
            }
        }

        public async Task ClearFeedbackTargetVariant_Staging()
        {
            var originalTimeout = Database.GetCommandTimeout();

            try
            {
                // set command timeout to 20 minutes (1200 seconds)
                Database.SetCommandTimeout(1200);

                await Database.ExecuteSqlRawAsync("EXEC [dbo].[TruncateFeedbackTargetVariant_Staging]");
            }
            finally
            {
                // reset the command timeout to its original value
                Database.SetCommandTimeout(originalTimeout);
            }
        }

        private string GetAccessToken()
        {
            if (_chainedTokenCredentialProvider != null)
            {
                var valueTask = _chainedTokenCredentialProvider.GetTokenAsync(new TokenRequestContext(new[] { AzureResource }));
                if (valueTask.IsCompletedSuccessfully)
                {
                    return valueTask.Result.Token;
                }
                else
                {
                    return valueTask.AsTask().GetAwaiter().GetResult().Token;
                }
            }

            return null;
        }
    }
}
