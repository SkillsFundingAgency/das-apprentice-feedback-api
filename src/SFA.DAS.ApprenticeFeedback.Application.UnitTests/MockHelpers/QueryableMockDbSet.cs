using Microsoft.EntityFrameworkCore;
using Moq;
using System.Linq;

namespace SFA.DAS.ApprenticeFeedback.Application.UnitTests.MockHelpers
{
    public static class QueryableMockDbSet
    {
        public static DbSet<T> Create<T>(params T[] sourceList) where T : class
        {
            var queryable = sourceList.AsQueryable();

            var dbSet = new Mock<DbSet<T>>();
            dbSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
            dbSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
            dbSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            dbSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => queryable.GetEnumerator());

            return dbSet.Object;
        }
    }
}
