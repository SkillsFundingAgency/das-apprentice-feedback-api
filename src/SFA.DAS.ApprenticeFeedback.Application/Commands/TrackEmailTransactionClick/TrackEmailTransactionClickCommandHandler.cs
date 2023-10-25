using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.ApprenticeFeedback.Domain.Entities;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Application.Commands.TrackEmailTransactionClick
{
    public class TrackEmailTransactionClickCommandHandler : IRequestHandler<TrackEmailTransactionClickCommand, TrackEmailTransactionClickResponse>
    {
        private readonly IFeedbackTransactionContext _contextFeedbackTransaction;
        private readonly IFeedbackTransactionClickContext _contextFeedbackTransactionClick;
        private readonly ILogger<TrackEmailTransactionClickCommandHandler> _logger;

        public TrackEmailTransactionClickCommandHandler(
            IFeedbackTransactionContext contextFeedbackTransaction
            , IFeedbackTransactionClickContext contextFeedbackTransactionClick
            , ILogger<TrackEmailTransactionClickCommandHandler> logger
            )
        {
            _contextFeedbackTransaction = contextFeedbackTransaction;
            _contextFeedbackTransactionClick = contextFeedbackTransactionClick;
            _logger = logger;
        }

        public async Task<TrackEmailTransactionClickResponse> Handle(TrackEmailTransactionClickCommand request, CancellationToken cancellationToken)
        {
            var clickStatus = ClickStatus.Invalid;
            var feedbackTransaction = await _contextFeedbackTransaction.FindByIdIncludeFeedbackTargetAsync(request.FeedbackTransactionId);

            if (null != feedbackTransaction && feedbackTransaction.ApprenticeFeedbackTargetId == request.ApprenticeFeedbackTargetId)
            {
                var feedbackTransactionClick = new FeedbackTransactionClick
                {
                    ApprenticeFeedbackTargetId = request.ApprenticeFeedbackTargetId,
                    FeedbackTransactionId = request.FeedbackTransactionId,
                    LinkName = request.LinkName,
                    LinkUrl = request.LinkUrl,
                    ClickedOn = request.ClickedOn
                };

                _contextFeedbackTransactionClick.Entities.Add(feedbackTransactionClick);
                await _contextFeedbackTransactionClick.SaveChangesAsync();
                clickStatus = ClickStatus.Valid;
            }
            else
            {
                _logger.LogWarning($"Invalid email click there is no FeedbackTransaction {request.FeedbackTransactionId} with ApprenticeFeedbackTarget {request.ApprenticeFeedbackTargetId}.");
            }

            return new TrackEmailTransactionClickResponse(feedbackTransaction.Id, clickStatus);
        }
    }
}
