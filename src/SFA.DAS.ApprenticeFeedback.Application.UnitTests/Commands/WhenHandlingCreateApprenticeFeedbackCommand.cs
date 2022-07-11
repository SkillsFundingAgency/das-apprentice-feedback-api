using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.ApprenticeFeedback.Application.Commands.CreateApprenticeFeedback;
using SFA.DAS.ApprenticeFeedback.Data;
using SFA.DAS.ApprenticeFeedback.Domain.Entities;
using SFA.DAS.ApprenticeFeedback.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Application.UnitTests.Commands
{
    public class WhenHandlingCreateApprenticeFeedbackCommand
    {
        [Test, AutoMoqData]
        public async Task And_GetFeedbackTargetIsNull_Then_ThrowException
            (CreateApprenticeFeedbackCommand command,
            [Frozen(Matching.ImplementedInterfaces)] ApprenticeFeedbackDataContext context,
            CreateApprenticeFeedbackHandler handler)
        {
            Func<Task> result = () => handler.Handle(command, CancellationToken.None);

            await result.Should().ThrowAsync<InvalidOperationException>().WithMessage($"Apprentice Feedback Target not found. ApprenticeFeedbackTargetId: {command.ApprenticeFeedbackTargetId}");
        }

        [Test, AutoMoqData]
        public async Task And_GetFeedbackTargetIsValid_And_GetAttributesIsNotValid_Then_ThrowException
            (CreateApprenticeFeedbackCommand command,
            [Frozen(Matching.ImplementedInterfaces)] ApprenticeFeedbackDataContext context,
            CreateApprenticeFeedbackHandler handler,
            Domain.Entities.ApprenticeFeedbackTarget apprenticeFeedbackTarget,
            IEnumerable<FeedbackAttribute> validAttributes)
        {
            context.Attributes.AddRange(validAttributes.Select(s => new Domain.Entities.Attribute { AttributeId = s.Id, AttributeName = s.Name }));
            context.ApprenticeFeedbackTargets.Add(apprenticeFeedbackTarget);
            context.SaveChanges();
            command.ApprenticeFeedbackTargetId = apprenticeFeedbackTarget.Id;

            var testAttribute1 = new FeedbackAttribute { Id = 1, Name = "FeedbackAttribute1", Status = FeedbackAttributeStatus.Agree };
            var testAttribute2 = new FeedbackAttribute { Id = 2, Name = "FeedbackAttribute2", Status = FeedbackAttributeStatus.Disagree };
            command.FeedbackAttributes = new List<FeedbackAttribute> { testAttribute1, testAttribute2 };
            command.FeedbackAttributes.AddRange(validAttributes);

            var mockAttributesNames = validAttributes.Select(attribute => attribute.Name).ToList();
            string mockAttributes = string.Join(", ", mockAttributesNames);
            var errorMessage = $"Some or all of the attributes supplied to create the feedback record do not exist in the database. Attributes provided in the request: FeedbackAttribute1, FeedbackAttribute2, {mockAttributes}, the following attributes are invalid: FeedbackAttribute1, FeedbackAttribute2";
            Func<Task> result = async () => await handler.Handle(command, CancellationToken.None);

            await result.Should().ThrowAsync<InvalidOperationException>().WithMessage(errorMessage);
        }

        [Test, AutoMoqData]
        public async Task And_GetFeedbackTargetIsValid_And_GetAttributesIsValid_Then_CreateNewFeedbackRecord_And_ReturnResponse
            (CreateApprenticeFeedbackCommand command,
            [Frozen(Matching.ImplementedInterfaces)] ApprenticeFeedbackDataContext context,
            Domain.Entities.ApprenticeFeedbackTarget apprenticeFeedbackTarget,
            ApprenticeFeedbackResult apprenticeFeedbackResult,
            CreateApprenticeFeedbackHandler handler,
            IEnumerable<Domain.Entities.Attribute> attributes)
        {
            command.FeedbackAttributes = attributes.Select(a => new FeedbackAttribute { Id = a.AttributeId, Name = a.AttributeName, Status = FeedbackAttributeStatus.Agree }).ToList();
            command.ApprenticeFeedbackTargetId = apprenticeFeedbackTarget.Id;

            context.Attributes.AddRange(attributes);
            context.ApprenticeFeedbackTargets.Add(apprenticeFeedbackTarget);
            context.SaveChanges();
            
            var result = await handler.Handle(command, CancellationToken.None);

            result.Should().BeOfType<CreateApprenticeFeedbackResponse>().Which.ApprenticeFeedbackResultId.Should().NotBeEmpty();

            context.ApprenticeFeedbackResults.FirstOrDefault(afr => afr.ApprenticeFeedbackTargetId == apprenticeFeedbackTarget.Id).Should().NotBeNull();
        }
    }
}
