using ProjectSystem.Repositories.Contacts.Repositories;

namespace ProjectSystem.Repositories.Contacts
{
    public interface IRepositoryManager
    {

        ICommentRepository CommentRepository { get; }

        Task<bool> SaveChangesAsync(CancellationToken token = default);
    }
}
