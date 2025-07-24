using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.ApprenticeFeedback.Api.Controllers;
using SFA.DAS.ApprenticeFeedback.Application.Queries.GetApprenticeFeedbackRatingSummary;
using SFA.DAS.ApprenticeFeedback.Domain.Entities;
using SFA.DAS.Testing.AutoFixture;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Api.UnitTests.Controllers.ApprenticeFeedbackResult
{
    public class WhenGettingApprenticeFeedbackRatingSummary
    {
        [Test, MoqAutoData]
        public async Task And_MediatorCommandIsSuccessful_Then_ReturnOk(
            [Frozen] Mock<IMediator> mediator,
            GetApprenticeFeedbackRatingSummaryResult summaryResult,
            [Greedy] ApprenticeFeedbackResultController controller)
        {
            mediator.Setup(m => m.Send(It.IsAny<GetApprenticeFeedbackRatingSummaryQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(summaryResult);
            var result = await controller.GetApprenticeFeedbackRatingSummary("AY2024");

            result.Should().BeOfType<OkObjectResult>().Which.Value.Should().BeEquivalentTo(summaryResult.RatingSummaries);
        }

        [Test, MoqAutoData]
        public async Task And_MediatorCommandIsSuccessful_With_No_Results_Then_ReturnOk(
            [Frozen] Mock<IMediator> mediator,
            GetApprenticeFeedbackRatingSummaryResult summaryResult,
            [Greedy] ApprenticeFeedbackResultController controller)
        {
            mediator.Setup(m => m.Send(It.IsAny<GetApprenticeFeedbackRatingSummaryQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetApprenticeFeedbackRatingSummaryResult(new List<ProviderStarsSummary>()));
            var result = await controller.GetApprenticeFeedbackRatingSummary("AY2024");

            result.Should().BeOfType<OkObjectResult>().Which.Value.Should().BeEquivalentTo(new List<GetApprenticeFeedbackRatingSummaryResult.RatingSummary>());
        }

        [Test, MoqAutoData]
        public async Task And_MediatorCommandIsUnsuccessful_Then_ReturnBadRequest
            ([Frozen] Mock<IMediator> mediator,
            [Greedy] ApprenticeFeedbackResultController controller)
        {
            mediator.Setup(m => m.Send(It.IsAny<GetApprenticeFeedbackRatingSummaryQuery>(), It.IsAny<CancellationToken>())).Throws(new Exception());

            var result = await controller.GetApprenticeFeedbackRatingSummary("AY2024");

            result.Should().BeOfType<BadRequestResult>();
        }

        [TestCase("All")]
        [TestCase("")]
        [TestCase("AY2024")]
        public async Task GetApprenticeFeedbackRatingSummary_WithValidTimePeriod_ReturnsOk(string timePeriod)
        {
            var mediator = new Mock<IMediator>();
            var logger = new Mock<ILogger<ApprenticeFeedbackResultController>>();
            var controller = new ApprenticeFeedbackResultController(mediator.Object, logger.Object);

            mediator.Setup(m => m.Send(It.IsAny<GetApprenticeFeedbackRatingSummaryQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetApprenticeFeedbackRatingSummaryResult(new List<ProviderStarsSummary>()));

            var result = await controller.GetApprenticeFeedbackRatingSummary(timePeriod);

            result.Should().BeOfType<OkObjectResult>();
        }
    }
}
