using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace entity_framework_core.Models.Entities
{
    [Table("comments")]
    public class Comment
    {
        [Key]
        public int Id { get; set; }

        [StringLength(50)]
        public required string Text { get; set; }

        // Self-Reference
        public int? ParentCommentId { get; set; } 

        [ForeignKey("ParentCommentId")]
        public Comment? ParentComment { get; set; }

        public List<Comment> Replies { get; set; } = new List<Comment>();
        // -------------------------------

        // Foreign key
        public required int PostId { get; set; }

        [ForeignKey("PostId")]
        [Required]
        public required Post Post { get; set; }

        public required int UserId { get; set; }

        [ForeignKey("UserId")]
        [Required]                                                                                                                                          
        public required User User { get; set; }
        // -------------------------------
    }
}
