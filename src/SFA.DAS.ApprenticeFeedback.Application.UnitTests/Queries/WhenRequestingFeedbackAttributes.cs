using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.ApprenticeFeedback.Application.Queries.GetAttributes;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Application.UnitTests.Queries
{
    public class WhenRequestingFeedbackAttributes
    {
        [Test, RecursiveMoqAutoData]
        public async Task ThenAttributesAreReturned(
            GetAttributesQuery query,
            [Frozen] Mock<IApprenticeFeedbackRepository> mockRepository,
            GetAttributesQueryHandler handler,
            List<Domain.Entities.Attribute> response)
        {
            mockRepository.Setup( s => s.GetAttributes()).ReturnsAsync(response);

            var result = await handler.Handle(query, CancellationToken.None);

            result.Attributes.Count.Should().Be(response.Count);
        }
    }
}
