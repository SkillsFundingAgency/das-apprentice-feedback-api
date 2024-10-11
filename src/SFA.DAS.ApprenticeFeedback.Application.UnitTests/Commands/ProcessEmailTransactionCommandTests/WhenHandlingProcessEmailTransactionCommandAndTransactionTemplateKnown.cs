using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NServiceBus;
using NUnit.Framework;
using SFA.DAS.ApprenticeFeedback.Application.Commands.ProcessEmailTransaction;
using SFA.DAS.ApprenticeFeedback.Application.Services;
using SFA.DAS.ApprenticeFeedback.Domain.Entities;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Application.UnitTests.Commands.ProcessEmailTransactionCommandTests
{
    [TestFixture]
    public class WhenHandlingProcessEmailTransactionCommandAndTransactionTemplateKnown 
        : WhenHandlingProcessEmailTransactionCommandBase
    {
        [AutoMoqInlineAutoData("AppStart")]
        [AutoMoqInlineAutoData("AppWelcome")]
        [AutoMoqInlineAutoData("AppMonthThree")]
        [AutoMoqInlineAutoData("AppMonthSix")]
        [AutoMoqInlineAutoData("AppMonthNine")]
        [AutoMoqInlineAutoData("AppMonthTwelve")]
        [AutoMoqInlineAutoData("AppMonthEighteen")]
        [AutoMoqInlineAutoData("AppAnnual")]
        [AutoMoqInlineAutoData("AppPreEpa")]
        public async Task ButEmailTemplateNotFound_ThenEmailResultFailure(
           string templateName,
           [Frozen] Mock<IFeedbackTransactionContext> feedbackTransactionContext,
           [Frozen] Mock<IExclusionContext> exclusionContext,
           [Frozen] Mock<IEngagementEmailContext> engagementEmailContext,
           [Frozen] Mock<IEmailTemplateService> emailTemplateService,
           ProcessEmailTransactionCommand command,
           ProcessEmailTransactionCommandHandler sut)
        {
            // Arrange
            var feedbackTarget = new ApprenticeFeedbackTarget()
            {
                Id = Guid.NewGuid()
            };

            var feedbackTransaction = new FeedbackTransaction()
            {
                Id = 101,
                ApprenticeFeedbackTargetId = feedbackTarget.Id,
                ApprenticeFeedbackTarget = feedbackTarget,
                TemplateName = templateName
            };

            CommonSetup(feedbackTransactionContext, exclusionContext, engagementEmailContext, null, feedbackTransaction);

            command.FeedbackTransactionId = feedbackTransaction.Id;
            command.IsFeedbackEmailContactAllowed = false;

            emailTemplateService.Setup(p => p.GetEmailTemplateInfoForTransaction(feedbackTransaction, command))
                .ReturnsAsync((null, null, null, new Dictionary<string, string>()));

            // Act
            var result = await sut.Handle(command, CancellationToken.None);

            // Assert
            result.EmailSentStatus.Should().Be(EmailSentStatus.Failed);
        }

        [AutoMoqInlineAutoData("AppStart")]
        [AutoMoqInlineAutoData("AppWelcome")]
        [AutoMoqInlineAutoData("AppMonthThree")]
        [AutoMoqInlineAutoData("AppMonthSix")]
        [AutoMoqInlineAutoData("AppMonthNine")]
        [AutoMoqInlineAutoData("AppMonthTwelve")]
        [AutoMoqInlineAutoData("AppMonthEighteen")]
        [AutoMoqInlineAutoData("AppAnnual")]
        [AutoMoqInlineAutoData("AppPreEpa")]
        public async Task ButEmailTemplateNotFound_ThenDoNotRemoveFeedbackTransaction(
           string templateName,
           [Frozen] Mock<IFeedbackTransactionContext> feedbackTransactionContext,
           [Frozen] Mock<IExclusionContext> exclusionContext,
           [Frozen] Mock<IEngagementEmailContext> engagementEmailContext,
           [Frozen] Mock<IEmailTemplateService> emailTemplateService,
           ProcessEmailTransactionCommand command,
           ProcessEmailTransactionCommandHandler sut)
        {
            // Arrange
            var feedbackTarget = new ApprenticeFeedbackTarget()
            {
                Id = Guid.NewGuid()
            };

            var feedbackTransaction = new FeedbackTransaction()
            {
                Id = 101,
                ApprenticeFeedbackTargetId = feedbackTarget.Id,
                ApprenticeFeedbackTarget = feedbackTarget,
                TemplateName = templateName
            };

            CommonSetup(feedbackTransactionContext, exclusionContext, engagementEmailContext, null, feedbackTransaction);

            command.FeedbackTransactionId = feedbackTransaction.Id;
            command.IsFeedbackEmailContactAllowed = false;

            emailTemplateService.Setup(p => p.GetEmailTemplateInfoForTransaction(feedbackTransaction, command))
                .ReturnsAsync((null, null, null, new Dictionary<string, string>()));

            // Act
            await sut.Handle(command, CancellationToken.None);

            // Assert
            _mockFeedbackTransactionDbSet.Verify(m => m.Remove(It.Is<FeedbackTransaction>(p => p.Id == feedbackTransaction.Id)), Times.Never);
        }

        [AutoMoqInlineAutoData("AppStart")]
        [AutoMoqInlineAutoData("AppWelcome")]
        [AutoMoqInlineAutoData("AppMonthThree")]
        [AutoMoqInlineAutoData("AppMonthSix")]
        [AutoMoqInlineAutoData("AppMonthNine")]
        [AutoMoqInlineAutoData("AppMonthTwelve")]
        [AutoMoqInlineAutoData("AppMonthEighteen")]
        [AutoMoqInlineAutoData("AppAnnual")]
        [AutoMoqInlineAutoData("AppPreEpa")]
        public async Task ButEmailTemplateNotFound_ThenNoEmailSent(
           string templateName,
           [Frozen] Mock<IFeedbackTransactionContext> feedbackTransactionContext,
           [Frozen] Mock<IExclusionContext> exclusionContext,
           [Frozen] Mock<IEngagementEmailContext> engagementEmailContext,
           [Frozen] Mock<IEmailTemplateService> emailTemplateService,
           [Frozen] Mock<IMessageSession> nserviceBusMessageSession,
           ProcessEmailTransactionCommand command,
           ProcessEmailTransactionCommandHandler sut)
        {
            // Arrange
            var feedbackTarget = new ApprenticeFeedbackTarget()
            {
                Id = Guid.NewGuid()
            };

            var feedbackTransaction = new FeedbackTransaction()
            {
                Id = 101,
                ApprenticeFeedbackTargetId = feedbackTarget.Id,
                ApprenticeFeedbackTarget = feedbackTarget,
                TemplateName = templateName
            };

            CommonSetup(feedbackTransactionContext, exclusionContext, engagementEmailContext, null, feedbackTransaction);

            command.FeedbackTransactionId = feedbackTransaction.Id;
            command.IsFeedbackEmailContactAllowed = false;

            emailTemplateService.Setup(p => p.GetEmailTemplateInfoForTransaction(feedbackTransaction, command))
                .ReturnsAsync((null, null, null, new Dictionary<string, string>()));

            // Act
            await sut.Handle(command, CancellationToken.None);

            // Assert
            VerifyDoesNotSendEmail(nserviceBusMessageSession);
        }
    }
}
