using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.ApprenticeFeedback.Api.Controllers;
using SFA.DAS.ApprenticeFeedback.Application.Queries.GetApprenticeFeedbackDetails;
using SFA.DAS.ApprenticeFeedback.Application.Queries.GetApprenticeFeedbackRatingSummary;
using SFA.DAS.Testing.AutoFixture;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.ApprenticeFeedback.Domain.Entities;

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
            var result = await controller.GetApprenticeFeedbackRatingSummary();

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
            var result = await controller.GetApprenticeFeedbackRatingSummary();

            result.Should().BeOfType<OkObjectResult>().Which.Value.Should().BeEquivalentTo(new List<GetApprenticeFeedbackRatingSummaryResult.RatingSummary>());
        }
        
        [Test, MoqAutoData]
        public async Task And_MediatorCommandIsUnsuccessful_Then_ReturnBadRequest
            ([Frozen] Mock<IMediator> mediator,
            [Greedy] ApprenticeFeedbackResultController controller)
        {
            mediator.Setup(m => m.Send(It.IsAny<GetApprenticeFeedbackRatingSummaryQuery>(), It.IsAny<CancellationToken>())).Throws(new Exception());

            var result = await controller.GetApprenticeFeedbackRatingSummary();

            result.Should().BeOfType<BadRequestResult>();
        }
    }
}
