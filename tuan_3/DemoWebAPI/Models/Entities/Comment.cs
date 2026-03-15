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
    }
}
