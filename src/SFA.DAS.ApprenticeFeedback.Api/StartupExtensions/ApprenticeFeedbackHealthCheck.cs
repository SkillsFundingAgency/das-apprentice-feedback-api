using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Api.StartupExtensions
{
    public class ApprenticeFeedbackHealthCheck : IHealthCheck
    {
        private const string HealthCheckResultsDescription = "Apprentice Feedback API Health Check";
        private readonly IAttributeContext _attributeContext;

        public ApprenticeFeedbackHealthCheck(IAttributeContext attributeContext)
        {
            _attributeContext = attributeContext;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var dbConnectionHealthy = true;
            try
            {
                await _attributeContext.Entities.ToListAsync();
            }
            catch
            {
                dbConnectionHealthy = false;
            }

            return dbConnectionHealthy ? HealthCheckResult.Healthy(HealthCheckResultsDescription) : HealthCheckResult.Unhealthy(HealthCheckResultsDescription);
        }
    }
}