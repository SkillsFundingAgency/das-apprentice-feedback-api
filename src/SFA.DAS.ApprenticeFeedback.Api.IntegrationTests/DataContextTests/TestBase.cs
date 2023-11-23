using NUnit.Framework;
using SFA.DAS.ApprenticeFeedback.Api.IntegrationTests.Services;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Api.IntegrationTests
{
    public class TestBase
    {
        private readonly DatabaseService _databaseService = new DatabaseService();
        
        [OneTimeSetUp]
        public async Task Setup()
        {
            await _databaseService.CloneDatabase();
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            _databaseService.DropDatabase();
        }
    }
}
