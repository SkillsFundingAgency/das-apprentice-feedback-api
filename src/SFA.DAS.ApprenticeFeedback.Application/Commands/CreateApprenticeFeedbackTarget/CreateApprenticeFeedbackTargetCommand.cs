using MediatR;
using System;

namespace SFA.DAS.ApprenticeFeedback.Application.Commands.CreateApprenticeFeedbackTarget
{
    public class CreateApprenticeFeedbackTargetCommand : IRequest<CreateApprenticeFeedbackTargetCommandResponse>
    {
        /// <summary>
        /// Apprentice Accounts Id, The guid representing the apprentice ID as stored in the Apprentice Login database
        /// </summary>
        public Guid ApprenticeId { get; set; }
        /// <summary>
        /// Apprentice Commitments Apprentice Id, the integer id assigned as part of apprentice commitments in CMAD database
        /// </summary>
        public long ApprenticeshipId { get; set; }
        /// <summary>
        /// Apprenticeship Id in Commitments database / apprenticeship table
        /// </summary>
        public long CommitmentApprenticeshipId { get; set; }
    }
}
