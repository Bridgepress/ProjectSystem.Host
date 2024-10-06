using Microsoft.AspNetCore.Mvc;
using ProjectSystem.Domain.Entities;
using ProjectSystem.Domain.Models;
using ProjectSystem.Domain.Requests;
using ProjectSystem.Domain.Responses;

namespace ProjectSystem.Repositories.Contacts.Repositories
{
    public interface ICommentRepository
    {
        Task AddComment(CreateCommentRequest comment);

        Task<List<Comment>> GetCommentTree(Guid commentId);

        Task<PaginatedResponse<Comment>> GetRootComments(int page, int pageSize);
    }
}
