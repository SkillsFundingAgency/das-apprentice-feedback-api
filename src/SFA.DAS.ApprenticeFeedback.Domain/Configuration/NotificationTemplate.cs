using System;

namespace SFA.DAS.ApprenticeFeedback.Domain.Configuration
{
    public class NotificationTemplate
    {
        /// <summary>
        /// The template name which is contained in the database
        /// </summary>
        public string TemplateName {get; set;}
        
        /// <summary>
        /// The template Id for the configurated GOV.UK Notify email template
        /// </summary>
        public Guid TemplateId { get; set; }
    }
}
