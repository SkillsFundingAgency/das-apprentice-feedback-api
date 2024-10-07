using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.ApprenticeFeedback.Domain.Entities;
using System;

namespace SFA.DAS.ApprenticeFeedback.Application.Commands.ProcessFeedbackTargetVariants
{
    public class ProcessFeedbackTargetVariantsCommandHandler : IRequestHandler<ProcessFeedbackTargetVariantsCommand>
    {
        private readonly IFeedbackTargetVariant_StagingContext _stagingContext;
        private readonly IFeedbackTargetVariantContext _context;
        private readonly ILogger<ProcessFeedbackTargetVariantsCommandHandler> _logger;
        private const int batchSize = 100; 

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
                await ClearStagingData();
            }
            
            await ImportStagingData(request);

            if(request.MergeStaging) 
            {
                await MergeFeedbackTargetVariantsAsync();
            }

            return Unit.Value;

        }

        private async Task ClearStagingData()
        {
            var stagingRecords = await _stagingContext.GetAll();

            _stagingContext.RemoveRange(stagingRecords);
        }

        private async Task ImportStagingData(ProcessFeedbackTargetVariantsCommand request)
        {
            await _stagingContext.AddRange(
                request.FeedbackTargetVariants.Select(v => (Domain.Entities.FeedbackTargetVariant_Staging)v).ToList());

            await _stagingContext.SaveChangesAsync();
        }

        private async Task MergeFeedbackTargetVariantsAsync()
        {
            var stagingData = await _stagingContext.GetAll();
            var existingData = await _context.GetAll();

            var stagingIds = stagingData.Select(s => s.ApprenticeshipId).ToHashSet();
            int changesCount = 0; 

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
                    changesCount++; 
                }
                else
                {
                    if (existingRecord.Variant != stagingRecord.Variant)
                    {
                        existingRecord.Variant = stagingRecord.Variant;
                        changesCount++; 
                    }
                }

                if (changesCount >= batchSize)
                {
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"Processed batch with {changesCount} changes.");
                    changesCount = 0; 
                }
            }

            if (changesCount > 0)
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Processed final batch with {changesCount} changes.");
                changesCount = 0;
            }

            var recordsToRemove = existingData.Where(e => !stagingIds.Contains(e.ApprenticeshipId)).ToList();
            _logger.LogInformation($"Starting to remove {recordsToRemove.Count} records in batches of {batchSize}.");

            for (int i = 0; i < recordsToRemove.Count; i += batchSize)
            {
                var batchToRemove = recordsToRemove.Skip(i).Take(batchSize).ToList();
                _context.RemoveRange(batchToRemove);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Removed batch {i / batchSize + 1} with {batchToRemove.Count} records.");
            }
        }


    }
}
