using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.ApprenticeFeedback.Application.Queries.GetAttributes;
using SFA.DAS.ApprenticeFeedback.Data;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Application.UnitTests.Queries
{
    public class WhenRequestingFeedbackAttributes
    {
        [Test, AutoMoqData]
        public async Task ThenAttributesAreReturned(
            GetAttributesQuery query,
            [Frozen(Matching.ImplementedInterfaces)] ApprenticeFeedbackDataContext context,
            GetAttributesQueryHandler handler,
            List<Domain.Entities.Attribute> response)
        {
            context.Attributes.AddRange(response);
            context.SaveChanges();

            query.AttributeType = response[0].AttributeType;
            var result = await handler.Handle(query, CancellationToken.None);

            result.ProviderAttributes.Count.Should().Be(1);
        }
    }
}
