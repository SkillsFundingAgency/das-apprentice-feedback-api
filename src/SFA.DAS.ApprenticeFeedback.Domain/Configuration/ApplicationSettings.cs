using System;
using System.Collections.Generic;

namespace SFA.DAS.ApprenticeFeedback.Domain.Configuration
{
    public class ApplicationSettings
    {
        public string DbConnectionString { get; set; }
        public string NServiceBusConnectionString { get; set; }
        public string NServiceBusLicense { get; set; }

        /// <summary>
        /// How soon since the last feedback given by an apprentice they are able to give feedback again
        /// </summary>
        public int RecentDenyPeriodDays { get; set; }
        /// <summary>
        /// How soon after starting an apprenticeship is an apprentice able to give feedback
        /// </summary>
        public int InitialDenyPeriodDays { get; set; }
        /// <summary>
        /// How long after finishing an apprenticeship is an apprentice able to give feedback
        /// </summary>
        public int FinalAllowedPeriodDays { get; set; }
        /// <summary>
        /// The minimum number of apprentices active for a given provider such that feedback is allowed
        /// </summary>
        public int MinimumActiveApprenticeshipCount { get; set; }
        /// <summary>
        /// FAT reporting endpoint returns feedback results where there has been 
        /// at least a certain number of responses within this number of months 
        /// </summary>
        public int ReportingFeedbackCutoffMonths { get; set; }
        /// <summary>
        /// FAT reporting endpoint returns feedback results where there has been 
        /// at least this number of responses within a certain number of months 
        /// </summary>
        public int ReportingMinNumberOfResponses { get; set; }
        /// <summary>
        /// How long after Sent Date has passed feedback transactions should be generated for
        /// </summary>
        public int FeedbackTransactionSentDateAgeDays { get; set; }
        /// Number of days since last calculating feedback eligibility before we
        /// allow the calculation to happen again
        /// </summary>
        public int EligibilityCalculationThrottleDays { get; set; }        
        /// <summary>
        /// Number of days to wait before reprocessing a feedback transaction email (eg. 90)
        /// </summary>
        public int FeedbackEmailProcessingRetryWaitDays { get; set; }
        /// <summary>
        /// Number of database changes to treat as a single batch when importing FeedbackTargetVariantion 
        /// </summary>
        public int FeedbackTargetVariantBatchSize { get; set; }

        public List<NotificationTemplate> NotificationTemplates { get; set; }
    }
}
