using System.Data.Common;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;

namespace SFA.DAS.ApprenticeFeedback.Api.StartupExtensions
{
    public class SqlServerConnectionFactory : IConnectionFactory
    {
        private readonly IConfiguration _configuration;
        private readonly IManagedIdentityTokenProvider _managedIdentityTokenProvider;

        public SqlServerConnectionFactory(IConfiguration configuration, IManagedIdentityTokenProvider managedIdentityTokenProvider)
        {
            _configuration = configuration;
            _managedIdentityTokenProvider = managedIdentityTokenProvider;
        }

        public DbContextOptionsBuilder<TContext> AddConnection<TContext>(DbContextOptionsBuilder<TContext> builder, string connection) where TContext : DbContext
        {
            return builder.UseSqlServer(CreateConnection(connection));
        }

        public DbContextOptionsBuilder<TContext> AddConnection<TContext>(DbContextOptionsBuilder<TContext> builder, DbConnection connection) where TContext : DbContext
        {
            return builder.UseSqlServer(connection);
        }

        public DbConnection CreateConnection(string connection)
        {
            var sqlConnection = new SqlConnection(connection)
            {
                AccessToken = GetAccessToken(),
            };

            return sqlConnection;
        }

        private string? GetAccessToken()
        {
            if (_configuration.IsLocalAcceptanceOrDev())
            {
                return null;
            }

            return _managedIdentityTokenProvider.GetSqlAccessTokenAsync()
                .GetAwaiter().GetResult();
        }
    }
}