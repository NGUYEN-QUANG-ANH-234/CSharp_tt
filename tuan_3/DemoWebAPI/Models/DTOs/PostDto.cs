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

    public class ReadPostDto 
    {  
        public int page { get; set; }
        public int pageSize { get; set; }
        public string? SortBy { get; set; }
        public string? Title { get; set; }
        public bool? IsDescending { get; set; }
    }
}
