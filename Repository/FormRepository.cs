using System.Data;
using System.Text.Json;
using Backend.Dto;
using Dapper;

namespace Backend.Repository;

public class FormRepository: IFormRepository
{
    private readonly IDbConnection _db;
    public FormRepository(IDbConnection db) => _db = db;

    public async Task<Guid> InsertFormAsync(string answers)
    {
        var id = Guid.NewGuid();
        var createdAt = DateTime.UtcNow;

        const string sql = @"
            INSERT INTO form_responses (id, answers, created_at)
            VALUES (@Id, @Answers::jsonb, @CreatedAt);
        ";

        await _db.ExecuteAsync(sql, new {
            Id = id,
            Answers = answers,
            CreatedAt = createdAt
        });

        return id;
    }
    
    public async Task<IEnumerable<FormListItemDto>> GetAllAsync()
    {
        const string sql = @"
            SELECT id,
                   created_at AS CreatedAt,
                   answers::text AS answers
            FROM form_responses
            ORDER BY created_at DESC;
        ";

        return await _db.QueryAsync<FormListItemDto>(sql);
    }

    public async Task<FormDetailDto?> GetByIdAsync(Guid id)
    {
        const string sql = @"
            SELECT id,
                   created_at AS CreatedAt,
                   answers::text AS answers
            FROM form_responses
            WHERE id = @Id;
        ";

        return await _db.QueryFirstOrDefaultAsync<FormDetailDto>(sql, new { Id = id });
    }
}