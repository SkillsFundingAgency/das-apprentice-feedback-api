using SFA.DAS.ApprenticeFeedback.Domain.Configuration;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;
using static SFA.DAS.ApprenticeFeedback.Domain.Models.Enums;

namespace SFA.DAS.ApprenticeFeedback.Domain.Models
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


        public static implicit operator FeedbackTransaction(Entities.FeedbackTransaction source)
        {
            if (source == null)
                return null;

            return new FeedbackTransaction
            {
                Id = source.Id,
                ApprenticeFeedbackTargetId = source.ApprenticeFeedbackTargetId,
                EmailAddress = source.EmailAddress,
                FirstName = source.FirstName,
                TemplateId = source.TemplateId,
                CreatedOn = source.CreatedOn,
                SendAfter = source.SendAfter,
                SentDate = source.SentDate
            };
        }
    }
}
