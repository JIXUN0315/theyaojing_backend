using Backend.Dto;

namespace Backend.Repository;

public interface IBlogPostRepository
{
    Task<int> CreateAsync(BlogPostDto post);
    Task<List<BlogPostDto>> GetAllAsync();
    Task<BlogPostDto?> GetByIdAsync(int id);
    Task UpdateAsync(BlogPostDto post);
    Task DeleteAsync(int id);
}
