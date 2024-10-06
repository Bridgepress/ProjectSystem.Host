using Microsoft.Extensions.DependencyInjection;
using ProjectSystem.DataAccess;
using ProjectSystem.Repositories.Contacts;
using ProjectSystem.Repositories.Contacts.Repositories;

namespace ProjectSystem.Repositories.Implementation
{
    public class RepositoryManager : IRepositoryManager
    {
        private readonly ApplicationDbContext _context;
        private readonly IServiceProvider _serviceProvider;

        public RepositoryManager(ApplicationDbContext context, IServiceProvider serviceProvider)
        {
            _context = context;
            _serviceProvider = serviceProvider;
        }

        public ICommentRepository CommentRepository => _serviceProvider.GetRequiredService<ICommentRepository>();

        public async Task<bool> SaveChangesAsync(CancellationToken token)
        {
            return await _context.SaveChangesAsync(token) > 0;
        }
    }
}
