using StackExchange.Redis;

namespace DemoWebAPI.Application.DTOs
{
    public class TokenResponseDTO
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;

        // Có thể thêm thời gian hết hạn để Client biết khi nào cần refresh
        public DateTime AccessTokenExpiration { get; set; }
    }
}