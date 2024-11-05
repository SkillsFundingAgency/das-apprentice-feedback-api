using SFA.DAS.ApprenticeFeedback.Application.Commands.ProcessEmailTransaction;
using SFA.DAS.ApprenticeFeedback.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Application.Services
{
    public interface IEmailTemplateService
    {
        Task<(Guid? Id, string Name, string Variant, Dictionary<string, string> Tokens)> GetEmailTemplateInfoForTransaction(FeedbackTransaction feedbackTransaction, ProcessEmailTransactionCommand request);
    }
}
