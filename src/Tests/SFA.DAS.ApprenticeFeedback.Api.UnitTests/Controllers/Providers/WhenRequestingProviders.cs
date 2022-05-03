using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.ApprenticeFeedback.Api.Controllers;
using SFA.DAS.ApprenticeFeedback.Application.Queries.GetAllProviders;
using SFA.DAS.Testing.AutoFixture;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Api.UnitTests.Controllers.WhenRequestingProviders
{
    public class WhenRequestingProviders
    {
        [Test, MoqAutoData]
        public async Task And_MediatorCommandReturnsNull_Then_ReturnNotFoundResult(
            [Frozen] Mock<IMediator> mediator,
            [Greedy] ProvidersController controller,
            Guid apprenticeId)
        {
            mediator.Setup(m => m.Send(It.Is<GetAllProvidersForApprenticeQuery>(s =>
                s.ApprenticeId == apprenticeId), It.IsAny<CancellationToken>())).ReturnsAsync((GetAllProvidersForApprenticeResult)null);

            var result = await controller.GetAllProvidersForApprentice(apprenticeId) as NotFoundResult;

            result.Should().NotBeNull();
        }

        [Test, MoqAutoData]
        public async Task And_MediatorCommandSuccessful_Then_ReturnOk(
            [Frozen] Mock<IMediator> mediator,
            [Greedy] ProvidersController controller,
            GetAllProvidersForApprenticeResult response,
            Guid apprenticeId)
        {
            mediator.Setup(m => m.Send(It.Is<GetAllProvidersForApprenticeQuery>(s =>
                s.ApprenticeId == apprenticeId), It.IsAny<CancellationToken>())).ReturnsAsync(response);

            var result = await controller.GetAllProvidersForApprentice(apprenticeId) as OkObjectResult;

            result.Should().NotBeNull();
            var model = result.Value;
            model.Should().BeEquivalentTo(new
            {
                response.TrainingProviders,
                response.FinalAllowedPeriodDays,
                response.RecentDenyPeriodDays,
                response.InitialDenyPeriodDays,
                response.MinimumActiveApprenticeshipCount
            });
        }
    }
}
