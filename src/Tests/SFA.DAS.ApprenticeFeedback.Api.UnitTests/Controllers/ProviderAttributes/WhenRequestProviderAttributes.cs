using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.ApprenticeFeedback.Api.Controllers;
using SFA.DAS.ApprenticeFeedback.Application.Queries.GetAttributes;
using SFA.DAS.Testing.AutoFixture;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Api.UnitTests.Controllers.ProviderAttributes
{
    public class WhenRequestProviderAttributes
    {
        [Test, MoqAutoData]
        public async Task And_MediatorCommandSuccessful_Then_ReturnOk(
            [Frozen] Mock<IMediator> mediator,
            [Greedy] ProviderAttributesController controller,
            GetAttributesResponse response)
        {
            mediator.Setup(m => m.Send(It.IsAny<GetAttributesQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(response);

            var result = await controller.GetProviderAttributes() as OkObjectResult;

            result.Should().NotBeNull();

            var model = result.Value as List<Domain.Models.Attribute>;

            model.Should().BeEquivalentTo(response.Attributes);
        }
    }
}
