using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoWebAPI.Models.Entities
{
    [Table("posts")]
    public class Post
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [StringLength(2000)] 
        public string? Description { get; set; } = string.Empty;

        [StringLength(500)] 
        public string? Image { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Foreign key
        public required Guid UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User? Author { get; set; }
        // -------------------------------

        public virtual List<Comment>? Comments { get; set; } = new List<Comment>();
    }
}
