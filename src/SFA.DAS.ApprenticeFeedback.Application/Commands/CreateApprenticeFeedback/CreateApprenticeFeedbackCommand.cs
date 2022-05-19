using MediatR;
using SFA.DAS.ApprenticeFeedback.Domain.Models;
using System;
using System.Collections.Generic;

namespace SFA.DAS.ApprenticeFeedback.Application.Commands.CreateApprenticeFeedback
{
    public class CreateApprenticeFeedbackCommand : IRequest<CreateApprenticeFeedbackResponse>
    {
        public Guid ApprenticeFeedbackTargetId { get; set; }
        public OverallRating OverallRating { get; set; }
        public List<FeedbackAttribute> FeedbackAttributes { get; set; }
        public bool AllowContact { get; set; }
    }

    public enum OverallRating
    {
        VeryPoor = 1,
        Poor = 2,
        Good = 3,
        Excellent = 4
    }
}