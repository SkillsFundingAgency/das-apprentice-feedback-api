using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.ApprenticeFeedback.Application.Commands.GenerateFeedbackSummaries;
using SFA.DAS.ApprenticeFeedback.Domain.Configuration;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Application.UnitTests.Commands
{
    public class WhenHandlingGenerateFeedbackSummariesCommand
    {
        [Test, AutoMoqData]
        public async Task And_CommandIsValid_Then_Invokes_Generate_method(
           GenerateFeedbackSummariesCommand command,
           [Frozen] ApplicationSettings settings,
           [Frozen] Mock<IProviderRatingSummaryContext> providerRatingSummaryContext,
           GenerateFeedbackSummariesCommandHandler handler
           )
        {
            //Arrange
            //providerRatingSummaryContext.Setup(s => s.GenerateFeedbackSummaries(settings.ReportingMinNumberOfResponses));

            //Act
            var result = await handler.Handle(command, CancellationToken.None);

            result.Should().BeOfType<GenerateFeedbackSummariesCommandResponse>();
            providerRatingSummaryContext.Verify(s => s.GenerateFeedbackSummaries(settings.ReportingMinNumberOfResponses), Times.Once);

        }
    }
}