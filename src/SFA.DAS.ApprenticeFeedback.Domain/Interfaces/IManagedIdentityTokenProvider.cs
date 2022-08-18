using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Domain.Interfaces
{
    public interface IManagedIdentityTokenProvider
    {
        Task<string> GetSqlAccessTokenAsync();
    }
}