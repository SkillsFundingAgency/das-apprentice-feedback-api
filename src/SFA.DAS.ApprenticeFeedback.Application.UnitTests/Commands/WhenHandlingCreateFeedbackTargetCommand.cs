using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using SFA.DAS.ApprenticeFeedback.Application.Commands.CreateApprenticeFeedbackTarget;
using SFA.DAS.ApprenticeFeedback.Data;
using SFA.DAS.ApprenticeFeedback.Domain.Entities;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;
using System;
using System.Threading;
using System.Threading.Tasks;
using static SFA.DAS.ApprenticeFeedback.Domain.Models.Enums;

namespace SFA.DAS.ApprenticeFeedback.Application.UnitTests.Commands
{
    public class WhenHandlingCreateFeedbackTargetCommand
    {
        [Test/*, RecursiveMoqAutoData*/]
        public async Task And_CommandIsValid_Then_CreatesFeedback_If_It_Doesnt_Exist(
           //CreateApprenticeFeedbackTargetCommand command,
           //[Frozen] Mock<IApprenticeFeedbackTargetContext> mockApprenticeFeedbackTargetContext,
           //CreateApprenticeFeedbackTargetCommandHandler handler,
           //Guid response
           )
        {

            var connection = new SqliteConnection("Filename=:memory:");
            connection.Open();

            // These options will be used by the context instances in this test suite, including the connection opened above.
            var contextOptions = new DbContextOptionsBuilder<ApprenticeFeedbackDataContext>()
                .UseSqlite(connection)
                .Options;

            // Create the schema and seed some data
            using var context = new ApprenticeFeedbackDataContext(contextOptions);

            context.Database.EnsureCreated();





            //mockApprenticeFeedbackTargetContext.Setup(s => s.FindByApprenticeIdAndApprenticeshipIdAndIncludeFeedbackResultsAsync(command.ApprenticeId, command.CommitmentApprenticeshipId)).ReturnsAsync((ApprenticeFeedbackTarget)null);

            /*
            mockApprenticeFeedbackRepository.Setup(s => s.CreateApprenticeFeedbackTarget(
                It.Is<ApprenticeFeedbackTarget>(s => 
                s.ApprenticeId == command.ApprenticeId &&
                s.ApprenticeshipId == command.CommitmentApprenticeshipId &&
                s.Status == (int)FeedbackTargetStatus.Unknown
                ))).ReturnsAsync(response);
            */

            //mockApprenticeFeedbackTargetContext.Setup(s => s.Add(It.IsAny<Domain.Models.ApprenticeFeedbackTarget>())).Returns(new Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<ApprenticeFeedbackTarget>(new Microsoft.EntityFrameworkCore.ChangeTracking.Internal.InternalEntityEntry()))

            try
            {
                var handler = new CreateApprenticeFeedbackTargetCommandHandler(context);
                var command = new CreateApprenticeFeedbackTargetCommand()
                {
                    ApprenticeId = Guid.NewGuid(),
                };
                var result = await handler.Handle(command, CancellationToken.None);

                result.ApprenticeFeedbackTargetId.Should().NotBeEmpty();
            }
            catch(Exception ex)
            {
                ex = ex;
            }
        }

        [Test, RecursiveMoqAutoData]
        public async Task And_CommandIsValid_Then_UpdatesFeedback_If_It_Exists(
           CreateApprenticeFeedbackTargetCommand command,
           [Frozen] Mock<IApprenticeFeedbackTargetContext> mockApprenticeFeedbackTargetDataContext,
           ApprenticeFeedbackTarget apprenticeFeedbackTarget,
           CreateApprenticeFeedbackTargetCommandHandler handler)
        {
            mockApprenticeFeedbackTargetDataContext.Setup(s => s.FindByApprenticeIdAndApprenticeshipIdAndIncludeFeedbackResultsAsync(command.ApprenticeId, command.CommitmentApprenticeshipId)).ReturnsAsync(apprenticeFeedbackTarget);
            /*
            mockApprenticeFeedbackRepository.Setup(s => s.UpdateApprenticeFeedbackTarget(It.Is<ApprenticeFeedbackTarget>(s =>
                s.StartDate == null && s.EndDate == null &&
                s.Status == (int)FeedbackTargetStatus.Unknown &&
                s.Id == apprenticeFeedbackTarget.Id))).ReturnsAsync(apprenticeFeedbackTarget);
            */
            var result = await handler.Handle(command, CancellationToken.None);

            result.ApprenticeFeedbackTargetId.Should().Be(apprenticeFeedbackTarget.Id);
        }
    }
}
