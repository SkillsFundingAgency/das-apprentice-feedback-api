using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.ApprenticeFeedback.Application.Commands.CreateApprenticeFeedbackTarget;
using SFA.DAS.ApprenticeFeedback.Data;
using SFA.DAS.ApprenticeFeedback.Data.Repository;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;

namespace SFA.DAS.ApprenticeFeedback.Api.AppStart
{
    public static class AddServiceRegistration
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddMediatR(typeof(CreateApprenticeFeedbackTargetCommand).Assembly);
            services.AddScoped<IApprenticeFeedbackDataContext>(s => s.GetRequiredService<ApprenticeFeedbackDataContext>());
            services.AddScoped<IApprenticeFeedbackTargetContext>(s => s.GetRequiredService<ApprenticeFeedbackDataContext>());
            services.AddScoped<IApprenticeFeedbackRepository, ApprenticeFeedbackRepository>();
            services.AddScoped<IDateTimeHelper, UtcTimeProvider>();
        }
    }
}
