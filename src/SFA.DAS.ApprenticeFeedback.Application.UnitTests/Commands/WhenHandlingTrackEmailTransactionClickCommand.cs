using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using Moq;
using NUnit.Framework;
using SFA.DAS.ApprenticeFeedback.Application.Commands.ProcessEmailTransaction;
using SFA.DAS.ApprenticeFeedback.Application.Commands.TrackEmailTransactionClick;
using SFA.DAS.ApprenticeFeedback.Data;
using SFA.DAS.ApprenticeFeedback.Domain.Entities;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static SFA.DAS.ApprenticeFeedback.Domain.Models.Enums;

namespace SFA.DAS.ApprenticeFeedback.Application.UnitTests.Commands
{
    [TestFixture]
    public class TrackEmailTransactionClickCommandHandlerTests
    {
        [Test, AutoMoqData]
        public async Task And_TransactionIdDoesNotExist_Then_ReturnClickStatusInvalid(
           TrackEmailTransactionClickCommand command,
           [Frozen(Matching.ImplementedInterfaces)] ApprenticeFeedbackDataContext context,
           TrackEmailTransactionClickCommandHandler handler
           )
        {
            // Arrange

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().Match<TrackEmailTransactionClickResponse>(p => 
                p.FeedbackTransactionId == command.FeedbackTransactionId &&
                p.ClickStatus == ClickStatus.Invalid);
        }

        [Test, AutoMoqData]
        public async Task And_ApprenticeFeedbackTargetIdDoesNotMatch_Then_ReturnClickStatusInvalid(
           TrackEmailTransactionClickCommand command,
           [Frozen(Matching.ImplementedInterfaces)] ApprenticeFeedbackDataContext context,
           TrackEmailTransactionClickCommandHandler handler
           )
        {
            // Arrange
            var feedbackTarget = new ApprenticeFeedbackTarget()
            {
                Id = Guid.NewGuid(),
            };

            var feedbackTransaction = new FeedbackTransaction()
            {
                Id = command.FeedbackTransactionId,
                ApprenticeFeedbackTargetId = feedbackTarget.Id
            };

            context.Add(feedbackTarget);
            context.Add(feedbackTransaction);
            await context.SaveChangesAsync();

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().Match<TrackEmailTransactionClickResponse>(p =>
                p.FeedbackTransactionId == command.FeedbackTransactionId &&
                p.ClickStatus == ClickStatus.Invalid);
        }


        [Test, AutoMoqData]
        public async Task And_ApprenticeFeedbackTargetIdDoesMatch_Then_ReturnClickStatusValid(
           TrackEmailTransactionClickCommand command,
           [Frozen(Matching.ImplementedInterfaces)] ApprenticeFeedbackDataContext context,
           TrackEmailTransactionClickCommandHandler handler
           )
        {
            // Arrange
            var feedbackTarget = new ApprenticeFeedbackTarget()
            {
                Id = command.ApprenticeFeedbackTargetId,
            };

            var feedbackTransaction = new FeedbackTransaction()
            {
                Id = command.FeedbackTransactionId,
                ApprenticeFeedbackTargetId = feedbackTarget.Id
            };

            context.Add(feedbackTarget);
            context.Add(feedbackTransaction);
            await context.SaveChangesAsync();

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().Match<TrackEmailTransactionClickResponse>(p =>
                p.FeedbackTransactionId == command.FeedbackTransactionId &&
                p.ClickStatus == ClickStatus.Valid);
        }

        [Test, AutoMoqData]
        public async Task And_ApprenticeFeedbackTargetIdDoesMatch_Then_CreatesFeedbackTransactionClick(
           TrackEmailTransactionClickCommand command,
           [Frozen(Matching.ImplementedInterfaces)] ApprenticeFeedbackDataContext context,
           TrackEmailTransactionClickCommandHandler handler
           )
        {
            // Arrange
            var feedbackTarget = new ApprenticeFeedbackTarget()
            {
                Id = command.ApprenticeFeedbackTargetId,
            };

            var feedbackTransaction = new FeedbackTransaction()
            {
                Id = command.FeedbackTransactionId,
                ApprenticeFeedbackTargetId = feedbackTarget.Id
            };

            context.Add(feedbackTarget);
            context.Add(feedbackTransaction);
            await context.SaveChangesAsync();

            command.LinkName = "SomeLinkName";
            command.LinkUrl = "https://somelink.com";
            command.ClickedOn = DateTime.Now;

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            context.FeedbackTransactionClicks.Should().HaveCount(1);
            context.FeedbackTransactionClicks.First().Should().BeEquivalentTo(new FeedbackTransactionClick
            {
                FeedbackTransaction = feedbackTransaction,
                FeedbackTransactionId = command.FeedbackTransactionId,
                ApprenticeFeedbackTargetId = command.ApprenticeFeedbackTargetId,
                LinkName = command.LinkName,
                LinkUrl = command.LinkUrl,
                ClickedOn = command.ClickedOn
            }, options => options
                .Excluding(p => p.Id)
                .Excluding(p => p.FeedbackTransaction)
                .Excluding(p => p.ApprenticeFeedbackTarget)
                .Excluding(p => p.CreatedOn)
                .Excluding(p => p.UpdatedOn));
        }

    }
}

