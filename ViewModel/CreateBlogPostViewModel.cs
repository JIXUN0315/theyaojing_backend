using Backend.Dto;

namespace Backend.ViewModel;

public class CreateBlogPostViewModel
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;

    public string? Subtitle1 { get; set; }
    public string? Subtitle2 { get; set; }

    public string Category { get; set; } = string.Empty;
    public bool IsPublished { get; set; }
    public DateTime Date { get; set; }
    
    public List<BlogImageDto>? Images { get; set; }
    
    public string  CoverImage { get; set; }
    
    public List<BlogImageDto> GetImagesOrDefault()
    {
        if (Images != null && Images.Any())
            return Images;

        return new List<BlogImageDto>
        {
            new BlogImageDto
            {
                Url = "https://res.cloudinary.com/drmlihopq/image/upload/v1765616589/news/oca3vnj789e6nlnzfq7q.png",
                Sort = 0
            }
        };
    }
}