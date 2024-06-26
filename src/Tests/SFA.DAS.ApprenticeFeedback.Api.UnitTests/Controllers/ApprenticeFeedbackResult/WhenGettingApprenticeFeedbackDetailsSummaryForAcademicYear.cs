using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.ApprenticeFeedback.Api.Controllers;
using SFA.DAS.ApprenticeFeedback.Application.Queries.GetApprenticeFeedbackDetailsForAcademicYear;
using SFA.DAS.Testing.AutoFixture;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Api.UnitTests.Controllers.ApprenticeFeedbackResult
{
    public class WhenGettingApprenticeFeedbackDetailsSummaryForAcademicYear
    {
        [Test, MoqAutoData]
        public async Task And_MediatorCommandIsSuccessful_Then_ReturnOk
            (long ukprn,
            string academicYear,
            [Frozen] Mock<IMediator> mediator,
            GetApprenticeFeedbackDetailsForAcademicYearResult academicFeedbackResult,
            [Greedy] ApprenticeFeedbackResultController controller)
        {
            // Arrange
            mediator.Setup(m => m.Send(It.Is<GetApprenticeFeedbackDetailsForAcademicYearQuery>(t => t.Ukprn == ukprn && t.AcademicYear == academicYear), It.IsAny<CancellationToken>()))
                .ReturnsAsync(academicFeedbackResult);

            // Act
            var result = await controller.GetApprenticeFeedbackSummaryAcademicYear(ukprn, academicYear);

            // Assert
            result.Should().BeOfType<OkObjectResult>().Which.Value.Should().BeEquivalentTo(academicFeedbackResult);
        }

        [Test, MoqAutoData]
        public async Task And_MediatorCommandIsUnsuccessful_Then_ReturnBadRequest
            (long ukprn,
            string academicYear,
            [Frozen] Mock<IMediator> mediator,
            [Greedy] ApprenticeFeedbackResultController controller)
        {
            // Arrange
            var errorMessage = $"Error attempting to retrieve annual apprentice feedback results for Ukprn: {ukprn} for year {academicYear}";
            mediator.Setup(m => m.Send(It.Is<GetApprenticeFeedbackDetailsForAcademicYearQuery>(t => t.Ukprn == ukprn && t.AcademicYear == academicYear), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception(errorMessage));

            // Act
            var result = await controller.GetApprenticeFeedbackSummaryAcademicYear(ukprn, academicYear);

            // Assert
            result.Should().BeOfType<BadRequestResult>();
        }

    }
}
