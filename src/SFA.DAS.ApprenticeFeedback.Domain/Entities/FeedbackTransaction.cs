﻿using System;

namespace SFA.DAS.ApprenticeFeedback.Domain.Entities
{
    public class FeedbackTransaction
    {
        public long Id { get; set; }
        public Guid ApprenticeFeedbackTargetId { get; set; }
        public string EmailAddress { get; set; }
        public string FirstName { get; set; }
        public Guid? TemplateId { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? SendAfter { get; set; }
        public DateTime? SentDate { get; set; }
        public string TemplateName { get; set; }
        public bool IsSuppressed { get; set; }
        public string Variant { get; set; }

        public ApprenticeFeedbackTarget ApprenticeFeedbackTarget { get; set; }
    }
}
