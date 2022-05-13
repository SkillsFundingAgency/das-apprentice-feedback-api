using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using SFA.DAS.ApprenticeFeedback.Application.Commands.CreateApprenticeFeedbackTarget;
using SFA.DAS.ApprenticeFeedback.Domain.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Api.IntegrationTests.Tests
{
    public class WhenCreatingApprenticeFeedbackTarget
    {
        private TestFixture _fixture;

        [OneTimeSetUp]
        public void Setup()
        {
            _fixture = new TestFixture();
        }

        [Test]
        public async Task Then_ApprenticeFeedbackTargetIsAdded()
        {
            var command = new CreateApprenticeFeedbackTargetCommand
            {
                ApprenticeId = Guid.NewGuid(),
                CommitmentApprenticeshipId = 1,
                ApprenticeshipId = 2
            };

            var response = await _fixture.SendAsync(command);

            var created = await _fixture.ExecuteDbContextAsync(db => db.ApprenticeFeedbackTargets.Where(c => c.Id == response.ApprenticeFeedbackTargetId).SingleOrDefaultAsync());

            created.Should().NotBeNull();
            created.ApprenticeId.Should().Be(command.ApprenticeId);
            created.ApprenticeshipId.Should().Be(command.CommitmentApprenticeshipId);
        }
    }
}