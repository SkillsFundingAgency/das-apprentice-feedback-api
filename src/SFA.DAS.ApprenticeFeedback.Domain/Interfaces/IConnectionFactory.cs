using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace SFA.DAS.ApprenticeFeedback.Domain.Interfaces
{
    public interface IConnectionFactory
    {
        DbContextOptionsBuilder<TContext> AddConnection<TContext>(DbContextOptionsBuilder<TContext> builder, string connection) where TContext : DbContext;
        DbContextOptionsBuilder<TContext> AddConnection<TContext>(DbContextOptionsBuilder<TContext> builder, DbConnection connection) where TContext : DbContext;
        DbConnection CreateConnection(string connection);
    }
}