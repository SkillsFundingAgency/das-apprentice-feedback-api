using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.ApprenticeFeedback.Api.Controllers;
using SFA.DAS.ApprenticeFeedback.Application.Queries.GetProvider;
using SFA.DAS.Testing.AutoFixture;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Api.UnitTests.Controllers.WhenRequestingProviders
{
    public class WhenRequestingProvider
    {
        [Test, MoqAutoData]
        public async Task And_MediatorCommandReturnsNull_Then_ReturnNotFoundResult(
            [Frozen] Mock<IMediator> mediator,
            [Greedy] ProvidersController controller,
            Guid apprenticeId, long ukprn)
        {
            mediator.Setup(m => m.Send(It.Is<GetProviderByUkprnQuery>(s =>
                s.ApprenticeId == apprenticeId && s.Ukprn == ukprn), It.IsAny<CancellationToken>())).ReturnsAsync((GetProviderByUkprnResult)null);

            var result = await controller.GetProviderForApprenticeAndUkprn(apprenticeId, ukprn) as NotFoundResult;

            result.Should().NotBeNull();
        }

        [Test, MoqAutoData]
        public async Task And_MediatorCommandSuccessful_Then_ReturnOk(
            [Frozen] Mock<IMediator> mediator,
            [Greedy] ProvidersController controller,
            GetProviderByUkprnResult response,
            Guid apprenticeId,
            long ukprn)
        {
            mediator.Setup(m => m.Send(It.Is<GetProviderByUkprnQuery>(s =>
                s.ApprenticeId == apprenticeId && s.Ukprn == ukprn), It.IsAny<CancellationToken>())).ReturnsAsync(response);

            var result = await controller.GetProviderForApprenticeAndUkprn(apprenticeId, ukprn) as OkObjectResult;

            result.Should().NotBeNull();
            var model = result.Value;
            model.Should().BeEquivalentTo(response);
        }
    }
}
