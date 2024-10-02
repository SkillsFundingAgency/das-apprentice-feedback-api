using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.ApprenticeFeedback.Domain.Entities;

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
            await ImportStagingData(request);

            await MergeFeedbackTargetVariantsAsync();

            return Unit.Value;

        }

        private async Task ImportStagingData(ProcessFeedbackTargetVariantsCommand request)
        {
            var stagingRecords = await _stagingContext.GetAll();

            _stagingContext.RemoveRange(stagingRecords);

            await _stagingContext.AddRange(
                request.FeedbackTargetVariants.Select(v => (Domain.Entities.FeedbackTargetVariant_Staging)v).ToList());

            await _stagingContext.SaveChangesAsync();

            stagingRecords = await _stagingContext.GetAll();

        }
        private async Task MergeFeedbackTargetVariantsAsync()
        {
            var stagingData = await _stagingContext.GetAll();
            var existingData = await _context.GetAll();


            foreach (var stagingRecord in stagingData)
            {
                var existingRecord = existingData.FirstOrDefault(e => e.ApprenticeshipId == stagingRecord.ApprenticeshipId);

                if (existingRecord == null)
                {
                    _context.Add(new FeedbackTargetVariant
                    {
                        ApprenticeshipId = stagingRecord.ApprenticeshipId,
                        Variant = stagingRecord.Variant
                    });
                }
                else
                {
                    if (existingRecord.Variant != stagingRecord.Variant)
                    {
                        existingRecord.Variant = stagingRecord.Variant;
                    }
                }
            }

            var stagingIds = stagingData.Select(s => s.ApprenticeshipId).ToHashSet();
            var recordsToRemove = existingData
                .Where(e => !stagingIds.Contains(e.ApprenticeshipId)).ToList();

            if(recordsToRemove != null && recordsToRemove.Any())
            {
                _context.RemoveRange(recordsToRemove);
            }

            await _context.SaveChangesAsync();
        }
    }
}
