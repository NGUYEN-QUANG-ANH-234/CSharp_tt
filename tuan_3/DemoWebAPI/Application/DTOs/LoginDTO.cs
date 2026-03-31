namespace DemoWebAPI.Application.DTOs
{
    public class LoginDTO
    {
        // Bạn có thể dùng [Required] để báo lỗi ngay nếu Client gửi thiếu
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}