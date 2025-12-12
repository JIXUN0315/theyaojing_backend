using System.Data;
using Backend.Dto;
using Dapper;
using Npgsql;

namespace Backend.Repository;

public class BlogPostRepository : IBlogPostRepository
{
    private readonly IDbConnection _db;

    public BlogPostRepository(IDbConnection db)
    {
        _db = db;
    }

    public async Task<Guid> CreateAsync(BlogPostDto post)
    {
        const string sql = @"
                INSERT INTO blog_posts (title, content, excerpt, featured_image_url, category, is_published)
                VALUES (@Title, @Content, @Excerpt, @FeaturedImageUrl, @Category, @IsPublished)
                RETURNING id;
            ";
        return await _db.ExecuteScalarAsync<Guid>(sql, post);
    }

    public async Task<List<BlogPostDto>> GetAllAsync()
    {
        const string sql = @"
            SELECT
                id,
                title,
                content,
                excerpt,
                category,
                is_published AS IsPublished,
                featured_image_url AS FeaturedImageUrl,
                created_at AS CreatedAt
            FROM blog_posts
            ORDER BY created_at DESC
            LIMIT 30;
        ";

        var posts = await _db.QueryAsync<BlogPostDto>(sql);
        return posts.ToList();
    }

    public async Task<BlogPostDto?> GetByIdAsync(Guid id)
    {
        const string sql = @"
            SELECT
                id,
                title,
                content,
                excerpt,
                category,
                is_published AS IsPublished,
                featured_image_url AS FeaturedImageUrl,
                created_at AS CreatedAt
            FROM blog_posts WHERE id = @id";
        return await _db.QueryFirstOrDefaultAsync<BlogPostDto>(sql, new { id });
    }

    public async Task UpdateAsync(BlogPostDto post)
    {
        const string sql = @"
                UPDATE blog_posts SET
                    title = @Title,
                    content = @Content,
                    excerpt = @Excerpt,
                    featured_image_url = @FeaturedImageUrl,
                    category = @Category,
                    is_published = @IsPublished
                WHERE id = @Id;
            ";
        await _db.ExecuteAsync(sql, post);
    }

    public async Task DeleteAsync(Guid id)
    {
        const string sql = "DELETE FROM blog_posts WHERE id = @id";
        await _db.ExecuteAsync(sql, new { id });
    }
}