using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.ApprenticeFeedback.Application.Commands.CreateOutcomeRecordedFeedbackTransaction;
using SFA.DAS.ApprenticeFeedback.Domain.Configuration;
using SFA.DAS.ApprenticeFeedback.Domain.Entities;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Application.UnitTests.Commands
{
    public class WhenHandlingCreateOutcomeRecordedFeedbackTransactionCommand
    {
        private const string TemplateName = "Outcome";
        private const long ApprenticeshipId = 123;

        private Mock<IApprenticeFeedbackTargetContext> _targetContext;
        private Mock<IFeedbackTransactionContext> _transactionContext;
        private Mock<IDateTimeHelper> _dateTimeHelper;
        private DateTime _now;
        private Guid _targetId;

        private CreateOutcomeRecordedFeedbackTransactionCommandHandler CreateSut(string outcomeTemplate, params NotificationTemplate[] templates)
        {
            _targetContext = new Mock<IApprenticeFeedbackTargetContext>();
            _transactionContext = new Mock<IFeedbackTransactionContext>();
            _dateTimeHelper = new Mock<IDateTimeHelper>();
            _now = DateTime.UtcNow;
            _targetId = Guid.NewGuid();
            _dateTimeHelper.Setup(x => x.Now).Returns(_now);

            var settings = new ApplicationSettings
            {
                ApprenticeshipOutcomeTemplateName = outcomeTemplate,
                NotificationTemplates = new List<NotificationTemplate>(templates)
            };
            return new CreateOutcomeRecordedFeedbackTransactionCommandHandler(
                _targetContext.Object, _transactionContext.Object, _dateTimeHelper.Object, settings,
                Mock.Of<ILogger<CreateOutcomeRecordedFeedbackTransactionCommandHandler>>());
        }

        private CreateOutcomeRecordedFeedbackTransactionCommandHandler CreateValidSut() =>
            CreateSut(TemplateName, new NotificationTemplate { TemplateName = TemplateName, TemplateId = Guid.NewGuid() });

        private static CreateOutcomeRecordedFeedbackTransactionCommand Command() =>
            new CreateOutcomeRecordedFeedbackTransactionCommand { ApprenticeshipId = ApprenticeshipId };

        [Test]
        public async Task And_TargetNotFound_Then_ReturnsNotFound()
        {
            var sut = CreateValidSut();
            _targetContext.Setup(x => x.FindByApprenticeshipId(ApprenticeshipId)).ReturnsAsync((ApprenticeFeedbackTarget)null);

            var response = await sut.Handle(Command(), CancellationToken.None);

            response.NotFound.Should().BeTrue();
            _transactionContext.Verify(x => x.AddFeedbackTransaction(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<DateTime>()), Times.Never);
        }

        [Test]
        public async Task And_UnsentTransactionExists_Then_DoesNotInsert()
        {
            var sut = CreateValidSut();
            _targetContext.Setup(x => x.FindByApprenticeshipId(ApprenticeshipId)).ReturnsAsync(new ApprenticeFeedbackTarget { Id = _targetId });
            _transactionContext.Setup(x => x.HasUnsentTransaction(_targetId, TemplateName)).ReturnsAsync(true);

            var response = await sut.Handle(Command(), CancellationToken.None);

            response.NotFound.Should().BeFalse();
            _transactionContext.Verify(x => x.AddFeedbackTransaction(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<DateTime>()), Times.Never);
        }

        [Test]
        public async Task And_NoUnsentTransaction_Then_InsertsFeedbackTransactionWithConfiguredTemplate()
        {
            var sut = CreateValidSut();
            _targetContext.Setup(x => x.FindByApprenticeshipId(ApprenticeshipId)).ReturnsAsync(new ApprenticeFeedbackTarget { Id = _targetId });
            _transactionContext.Setup(x => x.HasUnsentTransaction(_targetId, TemplateName)).ReturnsAsync(false);

            var response = await sut.Handle(Command(), CancellationToken.None);

            response.NotFound.Should().BeFalse();
            _transactionContext.Verify(x => x.AddFeedbackTransaction(_targetId, TemplateName, _now), Times.Once);
        }

        [Test]
        public void And_RepositoryThrows_Then_ExceptionIsRethrown()
        {
            var sut = CreateValidSut();
            _targetContext.Setup(x => x.FindByApprenticeshipId(ApprenticeshipId)).ThrowsAsync(new InvalidOperationException("boom"));

            Func<Task> act = () => sut.Handle(Command(), CancellationToken.None);

            act.Should().ThrowAsync<InvalidOperationException>().GetAwaiter().GetResult();
        }

        [TestCase(null)]
        [TestCase("")]
        public void And_OutcomeTemplateNameMissing_Then_Throws(string name)
        {
            var sut = CreateSut(name, new NotificationTemplate { TemplateName = TemplateName, TemplateId = Guid.NewGuid() });

            Func<Task> act = () => sut.Handle(Command(), CancellationToken.None);

            act.Should().ThrowAsync<InvalidOperationException>().GetAwaiter().GetResult();
            _targetContext.Verify(x => x.FindByApprenticeshipId(It.IsAny<long>()), Times.Never);
        }

    }
}
