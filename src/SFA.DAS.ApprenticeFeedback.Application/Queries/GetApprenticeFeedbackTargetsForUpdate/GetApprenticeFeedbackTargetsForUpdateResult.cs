﻿using SFA.DAS.ApprenticeFeedback.Domain.Models;
using System.Collections.Generic;

namespace SFA.DAS.ApprenticeFeedback.Application.Queries.GetApprenticeFeedbackTargetsForUpdate
{
    public class GetApprenticeFeedbackTargetsForUpdateResult
    {
        public IEnumerable<ApprenticeFeedbackTarget> ApprenticeFeedbackTargets { get; set; }
    }
}
