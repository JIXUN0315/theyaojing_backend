using System.Data;
using System.Text.Json;
using Backend.Dto;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace Backend.Repository;

public class BlogPostRepository : IBlogPostRepository
{
    private readonly IDbConnection _db;

    public BlogPostRepository(IDbConnection db)
    {
        _db = db;
    }

    public async Task<int> CreateAsync(BlogPostDto post)
    {
        const string sql = @"
        INSERT INTO blog_posts
            (title, content, subtitle1, subtitle2, category, is_published, date, images, coverimage, is_featured)
        VALUES
            (@Title, @Content, @Subtitle1, @Subtitle2, @Category, @IsPublished, @Date, @ImagesJson::jsonb, @CoverImage, @IsFeatured)
        RETURNING id;
    ";

        return await _db.ExecuteScalarAsync<int>(sql, new
        {
            post.Title,
            post.Content,
            post.Subtitle1,
            post.Subtitle2,
            post.Category,
            post.IsPublished,
            post.Date,
            post.CoverImage,
            post.IsFeatured,
            post.ImagesJson
        });
    }

    public async Task<List<BlogPostDto>> GetAllAsync()
    {
        const string sql = @"
        SELECT
            id,
            title,
            content,
            subtitle1,
            subtitle2,
            category,
            is_published AS IsPublished,
            date AS Date,
            images::text AS ImagesJson,
            coverimage AS CoverImage,
            is_featured AS IsFeatured
        FROM blog_posts
        ORDER BY date DESC;
    ";

        var posts = await _db.QueryAsync<BlogPostDto>(sql);
        return posts.ToList();
    }

    public async Task<BlogPostDto?> GetByIdAsync(int id)
    {
        const string sql = @"
        SELECT
            id,
            title,
            content,
            subtitle1,
            subtitle2,
            category,
            is_published AS IsPublished,
            date AS Date,
            images::text AS ImagesJson,
            coverimage AS CoverImage,
            is_featured AS IsFeatured
        FROM blog_posts
        WHERE id = @id;
    ";

        return await _db.QueryFirstOrDefaultAsync<BlogPostDto>(sql, new { id });
    }
    public async Task UpdateAsync(BlogPostDto post)
    {
        const string sql = @"
        UPDATE blog_posts SET
            title = @Title,
            content = @Content,
            subtitle1 = @Subtitle1,
            subtitle2 = @Subtitle2,
            category = @Category,
            is_published = @IsPublished,
            coverimage= @CoverImage,
            date = @Date,
            is_featured = @IsFeatured,
            images = @Images::jsonb
        WHERE id = @Id;
    ";

        await _db.ExecuteAsync(sql, new
        {
            post.Id,
            post.Title,
            post.Content,
            post.Subtitle1,
            post.Subtitle2,
            post.Category,
            post.IsPublished,
            post.Date,
            post.CoverImage,
            post.IsFeatured,
            Images = JsonSerializer.Serialize(post.Images)
        });
    }

    public async Task DeleteAsync(int id)
    {
        const string sql = "DELETE FROM blog_posts WHERE id = @id";
        await _db.ExecuteAsync(sql, new { id });
    }

    public async Task<bool> UpdatePublishStatusAsync(int id, bool isPublished) {
        var sql = @"
                UPDATE blog_posts
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
}