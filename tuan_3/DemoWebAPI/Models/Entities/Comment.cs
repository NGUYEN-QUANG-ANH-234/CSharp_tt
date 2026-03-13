using DemoWebAPI.Models.ViewModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoWebAPI.Models.Entities
{
    [Table("comments")]
    public class Comment
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [StringLength(50)]
        public required string Text { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Self-Reference

        [ForeignKey("ParentCommentId")]
        public Guid? ParentCommentId { get; set; }
        
        public virtual Comment? ParentComment { get; set; }

        public virtual List<Comment>? Replies { get; set; } = new List<Comment>();


        // -------------------------------

        // Foreign key
        public required Guid PostId { get; set; }

        [ForeignKey("PostId")]
        [Required]
        public virtual Post? Post { get; set; }

        public required Guid UserId { get; set; }

        [ForeignKey("UserId")]
        [Required]                                                                                                                                          
        public virtual User? User { get; set; }
        // -------------------------------

        //private List<CommentVM> MapToDto(List<Comment> entities)
        //{
        //    return entities.Select(e => new CommentVM
        //    {
        //        Id = e.Id,
        //        Text = e.Text,
        //        UserName = e.User?.FName + " " + e.User?.LName,
        //        Replies = e.Replies != null ? MapToDto(e.Replies) : new List<CommentVM>() // Đệ quy map các con
        //    }).ToList();
        //}
    }
}
