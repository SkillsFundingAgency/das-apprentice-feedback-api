using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
using SFA.DAS.NServiceBus.Services;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Application.NServiceBus
{
    public class CommandPublisher : ICommandPublisher
    {
        private readonly IEventPublisher _eventPublisher;

        public CommandPublisher(IEventPublisher eventPublisher)
        {
            _eventPublisher = eventPublisher;
        }
        
        public Task Publish(object command, CancellationToken cancellationToken = default)
        {
            return _eventPublisher.Publish(command);
        }
    }
}
