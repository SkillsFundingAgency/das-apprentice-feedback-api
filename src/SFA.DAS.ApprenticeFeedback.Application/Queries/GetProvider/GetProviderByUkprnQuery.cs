using MediatR;
using System;

namespace SFA.DAS.ApprenticeFeedback.Application.Queries.GetProvider
{
    public class GetProviderByUkprnQuery : IRequest<GetProviderByUkprnResult>
    {
        public Guid ApprenticeId { get; set; }
        public long Ukprn { get; set; }
    }
}
