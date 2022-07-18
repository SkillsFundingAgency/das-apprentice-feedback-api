using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Domain.Interfaces
{
    public interface IProviderRatingSummaryContext : IEntityContext<Domain.Entities.ProviderRatingSummary>
    {
        Task GenerateFeedbackSummaries(int minimumNumberOfResponses, int reportingFeedbackCutoffMonths);
    }
}