using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using ProjectSystem.Domain.Models;
using ProjectSystem.Domain.Responses;
using ProjectSystem.Repositories.Contacts;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;

namespace ProjectSystem.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentController : ControllerBase
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly string _captchaSecretKey;

        public CommentController(IRepositoryManager repositoryManager, IConfiguration configuration)
        {
            _repositoryManager = repositoryManager;
            _captchaSecretKey = configuration["Captcha:secretKey"];
        }

        [HttpGet("GetCommentTree/{commentId:guid}")]
        public async Task<IActionResult> GetCommentTree(Guid commentId)
        {
            var commentsTree = await _repositoryManager.CommentRepository.GetCommentTree(commentId);

            return Ok(commentsTree);
        }

        [HttpGet("GetRootComments")]
        public async Task<IActionResult> GetRootComments(int page, int pageSize, string? sortBy, string? sortOrder)
        {
            var paginatedComments = await _repositoryManager.CommentRepository.GetRootComments(page, pageSize, sortBy, sortOrder);
            return Ok(paginatedComments);
        }

        [HttpPost("AddComment")]
        public async Task<IActionResult> AddComment([FromForm] CreateCommentRequest comment)
        {
            if (comment.image != null && !ImageHelper.IsValidImage(comment.image))
            {
                return BadRequest("Invalid image format. Only JPG, PNG, and GIF are allowed.");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _repositoryManager.CommentRepository.AddComment(comment);
            return Ok();
        }
    }
}
