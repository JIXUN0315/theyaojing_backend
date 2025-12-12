using Backend.Dto;
using Dapper;
using Npgsql;
using System.Data;

namespace Backend.Repository {
    public class NewsRepository : INewsRepository {
        private readonly IDbConnection _db;

        public NewsRepository(IDbConnection db) {
            _db = db;
        }


        public async Task<List<News>> GetAllAsync() {
            var sql = @"SELECT 
                    id, title, content, img_url AS ImgUrl, date, 
                    is_published AS IsPublished
                FROM news
                ORDER BY date DESC";

            var posts = await _db.QueryAsync<News>(sql);
            return posts.ToList();
        }

        public async Task<News?> GetByIdAsync(int id) {
            var sql = @"SELECT id, title, content, img_url AS ImgUrl, date, is_published AS IsPublished
                    FROM news
                    WHERE id = @id";

            return await _db.QueryFirstOrDefaultAsync<News>(sql, new { id });
        }
        public async Task<int> CreateAsync(News req) {
            var sql = @"
                INSERT INTO news (title, content, img_url, date, is_published)
                VALUES (@Title, @Content, @ImgUrl, @Date, @IsPublished)
                RETURNING id;
            ";

            var id = await _db.ExecuteScalarAsync<int>(sql, req);
            return id;
        }

        public async Task<bool> UpdateAsync(News req) {
            var sql = @"
                UPDATE news
                SET
                    title = @Title,
                    content = @Content,
                    img_url = @ImgUrl,
                    date = @Date,
                    is_published = @IsPublished
                WHERE id = @Id
            ";
            var rows = await _db.ExecuteAsync(sql, req);
            return rows > 0;
        }
        public async Task<bool> UpdatePublishStatusAsync(int id, bool isPublished) {
            var sql = @"
                UPDATE news
                SET
                    is_published = @IsPublished
                WHERE id = @Id
            ";

            var rows = await _db.ExecuteAsync(sql, new {
                Id = id,
                IsPublished = isPublished
            });

            return rows > 0;
        }
        public async Task<bool> DeleteAsync(int id) {
            var sql = @"
                DELETE FROM news
                WHERE id = @Id
            ";

            var rows = await _db.ExecuteAsync(sql, new { Id = id });
            return rows > 0;
        }
    }
}
