using MediatR;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.ApprenticeFeedback.Data;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Api.IntegrationTests
{
    public class TestFixture
    {
        private readonly IServiceScopeFactory _scopeFactory;

        private readonly WebApplicationFactory<Program> _factory;

        public TestFixture()
        {
            _factory = new TestWebApplicationFactory();

            _scopeFactory = _factory.Services.GetRequiredService<IServiceScopeFactory>();
        }

        public Task SendAsync(IRequest request)
        {
            return ExecuteScopeAsync(sp =>
            {
                var mediator = sp.GetRequiredService<IMediator>();

                return mediator.Send(request);
            });
        }

        public Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request)
        {
            return ExecuteScopeAsync(sp =>
            {
                var mediator = sp.GetRequiredService<IMediator>();

                return mediator.Send(request);
            });
        }

        public Task<T> ExecuteDbContextAsync<T>(Func<ApprenticeFeedbackDataContext, Task<T>> action)
        {
            return ExecuteScopeAsync(sp => action(sp.GetService<ApprenticeFeedbackDataContext>()));
        }

        public async Task ExecuteScopeAsync(Func<IServiceProvider, Task> action)
        {
            using var scope = _scopeFactory.CreateScope();
           
            await action(scope.ServiceProvider);
        }

        public async Task<T> ExecuteScopeAsync<T>(Func<IServiceProvider, Task<T>> action)
        {
            using var scope = _scopeFactory.CreateScope();

            var result = await action(scope.ServiceProvider);

            return result;
        }
    }
}
