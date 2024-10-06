using Microsoft.AspNetCore.Mvc;
using ProjectSystem.Domain.Models;
using ProjectSystem.Repositories.Contacts;

namespace ProjectSystem.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentController : ControllerBase
    {
        private readonly IRepositoryManager _repositoryManager;

        public CommentController(IRepositoryManager repositoryManager)
        {
            _repositoryManager = repositoryManager;
        }

        [HttpGet("GetCommentTree/{commentId:guid}")]
        public async Task<IActionResult> GetCommentTree(Guid commentId)
        {
            var commentsTree = await _repositoryManager.CommentRepository.GetCommentTree(commentId);

            return Ok(commentsTree);
        }

        [HttpGet("GetRootComments")]
        public async Task<IActionResult> GetRootComments()
        {
            var rootComments = await _repositoryManager.CommentRepository.GetRootComments();

            return Ok(rootComments);
        }

        [HttpPost("AddComment")]
        public async Task<IActionResult> AddComment([FromBody] CreateCommentRequest comment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _repositoryManager.CommentRepository.AddComment(comment);
            return Ok();
        }
    }
}
