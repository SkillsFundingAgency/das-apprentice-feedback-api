using System;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.ApprenticeFeedback.Api.UoW
{
    public static class DbContextBuilderExtensions
    {
        public static DbContextOptionsBuilder<TContext> UseDataStorage<TContext>(
            this DbContextOptionsBuilder<TContext> builder, IConnectionFactory connectionFactory, string connection)
            where TContext : DbContext
        {
            return connectionFactory.AddConnection(builder, connection);
        }

        public static DbContextOptionsBuilder<TContext> UseDataStorage<TContext>(
            this DbContextOptionsBuilder<TContext> builder, IConnectionFactory connectionFactory, DbConnection connection)
            where TContext : DbContext
        {
            return connectionFactory.AddConnection(builder, connection);
        }

        public static DbContextOptionsBuilder<TContext> UseLocalSqlLogger<TContext>(
            this DbContextOptionsBuilder<TContext> builder, ILoggerFactory loggerFactory, IConfiguration config)
            where TContext : DbContext
        {
            if (config.IsLocalAcceptanceOrDev())
            {
                builder.EnableSensitiveDataLogging().UseLoggerFactory(loggerFactory);
            }

            return builder;
        }
    }
}
