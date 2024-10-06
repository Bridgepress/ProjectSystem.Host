using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProjectSystem.DataAccess;
using ProjectSystem.Domain.Entities;
using ProjectSystem.Domain.Models;
using ProjectSystem.Domain.Responses;
using ProjectSystem.Repositories.Contacts.Repositories;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Gif;

namespace ProjectSystem.Repositories.Implementation.Repositories
{
    public class CommentRepository : RepositoryBase<Comment>, ICommentRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CommentRepository> _logger;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public CommentRepository(ApplicationDbContext context, ILogger<CommentRepository> logger, IWebHostEnvironment hostingEnvironment) : base(context)
        {
            _context = context;
            _logger = logger;
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task AddComment(CreateCommentRequest newCommentRequest)
        {
            string? imageBase64 = null;

            // Обработка изображения
            if (newCommentRequest.image != null)
            {
                imageBase64 = await ProcessImageAsync(newCommentRequest.image);
            }

            string? textFileBase64 = null;
            if (newCommentRequest.textFile != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await newCommentRequest.textFile.CopyToAsync(memoryStream);
                    textFileBase64 = Convert.ToBase64String(memoryStream.ToArray());
                }
            }

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
                        UserId = newCommentRequest.userId,
                        CreatedAt = DateTime.UtcNow,
                        ImageBase64 = imageBase64,
                        TextFileBase64 = textFileBase64
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


        public async Task<PaginatedResponse<Comment>> GetRootComments(int page, int pageSize, string? sortBy, string? sortOrder)
        {
            var query = _context.Comments
                .Where(c => c.ParentId == null)
                .Include(c => c.User)
                .AsQueryable();

            switch (sortBy)
            {
                case "userName":
                    query = sortOrder == "asc" ? query.OrderBy(c => c.User.UserName) : query.OrderByDescending(c => c.User.UserName);
                    break;
                case "email":
                    query = sortOrder == "asc" ? query.OrderBy(c => c.User.Email) : query.OrderByDescending(c => c.User.Email);
                    break;
                default:
                    query = sortOrder == "asc" ? query.OrderBy(c => c.CreatedAt) : query.OrderByDescending(c => c.CreatedAt);
                    break;
            }
            var totalComments = await query.CountAsync();
            var comments = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResponse<Comment>(totalComments, comments);
        }

        private async Task<string?> ProcessImageAsync(IFormFile image)
        {
            string? imageBase64 = null;

            var extension = Path.GetExtension(image.FileName).ToLower();
            var allowedFormats = new[] { ".jpg", ".jpeg", ".png", ".gif" };

            if (allowedFormats.Contains(extension))
            {
                using (var imageStream = image.OpenReadStream())
                {
                    using (var img = await Image.LoadAsync(imageStream))
                    {
                        if (img.Width > 320 || img.Height > 240)
                        {
                            img.Mutate(x => x.Resize(new ResizeOptions
                            {
                                Size = new Size(320, 240),
                                Mode = ResizeMode.Max
                            }));
                        }

                        using (var memoryStream = new MemoryStream())
                        {
                            if (extension == ".jpg" || extension == ".jpeg")
                            {
                                var jpegEncoder = new JpegEncoder { Quality = 75 };
                                await img.SaveAsync(memoryStream, jpegEncoder);
                            }
                            else if (extension == ".png")
                            {
                                var pngEncoder = new PngEncoder();
                                await img.SaveAsync(memoryStream, pngEncoder);
                            }
                            else if (extension == ".gif")
                            {
                                var gifEncoder = new GifEncoder();
                                await img.SaveAsync(memoryStream, gifEncoder);
                            }

                            imageBase64 = Convert.ToBase64String(memoryStream.ToArray());
                        }
                    }
                }
            }
            else
            {
                throw new ArgumentException("Invalid image format. Only JPG, PNG, and GIF are allowed.");
            }

            return imageBase64;
        }
    }
}
