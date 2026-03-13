using DemoWebAPI.Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DemoWebAPI.Models.ViewModels 
{
 public class PostBasicVM 
    {
        public Guid Id { get; set; }
        
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; } = string.Empty;

        public string? Image { get; set; }
        public DateTime CreatedAt { get; set; }

        // Thông tin người dùng tối giản
        public string AuthorName { get; set; } = string.Empty;
        public int CommentCount { get; set; }
    }
}
