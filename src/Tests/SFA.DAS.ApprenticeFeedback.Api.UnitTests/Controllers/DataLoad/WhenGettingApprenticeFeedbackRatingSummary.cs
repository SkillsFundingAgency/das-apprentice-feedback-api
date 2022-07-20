using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.ApprenticeFeedback.Api.Controllers;
using SFA.DAS.ApprenticeFeedback.Application.Commands.GenerateFeedbackSummaries;
using SFA.DAS.Testing.AutoFixture;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Api.UnitTests.Controllers.DataLoad
{
    public class WhenGettingApprenticeFeedbackRatingSummary
    {
        [Test, MoqAutoData]
        public async Task And_MediatorCommandIsSuccessful_Then_ReturnOk([Greedy] DataLoadController controller)
        {
            var result = await controller.GenerateFeedbackSummaries();

            result.Should().BeOfType<OkResult>();
        }


        [Test, MoqAutoData]
        public async Task And_MediatorCommandIsUnsuccessful_Then_ReturnBadRequest
            ([Frozen] Mock<IMediator> mediator,
            [Greedy] DataLoadController controller)
        {
            mediator.Setup(m => m.Send(It.IsAny<GenerateFeedbackSummariesCommand>(), It.IsAny<CancellationToken>())).Throws(new Exception());

            var result = await controller.GenerateFeedbackSummaries();

            result.Should().BeOfType<BadRequestResult>();
        }
    }
}
