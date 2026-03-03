namespace entity_framework_core.Models.DTOs
{
    public class CommentDto
    {
        public Guid Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public string AuthorName { get; set; } = string.Empty;
        public List<CommentDto> Replies { get; set; } = new List<CommentDto>();
    }
}