using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.ApprenticeFeedback.Application.Commands.CreateApprenticeFeedback;
using SFA.DAS.ApprenticeFeedback.Data;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;

namespace SFA.DAS.ApprenticeFeedback.Api.AppStart
{
    public static class AddServiceRegistration
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddMediatR(typeof(CreateApprenticeFeedbackCommand).Assembly);
            services.AddScoped<IApprenticeFeedbackTargetContext>(s => s.GetRequiredService<ApprenticeFeedbackDataContext>());
            services.AddScoped<IApprenticeFeedbackResultContext>(s => s.GetRequiredService<ApprenticeFeedbackDataContext>());
            services.AddScoped<IAttributeContext>(s => s.GetRequiredService<ApprenticeFeedbackDataContext>());
            services.AddScoped<IProviderAttributeContext>(s => s.GetRequiredService<ApprenticeFeedbackDataContext>());
            services.AddScoped<IDateTimeHelper, UtcTimeProvider>();
        }
    }
}
