using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace entity_framework_core.Models.Entities
{
    [Table("users")]
    public class User
    {
        [Key]
        [Column("UserId")]
        public Guid Id { get; set; }

        [StringLength(50)]
        public required string FName { get; set; }

        [StringLength(50)]
        public required string LName { get; set; }

        [EmailAddress]
        [StringLength(50)]
        public required string Email { get; set; }

        [DataType(DataType.Password)]
        public required string Password { get; set; }

        [StringLength(20)]
        public string? PhoneNumber { get; set; }

        public virtual List<Post>? Posts { get; set; } = new List<Post>();
        public virtual List<Comment>? Comments { get; set; } = new List<Comment>();
    }
}
