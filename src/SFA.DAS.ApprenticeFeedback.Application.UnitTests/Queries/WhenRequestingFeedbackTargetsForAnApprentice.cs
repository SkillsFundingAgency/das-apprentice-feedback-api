using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.ApprenticeFeedback.Application.Queries.GetApprenticeFeedbackTargets;
using SFA.DAS.ApprenticeFeedback.Application.Queries.GetAttributes;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Application.UnitTests.Queries
{
    public class WhenRequestingFeedbackTargetsForAnApprentice
    {
        [Test, RecursiveMoqAutoData]
        public async Task ThenFeedbackTargetsAreReturned(
            GetApprenticeFeedbackTargetsQuery query,
            [Frozen] Mock<IApprenticeFeedbackRepository> mockRepository,
            [Frozen] Mock<IApprenticeFeedbackTargetDataContext> mockApprenticeFeedbackTargetDataContext,
            GetApprenticeFeedbackTargetsQueryHandler handler,
            List<Domain.Entities.ApprenticeFeedbackTarget> response)
        {
            mockApprenticeFeedbackTargetDataContext.Setup( s => s.GetApprenticeFeedbackTargetsAsync(query.ApprenticeId)).ReturnsAsync(response);

            var result = await handler.Handle(query, CancellationToken.None);

            result.ApprenticeFeedbackTargets.Should().BeEquivalentTo(response.Select(s => (Domain.Models.ApprenticeFeedbackTarget)s));
        }
    }
}
