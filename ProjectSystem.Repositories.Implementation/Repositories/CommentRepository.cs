using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectSystem.DataAccess;
using ProjectSystem.Domain.Entities;
using ProjectSystem.Domain.Models;
using ProjectSystem.Repositories.Contacts.Repositories;
using Microsoft.Extensions.Logging;
using System.Transactions;

namespace ProjectSystem.Repositories.Implementation.Repositories
{
    public class CommentRepository : RepositoryBase<Comment>, ICommentRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CommentRepository> _logger;

        public CommentRepository(ApplicationDbContext context, ILogger<CommentRepository> logger) : base(context)
        {
            _context = context;
            _logger = logger;
        }

        public async Task AddComment(CreateCommentRequest newCommentRequest)
        {
            // Используем транзакцию для атомарных операций
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    Comment parentComment = null;
                    if (newCommentRequest.parentId.HasValue)
                    {
                        parentComment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == newCommentRequest.parentId.Value);
                        if (parentComment == null)
                        {
                            throw new ArgumentException("Parent comment not found.");
                        }
                    }
                    var user = await ValidateUser(newCommentRequest.userId);
                    int newLeft;
                    if (parentComment != null)
                    {
                        newLeft = parentComment.Right;
                    }
                    else
                    {
                        newLeft = await _context.Comments.MaxAsync(c => (int?)c.Right) ?? 0;
                        newLeft += 1;
                    }
                    var newRight = newLeft + 1;
                    await ShiftComments(newLeft);
                    var newComment = new Comment
                    {
                        Content = newCommentRequest.content,
                        Left = newLeft,
                        Right = newRight,
                        ParentId = newCommentRequest.parentId,
                        UserId = newCommentRequest.userId
                    };
                    await _context.Comments.AddAsync(newComment);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while adding comment.");
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }

        private async Task<User> ValidateUser(Guid userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                throw new ArgumentException("User not found.");
            }
            return user;
        }

        private async Task ShiftComments(int newLeft)
        {
            var commentsToShiftRight = _context.Comments.Where(c => c.Right >= newLeft);
            await commentsToShiftRight.ForEachAsync(comment =>
            {
                comment.Right += 2;
            });
            var commentsToShiftLeft = _context.Comments.Where(c => c.Left > newLeft);
            await commentsToShiftLeft.ForEachAsync(comment =>
            {
                comment.Left += 2;
            });
            await _context.SaveChangesAsync();
        }

        public async Task<List<Comment>> GetCommentTree(Guid commentId)
        {
            var selectedComment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == commentId);
            if (selectedComment == null)
            {
                throw new ArgumentException("Comment not found.");
            }
            if (selectedComment.Left + 1 == selectedComment.Right)
            {
                return new List<Comment>();
            }
            var commentsTree = await _context.Comments
                .Where(c => c.ParentId == selectedComment.Id)
                .Include(c => c.User)
                .OrderBy(c => c.Left)
                .ToListAsync();

            return commentsTree;
        }


        public async Task<List<Comment>> GetRootComments()
        {
            return await _context.Comments
                .Where(c => c.ParentId == null)
                .Include(c => c.User)
                .OrderBy(c => c.Left)
                .ToListAsync();
        }
    }
}
