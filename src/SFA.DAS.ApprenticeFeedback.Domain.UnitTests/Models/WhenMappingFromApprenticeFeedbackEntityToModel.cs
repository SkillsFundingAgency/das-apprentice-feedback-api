using NUnit.Framework;
using SFA.DAS.ApprenticeFeedback.Domain.Entities;
using FluentAssertions;
using SFA.DAS.Testing.AutoFixture;
using System;
using static SFA.DAS.ApprenticeFeedback.Domain.Models.ApprenticeFeedbackTarget;

namespace SFA.DAS.ApprenticeFeedback.Domain.UnitTests.Models
{
    public class WhenMappingFromApprenticeFeedbackEntityToModel
    {
        [Test, MoqAutoData]
        public void ThenTheFieldsAreCorrectlyMapped(ApprenticeFeedbackTarget source)
        {
            var result = (Domain.Models.ApprenticeFeedbackTarget)source;

            result.ApprenticeId.Should().Be(source.ApprenticeId);
            result.ApprenticeshipId.Should().Be(source.ApprenticeshipId);
            result.Id.Should().Be(source.Id);
            result.Status.Should().Be(((FeedbackTargetStatus)source.Status));
            result.StartDate.Should().Be(source.StartDate);
            result.EndDate.Should().Be(source.EndDate); 
        }
    }
}
