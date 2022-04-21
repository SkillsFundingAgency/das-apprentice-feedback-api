using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.ApprenticeFeedback.Application.Commands.CreateApprenticeFeedback;
using SFA.DAS.ApprenticeFeedback.Domain.Entities;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
using SFA.DAS.ApprenticeFeedback.Domain.Models;
using SFA.DAS.Testing.AutoFixture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Application.UnitTests.Commands
{
    public class WhenHandlingCreateApprenticeFeedbackCommand
    {
        [Test, RecursiveMoqAutoData]
        public async Task And_GetFeedbackTargetIsNull_Then_ThrowException
            (CreateApprenticeFeedbackCommand command,
            [Frozen] Mock<IApprenticeFeedbackRepository> mockApprenticeFeedbackRepository,
            CreateApprenticeFeedbackHandler handler)
        {
            mockApprenticeFeedbackRepository.Setup(s => s.GetApprenticeFeedbackTargetById(command.ApprenticeFeedbackTargetId)).ReturnsAsync((Domain.Entities.ApprenticeFeedbackTarget)null);

            Func<Task> result = async () => await handler.Handle(command, CancellationToken.None);

            await result.Should().ThrowAsync<InvalidOperationException>().WithMessage($"Apprentice Feedback Target not found. ApprenticeFeedbackTargetId: {command.ApprenticeFeedbackTargetId}");
        }

        [Test, RecursiveMoqAutoData]
        public async Task And_GetFeedbackTargetIsValid_And_GetAttributesIsNotValid_Then_ThrowException
            (CreateApprenticeFeedbackCommand command,
            [Frozen] Mock<IApprenticeFeedbackRepository> mockApprenticeFeedbackRepository,
            CreateApprenticeFeedbackHandler handler,
            Domain.Entities.ApprenticeFeedbackTarget apprenticeFeedbackTarget)
        {
            mockApprenticeFeedbackRepository.Setup(s => s.GetApprenticeFeedbackTargetById(command.ApprenticeId)).ReturnsAsync(apprenticeFeedbackTarget);

            Func<Task> result = async () => await handler.Handle(command, CancellationToken.None);

            string attributesProvided = string.Empty;

            foreach (var attribute in command.FeedbackAttributes)
            {
                if (string.IsNullOrEmpty(attributesProvided))
                {
                    attributesProvided = attribute.Name;
                }
                else
                {
                    attributesProvided = $"{attributesProvided}, {attribute.Name}";
                }
            }

            await result.Should().ThrowAsync<InvalidOperationException>().WithMessage($"Some or all of the attributes supplied to create the feedback record do not exist in the database. Attributes provided in the request: {attributesProvided}");
            
        }

        [Test, RecursiveMoqAutoData]
        public async Task And_GetFeedbackTargetIsValid_And_GetAttributesIsValid_Then_CreateNewFeedbackRecord_And_ReturnResponse
            (CreateApprenticeFeedbackCommand command,
            [Frozen] Mock<IApprenticeFeedbackRepository> apprenticeFeedbackRepository,
            Domain.Entities.ApprenticeFeedbackTarget apprenticeFeedbackTarget,
            ApprenticeFeedbackResult apprenticeFeedbackResult,
            CreateApprenticeFeedbackHandler handler,
            IEnumerable<Domain.Entities.Attribute> attributes)
        {
            command.FeedbackAttributes = attributes.Select(a => new FeedbackAttribute { Id = a.AttributeId, Name = a.AttributeName, Status = FeedbackAttributeStatus.Agree }).ToList();
            apprenticeFeedbackRepository.Setup(s => s.GetApprenticeFeedbackTargetById(command.ApprenticeId)).ReturnsAsync(apprenticeFeedbackTarget);
            apprenticeFeedbackRepository.Setup(s => s.GetAttributes()).ReturnsAsync(attributes);
            apprenticeFeedbackRepository.Setup(s => s.CreateApprenticeFeedbackResult(It.Is<ApprenticeFeedbackResult>(c => c.StandardUId == command.StandardUId))).ReturnsAsync(apprenticeFeedbackResult);

            var result = await handler.Handle(command, CancellationToken.None);


            result.Should().BeOfType<CreateApprenticeFeedbackResponse>().Which.ApprenticeFeedbackResultId.Should().Be(apprenticeFeedbackResult.Id);
            apprenticeFeedbackRepository.Verify(s => s.CreateApprenticeFeedbackResult(It.IsAny<ApprenticeFeedbackResult>()), Times.Once());

        }
    }
}
