using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.ApprenticeFeedback.Application.Commands.CreateFeedbackTransaction;
using SFA.DAS.ApprenticeFeedback.Domain.Configuration;
using SFA.DAS.ApprenticeFeedback.Domain.Entities;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Application.UnitTests.Commands
{
    public class WhenHandlingCreateFeedbackTransactionCommand
    {
        [Test, AutoMoqData]
        public async Task And_ApprenticeshipTargetNotFound_Then_ReturnsNotFound(
            CreateFeedbackTransactionCommand command,
            [Frozen] Mock<IApprenticeFeedbackTargetContext> apprenticeFeedbackTargetContext,
            CreateFeedbackTransactionCommandHandler handler)
        {
            apprenticeFeedbackTargetContext
                .Setup(x => x.FindByApprenticeshipId(command.ApprenticeshipId))
                .ReturnsAsync((ApprenticeFeedbackTarget)null);

            var response = await handler.Handle(command, CancellationToken.None);

            response.NotFound.Should().BeTrue();
        }

        [Test, AutoMoqData]
        public async Task And_TemplateNotFound_Then_ReturnsNotFound(
            CreateFeedbackTransactionCommand command,
            [Frozen] ApplicationSettings settings,
            [Frozen] Mock<IApprenticeFeedbackTargetContext> apprenticeFeedbackTargetContext,
            CreateFeedbackTransactionCommandHandler handler)
        {
            apprenticeFeedbackTargetContext
                .Setup(x => x.FindByApprenticeshipId(command.ApprenticeshipId))
                .ReturnsAsync(new ApprenticeFeedbackTarget { Id = Guid.NewGuid() });
            settings.NotificationTemplates = new List<NotificationTemplate>();

            var response = await handler.Handle(command, CancellationToken.None);

            response.NotFound.Should().BeTrue();
        }

        [Test, AutoMoqData]
        public async Task And_UnsentTransactionAlreadyExists_Then_DoesNotInsert(
            CreateFeedbackTransactionCommand command,
            [Frozen] ApplicationSettings settings,
            [Frozen] Mock<IApprenticeFeedbackTargetContext> apprenticeFeedbackTargetContext,
            [Frozen] Mock<IFeedbackTransactionContext> feedbackTransactionContext,
            CreateFeedbackTransactionCommandHandler handler)
        {
            var apprenticeFeedbackTargetId = Guid.NewGuid();
            var templateId = Guid.NewGuid();

            apprenticeFeedbackTargetContext
                .Setup(x => x.FindByApprenticeshipId(command.ApprenticeshipId))
                .ReturnsAsync(new ApprenticeFeedbackTarget { Id = apprenticeFeedbackTargetId });
            settings.NotificationTemplates = new List<NotificationTemplate>
            {
                new NotificationTemplate { TemplateName = command.TemplateName, TemplateId = templateId }
            };
            feedbackTransactionContext
                .Setup(x => x.HasUnsentTransaction(apprenticeFeedbackTargetId, templateId))
                .ReturnsAsync(true);

            var response = await handler.Handle(command, CancellationToken.None);

            response.NotFound.Should().BeFalse();
            feedbackTransactionContext.Verify(x => x.AddFeedbackTransaction(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<DateTime>()), Times.Never);
        }

        [Test, AutoMoqData]
        public async Task And_UnsentTransactionDoesNotExist_Then_InsertsFeedbackTransaction(
            CreateFeedbackTransactionCommand command,
            [Frozen] ApplicationSettings settings,
            [Frozen] Mock<IApprenticeFeedbackTargetContext> apprenticeFeedbackTargetContext,
            [Frozen] Mock<IFeedbackTransactionContext> feedbackTransactionContext,
            [Frozen] Mock<IDateTimeHelper> dateTimeHelper,
            CreateFeedbackTransactionCommandHandler handler)
        {
            var apprenticeFeedbackTargetId = Guid.NewGuid();
            var templateId = Guid.NewGuid();
            var now = DateTime.UtcNow;

            apprenticeFeedbackTargetContext
                .Setup(x => x.FindByApprenticeshipId(command.ApprenticeshipId))
                .ReturnsAsync(new ApprenticeFeedbackTarget { Id = apprenticeFeedbackTargetId });
            settings.NotificationTemplates = new List<NotificationTemplate>
            {
                new NotificationTemplate { TemplateName = command.TemplateName, TemplateId = templateId }
            };
            feedbackTransactionContext
                .Setup(x => x.HasUnsentTransaction(apprenticeFeedbackTargetId, templateId))
                .ReturnsAsync(false);
            dateTimeHelper.Setup(x => x.Now).Returns(now);

            var response = await handler.Handle(command, CancellationToken.None);

            response.NotFound.Should().BeFalse();
            feedbackTransactionContext.Verify(x => x.AddFeedbackTransaction(apprenticeFeedbackTargetId, templateId, now), Times.Once);
        }

        [Test, AutoMoqData]
        public async Task And_OnlySentTransactionExists_Then_InsertsFeedbackTransaction(
            CreateFeedbackTransactionCommand command,
            [Frozen] ApplicationSettings settings,
            [Frozen] Mock<IApprenticeFeedbackTargetContext> apprenticeFeedbackTargetContext,
            [Frozen] Mock<IFeedbackTransactionContext> feedbackTransactionContext,
            [Frozen] Mock<IDateTimeHelper> dateTimeHelper,
            CreateFeedbackTransactionCommandHandler handler)
        {
            var apprenticeFeedbackTargetId = Guid.NewGuid();
            var templateId = Guid.NewGuid();
            var now = DateTime.UtcNow;

            apprenticeFeedbackTargetContext
                .Setup(x => x.FindByApprenticeshipId(command.ApprenticeshipId))
                .ReturnsAsync(new ApprenticeFeedbackTarget { Id = apprenticeFeedbackTargetId });
            settings.NotificationTemplates = new List<NotificationTemplate>
            {
                new NotificationTemplate { TemplateName = command.TemplateName, TemplateId = templateId }
            };

            // HasUnsentTransaction returns false for the "sent-only" case.
            feedbackTransactionContext
                .Setup(x => x.HasUnsentTransaction(apprenticeFeedbackTargetId, templateId))
                .ReturnsAsync(false);
            dateTimeHelper.Setup(x => x.Now).Returns(now);

            var response = await handler.Handle(command, CancellationToken.None);

            response.NotFound.Should().BeFalse();
            feedbackTransactionContext.Verify(x => x.AddFeedbackTransaction(apprenticeFeedbackTargetId, templateId, now), Times.Once);
        }
    }
}
