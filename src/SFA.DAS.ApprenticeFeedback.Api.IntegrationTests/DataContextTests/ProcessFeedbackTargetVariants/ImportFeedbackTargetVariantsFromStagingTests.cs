﻿using NUnit.Framework;
using SFA.DAS.ApprenticeFeedback.Api.IntegrationTests.Models;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Api.IntegrationTests.DataContextTests.ProcessFeedbackTargetVariants
{
    [TestFixture]
    public class ImportFeedbackTargetVariantsFromStagingTests : ProcessFeedbackTargeVariantsTestsBase
    {
        private FeedbackTargetVariantFixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _fixture = new FeedbackTargetVariantFixture();
        }

        [Test]
        public async Task Should_Insert_New_Records_From_Staging_To_Main()
        {
            // Arrange
            _fixture.WithFeedbackTargetVariantStaging(12345, "New Variant");

            // Act
            await _fixture.ImportIntoFeedbackTargetVariantFromStaging();

            // Assert
            await _fixture.VerifyFeedbackTargetVariantExists(new FeedbackTargetVariantModel
            {
                ApprenticeshipId = 12345,
                Variant = "New Variant"
            });
        }

        [Test]
        public async Task Should_Update_Existing_Records_From_Staging_To_Main()
        {
            // Arrange
            _fixture.WithFeedbackTargetVariant(12345, "Old Variant")
                    .WithFeedbackTargetVariantStaging(12345, "Updated Variant");

            // Act
            await _fixture.ImportIntoFeedbackTargetVariantFromStaging();

            // Assert
            await _fixture.VerifyFeedbackTargetVariantExists(new FeedbackTargetVariantModel
            {
                ApprenticeshipId = 12345,
                Variant = "Updated Variant"  
            });
        }

        [Test]
        public async Task Should_Delete_Records_From_Main_When_Null_In_Staging()
        {
            // Arrange
            _fixture
                .WithFeedbackTargetVariantStaging(12345, null)
                .WithFeedbackTargetVariant(12345, "To Be Deleted");

            // Act
            await _fixture.ImportIntoFeedbackTargetVariantFromStaging();

            // Assert
            await _fixture.VerifyFeedbackTargetVariantRowCount(0);
        }

        [Test]
        public async Task Should_Not_Make_Changes_If_Main_And_Staging_Tables_Match()
        {
            // Arrange
            _fixture.WithFeedbackTargetVariant(12345, "Same Variant")
                    .WithFeedbackTargetVariantStaging(12345, "Same Variant");

            // Act
            await _fixture.ImportIntoFeedbackTargetVariantFromStaging();

            // Assert
            await _fixture.VerifyFeedbackTargetVariantExists(new FeedbackTargetVariantModel
            {
                ApprenticeshipId = 12345,
                Variant = "Same Variant"
            });
        }

        [Test]
        public async Task Should_Retain_Latest_Entry_Staging_Contains_Duplicates()
        {
            // Arrange
            _fixture.WithFeedbackTargetVariantStaging(12345, "Variant")
                    .WithFeedbackTargetVariantStaging(12345, "Different Variant")
                    .WithFeedbackTargetVariantStaging(23451, "Variant")
                    .WithFeedbackTargetVariantStaging(23451, "Different Variant")
                    .WithFeedbackTargetVariantStaging(34512, "Variant");

            // Act
            await _fixture.ImportIntoFeedbackTargetVariantFromStaging();

            // Assert
            await _fixture.VerifyFeedbackTargetVariantExists(new FeedbackTargetVariantModel
            {
                ApprenticeshipId = 12345,
                Variant = "Different Variant"
            });

            await _fixture.VerifyFeedbackTargetVariantExists(new FeedbackTargetVariantModel
            {
                ApprenticeshipId = 23451,
                Variant = "Different Variant"
            });

            await _fixture.VerifyFeedbackTargetVariantExists(new FeedbackTargetVariantModel
            {
                ApprenticeshipId = 34512,
                Variant = "Variant"
            });
        }

        [TearDown]
        public void TearDown()
        {
            // Clean up resources and reset the state
            _fixture.Dispose();
        }
    }

}
