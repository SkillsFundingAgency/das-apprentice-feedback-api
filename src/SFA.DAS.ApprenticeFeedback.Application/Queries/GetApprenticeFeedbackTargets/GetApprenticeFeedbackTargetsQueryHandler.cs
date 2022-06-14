﻿using MediatR;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Application.Queries.GetApprenticeFeedbackTargets
{
    public class GetApprenticeFeedbackTargetsQueryHandler : IRequestHandler<GetApprenticeFeedbackTargetsQuery, GetApprenticeFeedbackTargetsResult>
    {
        private readonly IApprenticeFeedbackTargetDataContext _apprenticeFeedbackTargetDataContext;

        public GetApprenticeFeedbackTargetsQueryHandler(IApprenticeFeedbackTargetDataContext apprenticeFeedbackTargetDataContext)
        {
            _apprenticeFeedbackTargetDataContext = apprenticeFeedbackTargetDataContext;
        }

        public async Task<GetApprenticeFeedbackTargetsResult> Handle(GetApprenticeFeedbackTargetsQuery request, CancellationToken cancellationToken)
        {
            var apprenticeFeedbackTargets = await _apprenticeFeedbackTargetDataContext.GetApprenticeFeedbackTargetsAsync(request.ApprenticeId);

            return new GetApprenticeFeedbackTargetsResult
            {
                ApprenticeFeedbackTargets = apprenticeFeedbackTargets.Select(s => (Domain.Models.ApprenticeFeedbackTarget)s).ToList()
            };
        }
    }
}
