using DemoWebAPI.Core.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoWebAPI.Core.Entities
{
    [Table("users")]
    public class User
    {
        [Key]
        [Column("UserId")]
        public Guid Id { get; set; } = Guid.NewGuid();

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

        // Liên kết với Enum ở trên
        public UserRoles Role { get; set; } = UserRoles.User;

        // --- Dành cho Refresh Token ---

        // Chuỗi ngẫu nhiên dài được lưu xuống DB
        public string? RefreshToken { get; set; }

        // Thời hạn của Refresh Token (thường là 7-30 ngày)
        // Dùng để kiểm tra xem Token còn dùng để đổi Access Token mới được không
        public DateTime? RefreshTokenExpiryTime { get; set; }
    }
}
