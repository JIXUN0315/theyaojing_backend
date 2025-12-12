using Backend.Dto;
using Npgsql;

namespace Backend.Repository {
    public interface INewsRepository {

        Task<List<News>> GetAllAsync();

        Task<News?> GetByIdAsync(int id);
        Task<int> CreateAsync(News news);
        Task<bool> UpdateAsync(News req);
        Task<bool> UpdatePublishStatusAsync(int id, bool isPublished);
        Task<bool> DeleteAsync(int id);
    }
}
