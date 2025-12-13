using System.Text.Json;
using Backend.ViewModel;
using Org.BouncyCastle.Ocsp;

namespace Backend.Dto;


public class BlogPostDto
{
    public BlogPostDto()
    {
        
    }
    
    public BlogPostDto(CreateBlogPostViewModel vm)
    {
        this.Title = vm.Title;
        this.Content = vm.Content;
        this.Subtitle1 = vm.Subtitle1;
        this.Subtitle2 = vm.Subtitle2;
        this.Category = vm.Category;
        this.IsPublished = vm.IsPublished;
        this.Date = vm.Date;
        this.CoverImage = vm.CoverImage;
        this.ImagesJson = JsonSerializer.Serialize(vm.GetImagesOrDefault());
    }
    
    public int Id { get; set; }                 
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;

    public string? Subtitle1 { get; set; }
    public string? Subtitle2 { get; set; }

    public string Category { get; set; } = string.Empty;
    public bool IsPublished { get; set; }

    public DateTime Date { get; set; }

    public string ImagesJson { get; set; } = "[]";
    
    public string CoverImage { get; set; }
    
    public List<BlogImageDto> Images =>
        JsonSerializer.Deserialize<List<BlogImageDto>>(
            ImagesJson,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }
        ) ?? new List<BlogImageDto>();
}

public class BlogImageDto
{
    public string Url { get; set; } = string.Empty;
    public int Sort { get; set; }
}
public class BlogPostListDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = "";
    public string? FeaturedImageUrl { get; set; }
    public string? Category { get; set; }
    public bool IsPublished { get; set; }
    public DateTime CreatedAt { get; set; }
}