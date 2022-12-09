using System;

namespace SFA.DAS.ApprenticeFeedback.Domain.Entities
{
    public class ExitSurveyAttribute
    {
        public Guid ApprenticeExitSurveyId { get; set; }
        public int AttributeId { get; set; }
        public int AttributeValue { get; set; }
    }
}
