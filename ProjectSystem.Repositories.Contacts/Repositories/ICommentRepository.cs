using Microsoft.AspNetCore.Mvc;
using ProjectSystem.Domain.Entities;
using ProjectSystem.Domain.Models;

namespace ProjectSystem.Repositories.Contacts.Repositories
{
    public interface ICommentRepository
    {
        Task AddComment(CreateCommentRequest comment);

        Task<List<Comment>> GetCommentTree(Guid commentId);

        Task<List<Comment>> GetRootComments();
    }
}
