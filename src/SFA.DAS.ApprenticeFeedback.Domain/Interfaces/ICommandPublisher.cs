using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Domain.Interfaces
{
    public interface ICommandPublisher
    {
        Task Publish(object command, CancellationToken cancellationToken = default(CancellationToken));
    }
}
