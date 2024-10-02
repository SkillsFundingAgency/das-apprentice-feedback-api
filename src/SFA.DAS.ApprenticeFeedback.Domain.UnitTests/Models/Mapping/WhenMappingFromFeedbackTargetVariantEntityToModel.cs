using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.ApprenticeFeedback.Domain.Entities;

namespace SFA.DAS.ApprenticeFeedback.Domain.UnitTests.Models
{
    public class WhenMappingFromFeedbackTargetVariantModelToEntity
    {
        [Test]
        public void ThenTheFieldsAreCorrectlyMappedToFeedbackTargetVariantModel(Domain.Models.FeedbackTargetVariant source)
        {
            var result = (FeedbackTargetVariant)source;

            result.ApprenticeshipId.Should().Be(source.ApprenticeshipId);
            result.Variant.Should().Be(source.Variant);
        }

        [Test]
        public void ThenTheFieldsAreCorrectlyMappedToFeedbackTargetVariantModel_Staging(Domain.Models.FeedbackTargetVariant source)
        {
            var result = (FeedbackTargetVariant_Staging)source;

            result.ApprenticeshipId.Should().Be(source.ApprenticeshipId);
            result.Variant.Should().Be(source.Variant);
        }
    }
}
