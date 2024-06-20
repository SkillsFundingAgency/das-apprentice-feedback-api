using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.ApprenticeFeedback.Api.Controllers;
using SFA.DAS.ApprenticeFeedback.Application.Queries.GetApprenticeFeedbackDetailsAnnual;
using SFA.DAS.Testing.AutoFixture;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Api.UnitTests.Controllers.ApprenticeFeedbackResult
{
    public class WhenGettingApprenticeFeedbackDetailsSummaryAnnual
    {
        [Test, MoqAutoData]
        public async Task And_MediatorCommandIsSuccessful_Then_ReturnOk
            (long ukprn,
            [Frozen] Mock<IMediator> mediator,
            GetApprenticeFeedbackDetailsAnnualResult annualFeedbackResult,
            [Greedy] ApprenticeFeedbackResultController controller)
        {
            mediator.Setup(m => m.Send(It.Is<GetApprenticeFeedbackDetailsAnnualQuery>(t => t.Ukprn == ukprn), It.IsAny<CancellationToken>())).ReturnsAsync(annualFeedbackResult);
            var result = await controller.GetApprenticeFeedbackRatingSummaryAnnual(ukprn);

            result.Should().BeOfType<OkObjectResult>().Which.Value.Should().BeEquivalentTo(annualFeedbackResult);
        }

        [Test, MoqAutoData]
        public async Task And_MediatorCommandIsUnsuccessful_Then_ReturnBadRequest
            (long ukprn,
            [Frozen] Mock<IMediator> mediator,
            [Greedy] ApprenticeFeedbackResultController controller)
        {
            var exception = new Exception("Error");
            mediator.Setup(m => m.Send(It.Is<GetApprenticeFeedbackDetailsAnnualQuery>(t => t.Ukprn == ukprn), It.IsAny<CancellationToken>())).ThrowsAsync(exception);
            var result = await controller.GetApprenticeFeedbackRatingSummaryAnnual(ukprn);

            result.Should().BeOfType<BadRequestResult>();
        }
    }
}
