using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.ApprenticeFeedback.Application.Services;
using SFA.DAS.ApprenticeFeedback.Data.Repository;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Api.AppStart
{
    public static class AddServiceRegistration
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddTransient<IApprenticeFeedbackRepository, ApprenticeFeedbackRepository>();

            services.AddTransient<IApprenticeFeedbackService, ApprenticeFeedbackService>();
        }
    }
}
