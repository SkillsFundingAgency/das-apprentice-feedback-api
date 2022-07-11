using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace SFA.DAS.ApprenticeFeedback.Domain.Interfaces
{
    public interface IEntityContext<T> where T : class
    {
        DbSet<T> Entities { get; }

        EntityEntry<T> Add(T entity) => Entities.Add(entity);
    }
}
