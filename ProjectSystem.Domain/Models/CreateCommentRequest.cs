using Microsoft.AspNetCore.Http;

namespace ProjectSystem.Domain.Models
{
    public class CreateCommentRequest
    {
        public string content { get; set; }
        public Guid userId { get; set; }
        public Guid? parentId { get; set; } = null;
        public string? captchaToken { get; set; }
        [FormFileValidation(new[] { ".txt" }, 102400)]
        public IFormFile? textFile { get; set; }
        public IFormFile? image { get; set; }
    }
}
