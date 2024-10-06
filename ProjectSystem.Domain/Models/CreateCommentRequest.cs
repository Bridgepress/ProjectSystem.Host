namespace ProjectSystem.Domain.Models
{
    public class CreateCommentRequest
    {
        public string content { get; set; }
        public Guid userId { get; set; }
        public Guid? parentId { get; set; } = null;
    }
}
