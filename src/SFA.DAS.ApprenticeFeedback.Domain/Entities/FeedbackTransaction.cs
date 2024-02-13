using System;
using System.Collections.Generic;

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
        public ApprenticeFeedbackTarget ApprenticeFeedbackTarget { get; set; }
        
        public static implicit operator FeedbackTransaction(Models.FeedbackTransaction source)
        {
            return new FeedbackTransaction
            {
                Id = source.Id,
                ApprenticeFeedbackTargetId = source.ApprenticeFeedbackTargetId,
                EmailAddress = source.EmailAddress,
                FirstName = source.FirstName,
                TemplateId = source.TemplateId,
                CreatedOn = source.CreatedOn,
                SendAfter = source.SendAfter,
                SentDate = source.SentDate,
                TemplateName = source.TemplateName,
                IsSuppressed = source.IsSuppressed
            };
        }
    }
}
