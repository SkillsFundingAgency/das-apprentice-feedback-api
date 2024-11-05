using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Api.IntegrationTests.DataContextTests.ProcessFeedbackTargetVariants
{
    [TestFixture]
    public class FeedbackTargetVariantStagingTests : ProcessFeedbackTargeVariantsTestsBase
    {
        private FeedbackTargetVariantFixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _fixture = new FeedbackTargetVariantFixture();
        }

        [Test]
        public async Task TruncateFeedbackTargetVariantStaging_Should_Clear_All_Records()
        {
            // Arrange
            _fixture.WithFeedbackTargetVariantStaging(12345, "Test Variant 1")
                    .WithFeedbackTargetVariantStaging(12346, "Test Variant 2")
                    .WithFeedbackTargetVariantStaging(12347, "Test Variant 3");

            await _fixture.VerifyFeedbackTargetVariantStagingRowCount(3);

            // Act
            await _fixture.TruncateStagingData();

            // Assert
            await _fixture.VerifyFeedbackTargetVariantStagingRowCount(0);
        }


        [Test]
        public async Task TruncateFeedbackTargetVariantStaging_Should_Not_Throw_On_Empty_Table()
        {
            // Arrange
            await _fixture.VerifyFeedbackTargetVariantStagingRowCount(0);

            // Act
            await _fixture.TruncateStagingData();

            // Assert
            await _fixture.VerifyFeedbackTargetVariantStagingRowCount(0);
        }

        [Test]
        public async Task TruncateFeedbackTargetVariantStaging_Should_Not_Affect_Main_Table()
        {
            // Arrange
            _fixture.WithFeedbackTargetVariantStaging(12345, "Staging Variant")
                    .WithFeedbackTargetVariant(12345, "Main Table Variant");

            await _fixture.VerifyFeedbackTargetVariantRowCount(1);  
            await _fixture.VerifyFeedbackTargetVariantStagingRowCount(1);  

            // Act
            await _fixture.TruncateStagingData();

            // Assert
            await _fixture.VerifyFeedbackTargetVariantStagingRowCount(0);
            await _fixture.VerifyFeedbackTargetVariantRowCount(1);
        }

        [TearDown]
        public void TearDown()
        {
            // Clean up resources and reset the state
            _fixture.Dispose();
        }
    }
}
