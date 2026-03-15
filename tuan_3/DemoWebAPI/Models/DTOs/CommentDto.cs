namespace DemoWebAPI.Models.DTOs
{
    public class CreateCommentDto
    {
        public Guid UserId { get; set; }
        public Guid PostId { get; set; }
        public string Text { get; set; } = string.Empty;
        public Guid? ParentCommentId { get; set; }

    }

    public class UpdateCommentDto
    {
        public string Text { get; set; } = string.Empty;
    }

    public class ReadCommentDto
    {
        public int page { get; set; }
        public int pageSize { get; set; }
        public string? SortBy { get; set; }
        public string? Text { get; set; }
        public bool? IsDescending { get; set; }
    }
}