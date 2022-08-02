
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.ApprenticeFeedback.Api.Controllers;
using SFA.DAS.ApprenticeFeedback.Application.Commands.GenerateFeedbackTransactions;
using SFA.DAS.Testing.AutoFixture;
using Microsoft.AspNetCore.Http;

namespace SFA.DAS.ApprenticeFeedback.Api.UnitTests.Controllers.ApprenticeFeedbackTarget
{
    public class WhenGeneratingFeedbackTransactions
    {
        FeedbackTransactionController _sut;
        Mock<IMediator> _mockMediator;

        [SetUp]
        public void Setup()
        {
            _mockMediator = new Mock<IMediator>();
            _sut = new FeedbackTransactionController(_mockMediator.Object, Mock.Of<ILogger<FeedbackTransactionController>>());
        }

        [Test, RecursiveMoqAutoData]
        public async Task And_MediatorCommandSuccessful_Then_ReturnOk(
            GenerateFeedbackTransactionsCommandResponse response)
        {
            _mockMediator.Setup(s => s.Send<GenerateFeedbackTransactionsCommandResponse>(
                It.IsAny<GenerateFeedbackTransactionsCommand>(),
                It.IsAny<CancellationToken>())
            ).ReturnsAsync(response);

            var result = await _sut.FeedbackTransaction();
            result.Should().BeOfType<OkObjectResult>().Which.Value.Should().BeEquivalentTo(response);
        }

        [Test, MoqAutoData]
        public async Task And_MediatorThrowsException_Then_ReturnBadRequest()
        {
            _mockMediator.Setup(s => s.Send<GenerateFeedbackTransactionsCommandResponse>(
                It.IsAny<GenerateFeedbackTransactionsCommand>(),
                It.IsAny<CancellationToken>())
            ).Throws(new Exception());

            var result = await _sut.FeedbackTransaction();
            result.Should().BeOfType<ObjectResult>();
            Assert.AreEqual(StatusCodes.Status500InternalServerError, ((ObjectResult)result).StatusCode);
        }
    }
}
