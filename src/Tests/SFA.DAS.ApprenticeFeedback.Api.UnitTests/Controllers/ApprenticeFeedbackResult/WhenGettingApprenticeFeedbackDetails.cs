using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.ApprenticeFeedback.Api.Controllers;
using SFA.DAS.ApprenticeFeedback.Application.Queries.GetApprenticeFeedbackDetails;
using SFA.DAS.Testing.AutoFixture;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Api.UnitTests.Controllers.ApprenticeFeedbackResult
{
    public class WhenGettingApprenticeFeedbackDetails
    {
        [Test, MoqAutoData]
        public async Task And_MediatorCommandIsSuccessful_Then_ReturnOk
            (long ukprn,
            [Frozen] Mock<IMediator> mediator,
            GetApprenticeFeedbackDetailsResult detailResult,
            [Greedy] ApprenticeFeedbackResultController controller)
        {
            mediator.Setup(m => m.Send(It.Is<GetApprenticeFeedbackDetailsQuery>(t => t.Ukprn == ukprn), It.IsAny<CancellationToken>())).ReturnsAsync(detailResult);
            var result = await controller.GetApprenticeFeedbackDetails(ukprn);

            result.Should().BeOfType<OkObjectResult>().Which.Value.Should().BeEquivalentTo(detailResult);
        }

        [Test, MoqAutoData]
        public async Task And_MediatorCommandIsUnsuccessful_Then_ReturnBadRequest
            (long ukprn,
            [Frozen] Mock<IMediator> mediator,
            [Greedy] ApprenticeFeedbackResultController controller)
        {
            mediator.Setup(m => m.Send(It.IsAny<GetApprenticeFeedbackDetailsQuery>(), It.IsAny<CancellationToken>())).Throws(new Exception());

            var result = await controller.GetApprenticeFeedbackDetails(ukprn);

            result.Should().BeOfType<BadRequestResult>();
        }
    }
}
