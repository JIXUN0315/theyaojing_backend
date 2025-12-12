using Backend.ViewModel;

namespace Backend.Services;

public interface IEmailService
{
    /// <summary>
    /// 寄信通知後台
    /// 信件內容 待討論
    /// </summary>
    /// <returns></returns>
    public Task SendAdminNotificationAsync(SubmitFormViewModel vm);
}