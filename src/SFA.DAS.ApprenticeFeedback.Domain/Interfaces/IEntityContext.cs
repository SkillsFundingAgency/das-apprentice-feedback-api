using Microsoft.EntityFrameworkCore;

namespace SFA.DAS.ApprenticeFeedback.Domain.Interfaces
{
    public interface IEntityContext<T> where T : class
    {
        DbSet<T> Entities { get; }

        /*
        EntityEntry<T> Add(T entity) => Entities.Add(entity);

        ValueTask<EntityEntry<T>> AddAsync(T entity, CancellationToken cancellationToken = default)
            => Entities.AddAsync(entity, cancellationToken);
        */
    }
}
