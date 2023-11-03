
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.ApprenticeFeedback.Application.Commands.GenerateFeedbackTransactions;
using SFA.DAS.ApprenticeFeedback.Domain.Configuration;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
using System.Threading;
using System.Threading.Tasks;


namespace SFA.DAS.ApprenticeFeedback.Application.UnitTests.Commands
{
    public class WhenHandlingGenerateFeedbackTransactionsCommand
    {
        [Test, AutoMoqData]
        public async Task And_CommandIsValid_Then_Invokes_Generate_method(
           GenerateFeedbackTransactionsCommand command,
           [Frozen] ApplicationSettings settings,
           [Frozen] Mock<IFeedbackTransactionContext> dataContext,
           GenerateFeedbackTransactionsCommandHandler handler)
        {
            //Act
            var result = await handler.Handle(command, CancellationToken.None);

            result.Should().BeOfType<GenerateFeedbackTransactionsCommandResponse>();
            dataContext.Verify(s => s.GenerateFeedbackTransactionsAsync(settings.FeedbackTransactionSentDateAgeDays), Times.Once);
        }
    }
}
