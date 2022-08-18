using System.Threading.Tasks;
using Microsoft.Azure.Services.AppAuthentication;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;

namespace SFA.DAS.ApprenticeFeedback.Api.StartupExtensions
{
    public class ManagedIdentityTokenProvider : IManagedIdentityTokenProvider
    {
        public Task<string> GetSqlAccessTokenAsync()
        {
            var provider = new AzureServiceTokenProvider();
            return provider.GetAccessTokenAsync("https://database.windows.net/");
        }
    }
}