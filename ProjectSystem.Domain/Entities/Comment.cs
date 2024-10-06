namespace ProjectSystem.Domain.Entities
{
    public class Comment : EntityBase
    {
        public string? Content { get; set; }
        public int Left { get; set; }  
        public int Right { get; set; } 
        public Guid? ParentId { get; set; }
        public Guid UserId { get; set; }
        public Comment? Parent { get; set; } 
        public ICollection<Comment> Children { get; set; } = new List<Comment>();
        public User? User { get; set; }
    }
}
