namespace SFA.DAS.ApprenticeFeedback.Api.Configuration
{
    public class ApplicationSettings
    {
        public string DbConnectionString { get; set; }
        
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
    }
}
