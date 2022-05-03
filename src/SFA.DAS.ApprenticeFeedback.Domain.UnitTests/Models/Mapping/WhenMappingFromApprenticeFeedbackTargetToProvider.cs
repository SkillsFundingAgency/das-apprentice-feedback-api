using NUnit.Framework;
using FluentAssertions;
using SFA.DAS.Testing.AutoFixture;
using SFA.DAS.ApprenticeFeedback.Domain.Models;

namespace SFA.DAS.ApprenticeFeedback.Domain.UnitTests.Models
{
    public class WhenMappingFromApprenticeFeedbackTargetToProvider
    {
        [Test, RecursiveMoqAutoData]
        public void ThenTheFieldsAreCorrectlyMapped(ApprenticeFeedbackTarget source)
        {
            var result = (TrainingProvider)source;

            result.ApprenticeFeedbackTargetId.Should().Be(source.Id.Value);
            result.Ukprn.Should().Be(source.Ukprn);
            result.StartDate.Should().Be(source.StartDate);
            result.EndDate.Should().Be(source.EndDate);
            result.ProviderName.Should().Be(source.ProviderName);
            result.Status.Should().Be(source.Status);
            result.FeedbackEligibility.Should().Be(source.FeedbackEligibility);
        }
    }
}
