using MediatR;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Application.Queries.GetApprenticeFeedbackTargets
{
    public class GetApprenticeFeedbackTargetsQueryHandler : IRequestHandler<GetApprenticeFeedbackTargetsQuery, GetApprenticeFeedbackTargetsResult>
    {
        private readonly IApprenticeFeedbackDataContext _dbContext;

        public GetApprenticeFeedbackTargetsQueryHandler(IApprenticeFeedbackDataContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<GetApprenticeFeedbackTargetsResult> Handle(GetApprenticeFeedbackTargetsQuery request, CancellationToken cancellationToken)
        {
            var apprenticeFeedbackTargets = await _dbContext.ApprenticeFeedbackTargets
                                                    .Include(s => s.ApprenticeFeedbackResults)
                                                    .Where(aft => aft.ApprenticeId == request.ApprenticeId).ToListAsync();

            return new GetApprenticeFeedbackTargetsResult
            {
                ApprenticeFeedbackTargets = apprenticeFeedbackTargets.Select(s => (Domain.Models.ApprenticeFeedbackTarget)s).ToList()
            };
        }
    }
}
