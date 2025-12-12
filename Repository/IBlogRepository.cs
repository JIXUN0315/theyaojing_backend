using Backend.Dto;

namespace Backend.Repository;

public interface IBlogPostRepository
{
    Task<Guid> CreateAsync(BlogPostDto post);
    Task<List<BlogPostDto>> GetAllAsync();
    Task<BlogPostDto?> GetByIdAsync(Guid id);
    Task UpdateAsync(BlogPostDto post);
    Task DeleteAsync(Guid id);
}
