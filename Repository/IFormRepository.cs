using Backend.Dto;

namespace Backend.Repository;

public interface IFormRepository
{
    /// <summary>
    /// 新增 user 回應
    /// </summary>
    /// <param name="answers"></param>
    /// <returns></returns>
    public Task<Guid> InsertFormAsync(string answers);

    /// <summary>
    /// 取得所有回應
    /// </summary>
    /// <returns></returns>
    public Task<IEnumerable<FormListItemDto>> GetAllAsync();

    /// <summary>
    /// 依據Id 取得資料
    /// </summary>
    /// <returns></returns>
    public Task<IEnumerable<FormDetailDto>> QueryAsync(Guid[] ids);

    /// <summary>
    /// 依據id 取得資料
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public Task<FormDetailDto?> GetByIdAsync(Guid id);
}