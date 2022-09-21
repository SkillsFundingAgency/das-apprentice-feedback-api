
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.ApprenticeFeedback.Domain.Entities;
using SFA.DAS.Testing.AutoFixture;


namespace SFA.DAS.ApprenticeFeedback.Domain.UnitTests.Models
{
    public class WhenMappingFromFeedbackTransactionTargetEntityToModel
    {
        [Test, RecursiveMoqAutoData]
        public void ThenTheFieldsAreCorrectlyMapped(FeedbackTransaction source)
        {
            var result = (Domain.Models.FeedbackTransaction)source;

            result.Id.Should().Be(source.Id);
            result.ApprenticeFeedbackTargetId.Should().Be(source.ApprenticeFeedbackTargetId);
            result.EmailAddress.Should().Be(source.EmailAddress);
            result.FirstName.Should().Be((source.FirstName));
            result.TemplateId.Should().Be(source.TemplateId);
            result.CreatedOn.Should().Be(source.CreatedOn);
            result.SendAfter.Should().Be(source.SendAfter);
            result.SentDate.Should().Be(source.SentDate);
        }
    }
}
