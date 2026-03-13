namespace DemoWebAPI.Models.DTOs
{
	public class CreatePostDto
	{
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Image { get; set; }
        public required Guid UserId { get; set; }
    }

    public class UpdatePostDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Image { get; set; }
    }
}
