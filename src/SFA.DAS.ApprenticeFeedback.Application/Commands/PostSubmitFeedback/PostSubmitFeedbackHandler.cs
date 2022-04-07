using MediatR;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Application.Commands.PostSubmitFeedback
{
    public class PostSubmitFeedbackHandler : IRequestHandler<PostSubmitFeedbackCommand, PostSubmitFeedbackResponse>
    {
        public readonly IPostSubmitFeedback _postSubmitFeedback;

        public PostSubmitFeedbackHandler(IPostSubmitFeedback postSubmitFeedback)
        {
            _postSubmitFeedback = postSubmitFeedback;
        }

        public Task<PostSubmitFeedbackResponse> Handle(PostSubmitFeedbackCommand request, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
            //need code here
        }
    }
}
