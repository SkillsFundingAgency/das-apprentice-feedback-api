using MediatR;
using System;

namespace SFA.DAS.ApprenticeFeedback.Application.Queries.GetAllProviders
{
    public class GetAllProvidersForApprenticeQuery : IRequest<GetAllProvidersForApprenticeResult>
    {
        public Guid ApprenticeId { get; set; }
    }
}
