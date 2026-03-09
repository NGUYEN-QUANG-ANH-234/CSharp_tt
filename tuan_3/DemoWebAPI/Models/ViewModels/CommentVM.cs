using System;
using System.Collections.Generic;

namespace DemoWebAPI.Models.ViewModels
{
    public class CommentVM
    {
        public Guid Id { get; set; }
        public string Text { get; set; }
        public DateTime CreatedAt { get; set; }

        // Thông tin người dùng tối giản
        public Guid UserId { get; set; }
        public string UserName { get; set; }

        // Thông tin phân cấp
        public Guid? ParentCommentId { get; set; }
        public int ReplyCount { get; set; }
        public List<CommentVM> Replies { get; set; } = new List<CommentVM>();

        // Nếu muốn hỗ trợ load 1 vài reply mẫu
        public List<CommentVM> TopReplies { get; set; } = new();
    }
}
