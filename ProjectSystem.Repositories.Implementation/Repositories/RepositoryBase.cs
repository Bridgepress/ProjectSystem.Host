using ProjectSystem.DataAccess;
using ProjectSystem.Domain.Entities;
using ProjectSystem.Repositories.Contacts.Repositories;

namespace ProjectSystem.Repositories.Implementation.Repositories
{
    public class RepositoryBase<TEntity> : IRepositoryBase<TEntity>
        where TEntity : EntityBase
    {
        protected readonly ApplicationDbContext Context;

        protected RepositoryBase(ApplicationDbContext context)
        {
            Context = context;
        }
    }
}
