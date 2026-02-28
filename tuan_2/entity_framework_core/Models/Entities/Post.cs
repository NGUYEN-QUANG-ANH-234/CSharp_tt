using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace entity_framework_core.Models.Entities
{
    [Table("posts")]
    public class Post
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [StringLength(2000)] 
        public string? Description { get; set; } = string.Empty;

        [StringLength(500)] 
        public string? Image { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Foreign key
        public required int UserId { get; set; }

        [ForeignKey("UserId")]
        public required User Author { get; set; }
        // -------------------------------
    }
}
