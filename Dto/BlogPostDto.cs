namespace Backend.Dto;


public class BlogPostDto
{
    public Guid? Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? Excerpt { get; set; }
    public string? FeaturedImageUrl { get; set; }
    public string Category { get; set; } = string.Empty;
    public bool IsPublished { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class BlogPostListDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = "";
    public string? Excerpt { get; set; }
    public string? FeaturedImageUrl { get; set; }
    public string? Category { get; set; }
    public bool IsPublished { get; set; }
    public DateTime CreatedAt { get; set; }
}