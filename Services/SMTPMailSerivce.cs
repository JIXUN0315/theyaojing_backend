using Backend.Environment;
using Backend.ViewModel;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Backend.Services;

public class MailKitService: IEmailService
{
    private readonly IConfiguration _config;
    private readonly FormNotifyOptions _options;


    public MailKitService(IConfiguration config,
        IOptions<FormNotifyOptions> options)
    {
        _options = options.Value;
        _config = config;
    }

    public async Task SendAdminNotificationAsync(SubmitFormViewModel viewModel)
    {
        var message = new MimeMessage();

        message.From.Add(new MailboxAddress("表單通知", _options.Sender));
        message.To.Add(new MailboxAddress("Admin", _options.AdminEmail));
        message.Subject = "🔔 收到一筆新表單提交";

        var bodyText = $@"有人剛剛填寫了表單
                        請至後台查看完整內容。
                        摘要：
                        姓名：{viewModel.FullName}
                        Email：{viewModel.Email}
                        電話/LINE：{viewModel.PhoneOrLine}
                        就讀/畢業學校：{viewModel.School}
                        科系：{viewModel.Department}
                        目標國家：{viewModel.TargetCountry}
                        想解決的問題：{string.Join(", ", viewModel.QuestionToResolve ?? Array.Empty<string>())}
                        想了解課程類別：{viewModel.ProgramType}
                        欲就讀科系：{string.Join(", ", viewModel.IntendedMajor ?? Array.Empty<string>())}
                        預計年份：{viewModel.DepartYear}
                        來源：{viewModel.Referral}
                        諮詢方式：{viewModel.AskType}
                        其他補充：{viewModel.OtherInfo}
                        ";


        message.Body = new TextPart("plain") { Text = bodyText };

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(_options.SmtpHost, 587, SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync(_options.SmtpUser, _options.SmtpPass);
        await smtp.SendAsync(message);
        await smtp.DisconnectAsync(true);
    }
}