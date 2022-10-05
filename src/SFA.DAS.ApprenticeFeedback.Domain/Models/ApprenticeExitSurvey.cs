using System;

namespace SFA.DAS.ApprenticeFeedback.Domain.Models
{
    public class ApprenticeExitSurvey
    {
        public Guid Id { get; set; }
        public Guid ApprenticeFeedbackTargetId { get; set; }
        public DateTime DateTimeCompleted { get; set; }

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
                DateTimeCompleted = source.DateTimeCompleted
            };
        }
    }
}
