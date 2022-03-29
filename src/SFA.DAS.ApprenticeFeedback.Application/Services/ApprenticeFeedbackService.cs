using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
using SFA.DAS.ApprenticeFeedback.Domain.Models;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Application.Services
{
    public class ApprenticeFeedbackService : IApprenticeFeedbackService
    {
        private readonly IApprenticeFeedbackRepository _repository;

        public ApprenticeFeedbackService(IApprenticeFeedbackRepository repository)
        {
            _repository = repository;
        }

        public async Task<Guid?> CreateApprenticeFeedbackTarget(ApprenticeFeedbackTarget feedbackTarget)
        {
            return await _repository.CreateApprenticeFeedbackTarget(feedbackTarget);
        }
    }
}
