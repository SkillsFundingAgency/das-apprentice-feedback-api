﻿using Microsoft.EntityFrameworkCore;
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
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq;
using System.Collections.Generic;

namespace SFA.DAS.ApprenticeFeedback.Data
{
    public class ApprenticeFeedbackDataContext : DbContext, 
        IApprenticeFeedbackTargetContext,
        IApprenticeFeedbackResultContext,
        IProviderAttributeContext,
        IAttributeContext
    {
        private const string AzureResource = "https://database.windows.net/";
        private readonly ApplicationSettings _configuration;
        private readonly AzureServiceTokenProvider _azureServiceTokenProvider;

        public virtual DbSet<Domain.Entities.Attribute> Attributes { get; set; }
        public virtual DbSet<ApprenticeFeedbackTarget> ApprenticeFeedbackTargets { get; set; } = null!;
        public virtual DbSet<ApprenticeFeedbackResult> ApprenticeFeedbackResults { get; set; } = null!;
        public virtual DbSet<ProviderAttribute> ProviderAttributes { get; set; } = null!;


        DbSet<ApprenticeFeedbackTarget> IEntityContext<ApprenticeFeedbackTarget>.Entities => ApprenticeFeedbackTargets;
        DbSet<ApprenticeFeedbackResult> IEntityContext<ApprenticeFeedbackResult>.Entities => ApprenticeFeedbackResults;
        DbSet<Domain.Entities.Attribute> IEntityContext<Domain.Entities.Attribute>.Entities => Attributes;
        DbSet<ProviderAttribute> IEntityContext<ProviderAttribute>.Entities => ProviderAttributes;


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
            modelBuilder.Entity<FeedbackForProvidersResult>().HasNoKey();
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

        public async Task<IEnumerable<FeedbackForProvidersResult>> GetFeedbackForProvidersAsync(long[] ukPrns)
        {
            var parameterRecentFeedbackMonths = new SqlParameter
            {
                ParameterName = "recentFeedbackMonths",
                SqlDbType = System.Data.SqlDbType.Int,
                Value = 12,
            };

            var parameterMinimumNumberOfReviews = new SqlParameter
            {
                ParameterName = "minimumNumberOfReviews",
                SqlDbType = System.Data.SqlDbType.Int,
                Value = 0,
            };

            var result = await Set<FeedbackForProvidersResult>()
                .FromSqlRaw("EXEC [dbo].[GetFeedbackForProviders] @recentFeedbackMonths, @minimumNumberOfReviews", parameterRecentFeedbackMonths, parameterMinimumNumberOfReviews)
                .ToListAsync();

            if(null != ukPrns && ukPrns.Length > 0)
            {
                result = result.Where(sr => ukPrns.Contains(sr.Ukprn)).ToList();
            }

            return result;
        }
    }
}
