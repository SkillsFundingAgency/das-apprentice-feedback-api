using Microsoft.Extensions.Configuration;
using System;

namespace SFA.DAS.ApprenticeFeedback.Extensions
{
    public static class ConfigurationExtensions
    {
        public static bool IsLocalOrDev(this IConfiguration config)
        {
            return config["EnvironmentName"].Equals("LOCAL", StringComparison.CurrentCultureIgnoreCase) ||
                   config["EnvironmentName"].Equals("DEV", StringComparison.CurrentCultureIgnoreCase);
        }
    }
}
