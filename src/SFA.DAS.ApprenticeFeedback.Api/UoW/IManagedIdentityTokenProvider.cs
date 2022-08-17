using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Api.UoW
{
    public interface IManagedIdentityTokenProvider
    {
        Task<string> GetSqlAccessTokenAsync();
    }
}