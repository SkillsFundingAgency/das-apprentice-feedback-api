using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.ApprenticeFeedback.Domain.Entities;
using System;
using Microsoft.Extensions.Options;
using SFA.DAS.ApprenticeFeedback.Domain.Configuration;

namespace SFA.DAS.ApprenticeFeedback.Application.Commands.ProcessFeedbackTargetVariants
{
    public class ProcessFeedbackTargetVariantsCommandHandler : IRequestHandler<ProcessFeedbackTargetVariantsCommand>
    {
        private readonly IFeedbackTargetVariant_StagingContext _stagingContext;
        private readonly IFeedbackTargetVariantContext _context;
        private readonly ILogger<ProcessFeedbackTargetVariantsCommandHandler> _logger;

        public ProcessFeedbackTargetVariantsCommandHandler(
            IFeedbackTargetVariant_StagingContext stagingContext,
            IFeedbackTargetVariantContext context,
            ILogger<ProcessFeedbackTargetVariantsCommandHandler> logger)
        {
            _stagingContext = stagingContext;
            _context = context;
            _logger = logger;
        }

        public async Task<Unit> Handle(ProcessFeedbackTargetVariantsCommand request, CancellationToken cancellationToken)
        {
            if (request.ClearStaging)
            {
                await _stagingContext.ClearFeedbackTargetVariant_Staging();
            }
            
            await ImportStagingData(request);

            if(request.MergeStaging) 
            {
                await _context.ImportIntoFeedbackTargetVariantFromStaging();
            }

            return Unit.Value;

        }

        private async Task ImportStagingData(ProcessFeedbackTargetVariantsCommand request)
        {
            await _stagingContext.AddRange(
                request.FeedbackTargetVariants.Select(v => (Domain.Entities.FeedbackTargetVariant_Staging)v).ToList());

            await _stagingContext.SaveChangesAsync();
        }
    }
}
