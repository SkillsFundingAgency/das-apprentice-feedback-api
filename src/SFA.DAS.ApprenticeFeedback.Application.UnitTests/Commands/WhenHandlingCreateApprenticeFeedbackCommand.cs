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
        [Test, MoqAutoData]
        public async Task And_GetFeedbackTargetIsNull_Then_ThrowException
            (CreateApprenticeFeedbackCommand command,
            [Frozen] Mock<IApprenticeFeedbackRepository> mockApprenticeFeedbackRepository,
            CreateApprenticeFeedbackHandler handler)
        {
            mockApprenticeFeedbackRepository.Setup(s => s.GetApprenticeFeedbackTargetById(command.ApprenticeId)).Throws(new Exception());

            Func<Task> result = async () => await handler.Handle(command, CancellationToken.None);

            await result.Should().ThrowAsync<Exception>();
        }

        [Test, MoqAutoData]
        public async Task And_GetFeedbackTargetIsValid_And_GetAttributesIsNotValid_Then_ThrowException
            (CreateApprenticeFeedbackCommand command,
            [Frozen] Mock<IApprenticeFeedbackRepository> mockApprenticeFeedbackRepository,
            CreateApprenticeFeedbackHandler handler,
            Domain.Entities.ApprenticeFeedbackTarget apprenticeFeedbackTarget)
        {
            mockApprenticeFeedbackRepository.Setup(s => s.GetApprenticeFeedbackTargetById(command.ApprenticeId)).ReturnsAsync(apprenticeFeedbackTarget);
            mockApprenticeFeedbackRepository.Setup(s => s.GetAttributes());

            Func<Task> result = async () => await handler.Handle(command, CancellationToken.None);

            await result.Should().ThrowAsync<Exception>();
            
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
            command.FeedbackAttributes = attributes.Select(a => new Domain.Models.FeedbackAttribute { Id = a.AttributeId, Name = a.AttributeName, Status = FeedbackAttributeStatus.Agree }).ToList();
            apprenticeFeedbackRepository.Setup(s => s.GetApprenticeFeedbackTargetById(command.ApprenticeId)).ReturnsAsync(apprenticeFeedbackTarget);
            apprenticeFeedbackRepository.Setup(s => s.GetAttributes()).ReturnsAsync(attributes);
            apprenticeFeedbackRepository.Setup(s => s.CreateApprenticeFeedbackResult(apprenticeFeedbackResult));

            var result = await handler.Handle(command, CancellationToken.None);


            result.Should().BeOfType<CreateApprenticeFeedbackResponse>();

        }
    }
}
