using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Domain.Interfaces
{
    public interface IEntityContext<T> where T : class
    {
        DbSet<T> Entities { get; }

        /*
        ValueTask<EntityEntry<T>> AddAsync(T entity, CancellationToken cancellationToken = default)
            => Entities.AddAsync(entity, cancellationToken);
        */

        
        EntityEntry<T> Add(T entity) => Entities.Add(entity);

        
    }
}
