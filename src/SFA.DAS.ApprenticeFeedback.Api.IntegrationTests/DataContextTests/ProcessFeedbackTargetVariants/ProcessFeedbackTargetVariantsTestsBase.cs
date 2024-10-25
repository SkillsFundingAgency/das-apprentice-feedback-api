using FluentAssertions;
using SFA.DAS.ApprenticeFeedback.Api.IntegrationTests.Handlers;
using SFA.DAS.ApprenticeFeedback.Api.IntegrationTests.Models;
using SFA.DAS.ApprenticeFeedback.Api.IntegrationTests.Services;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Api.IntegrationTests.DataContextTests.ProcessFeedbackTargetVariants
{
    public class ProcessFeedbackTargeVariantsTestsBase : TestsBase
    {
        protected class FeedbackTargetVariantFixture : IDisposable
        {
            private readonly List<FeedbackTargetVariantModel> _feedbackTargetVariants = new List<FeedbackTargetVariantModel>();
            private readonly List<FeedbackTargetVariantModel> _feedbackTargetVariantStaging = new List<FeedbackTargetVariantModel>();

            private readonly DatabaseService _databaseService = new DatabaseService();
            private readonly IFeedbackTargetVariant_StagingContext _feedbackTargetVariantStagingContext;
            private readonly IFeedbackTargetVariantContext _feedbackTargetVariantContext;

            public FeedbackTargetVariantFixture()
            {
                _feedbackTargetVariantStagingContext = _databaseService.TestContext;
                _feedbackTargetVariantContext = _databaseService.TestContext;
                DeleteAllRecords();
            }

            public FeedbackTargetVariantFixture WithFeedbackTargetVariant(long apprenticeshipId, string variant)
            {
                var feedbackTargetVariant = new FeedbackTargetVariantModel
                {
                    ApprenticeshipId = apprenticeshipId,
                    Variant = variant
                };

                _feedbackTargetVariants.Add(feedbackTargetVariant);
                FeedbackTargetVariantHandler.InsertRecord(feedbackTargetVariant);

                return this;
            }

            public FeedbackTargetVariantFixture WithFeedbackTargetVariantStaging(long apprenticeshipId, string variant)
            {
                var feedbackTargetVariantStaging = new FeedbackTargetVariantModel
                {
                    ApprenticeshipId = apprenticeshipId,
                    Variant = variant
                };

                _feedbackTargetVariantStaging.Add(feedbackTargetVariantStaging);
                FeedbackTargetVariantStagingHandler.InsertRecord(feedbackTargetVariantStaging);

                return this;
            }

            public async Task TruncateStagingData()
            {
                await _feedbackTargetVariantStagingContext.ClearFeedbackTargetVariant_Staging();
            }

            public async Task ImportIntoFeedbackTargetVariantFromStaging()
            {
                await _feedbackTargetVariantContext.ImportIntoFeedbackTargetVariantFromStaging();
            }

            // Verification method to check if a FeedbackTargetVariant record exists in the database
            public async Task<FeedbackTargetVariantFixture> VerifyFeedbackTargetVariantExists(FeedbackTargetVariantModel feedbackTargetVariant)
            {
                var result = await FeedbackTargetVariantHandler.QueryFirstOrDefaultAsync(feedbackTargetVariant);
                result.Should().NotBeNull(FeedbackTargetVariantHandler.Because(feedbackTargetVariant));

                return this;
            }

            // Verification method to check the number of FeedbackTargetVariant records
            public async Task<FeedbackTargetVariantFixture> VerifyFeedbackTargetVariantRowCount(int count)
            {
                var result = await FeedbackTargetVariantHandler.QueryCountAllAsync();
                result.Should().Be(count);

                return this;
            }

            // Verification method to check if a FeedbackTargetVariant_Staging record exists in the database
            public async Task<FeedbackTargetVariantFixture> VerifyFeedbackTargetVariantStagingExists(FeedbackTargetVariantModel feedbackTargetVariantStaging)
            {
                var result = await FeedbackTargetVariantStagingHandler.QueryFirstOrDefaultAsync(feedbackTargetVariantStaging);
                result.Should().NotBeNull(FeedbackTargetVariantStagingHandler.Because(feedbackTargetVariantStaging));

                return this;
            }

            // Verification method to check the number of FeedbackTargetVariant_Staging records
            public async Task<FeedbackTargetVariantFixture> VerifyFeedbackTargetVariantStagingRowCount(int count)
            {
                var result = await FeedbackTargetVariantStagingHandler.QueryCountAllAsync();
                result.Should().Be(count);

                return this;
            }

            // Method to delete all records from both the FeedbackTargetVariant and FeedbackTargetVariant_Staging tables
            public void Dispose()
            {
                DeleteAllRecords();
            }

            protected static void DeleteAllRecords()
            {
                FeedbackTargetVariantHandler.DeleteAllRecords();
                FeedbackTargetVariantStagingHandler.DeleteAllRecords();
            }
        }
    }
}
