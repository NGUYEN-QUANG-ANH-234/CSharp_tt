namespace DemoWebAPI.Application.DTOs
{
    public class CreateCommentDto
    {
        public Guid UserId { get; set; }
        public string Text { get; set; } = string.Empty;
        public Guid? ParentCommentId { get; set; }
    }

    public class UpdateCommentDto
    {
        public string Text { get; set; } = string.Empty;
    }

    public class ReadCommentDto
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public string? SortBy { get; set; }
        public string? Text { get; set; }
        public bool? IsDescending { get; set; }
    }


    // VIEW
    public class CommentBasicVM
    {
        public Guid Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        // Thông tin người dùng tối giản
        public string UserName { get; set; } = string.Empty;

        // Thông tin phân cấp
        // Quan trọng để FE biết cấp bậc nếu cần
        public Guid? ParentCommentId { get; set; } 
        public int ReplyCount { get; set; }
    }

    public class CommentTreeVM : CommentBasicVM
    {
        public List<CommentTreeVM> Replies { get; set; } = new List<CommentTreeVM>();
    }

    public class CommentFlatVM : CommentBasicVM
    {
    }
}