using Microsoft.Extensions.Configuration;
using System;

namespace SFA.DAS.ApprenticeFeedback.Api.StartupExtensions
{
    public static class ConfigurationExtensions
    {
        public static bool IsLocalAcceptanceOrDev(this IConfiguration config)
        {
            return config["EnvironmentName"].Equals("LOCAL", StringComparison.CurrentCultureIgnoreCase) ||
                   config["EnvironmentName"].Equals("ACCEPTANCE_TESTS", StringComparison.CurrentCultureIgnoreCase) ||
                   config["EnvironmentName"].Equals("DEV", StringComparison.CurrentCultureIgnoreCase);
        }

        public static bool IsIntegrationTests(this IConfiguration config)
        {
            return config["EnvironmentName"].Equals("IntegrationTests", StringComparison.CurrentCultureIgnoreCase);
        }
    }
}
