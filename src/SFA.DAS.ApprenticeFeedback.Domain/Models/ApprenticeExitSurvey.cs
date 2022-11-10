using SFA.DAS.ApprenticeFeedback.Domain.Entities;
using System;
using System.Collections.Generic;

namespace SFA.DAS.ApprenticeFeedback.Domain.Models
{
    public class ApprenticeExitSurvey
    {
        public Guid Id { get; set; }
        public Guid ApprenticeFeedbackTargetId { get; set; }
        public string StandardUId { get; set; }
        public bool AllowContact { get; set; }
        public bool DidNotCompleteApprenticeship { get; set; }
        public DateTime DateTimeCompleted { get; set; }

        public ICollection<ExitSurveyAttribute> ExitSurveyAttributes { get; set; }
        public int PrimaryReason { get; set; }

        public static implicit operator ApprenticeExitSurvey(Entities.ApprenticeExitSurvey source)
        {
            if (source == null)
            {
                return null;
            }

            return new ApprenticeExitSurvey
            {
                Id = source.Id,
                ApprenticeFeedbackTargetId = source.ApprenticeFeedbackTargetId,
                StandardUId = source.StandardUId,
                AllowContact = source.AllowContact,
                DidNotCompleteApprenticeship = source.DidNotCompleteApprenticeship,
                DateTimeCompleted = source.DateTimeCompleted,
                ExitSurveyAttributes = source.ExitSurveyAttributes,
                PrimaryReason = source.PrimaryReason,
            };
        }
    }
}
