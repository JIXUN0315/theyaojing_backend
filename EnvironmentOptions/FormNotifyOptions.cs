namespace Backend.Environment;

public class FormNotifyOptions
{
    public string Sender { get; set; } = "";
    public string SmtpUser { get; set; } = "";
    public string SmtpPass { get; set; } = "";
    public string SmtpHost { get; set; } = "";
    public string AdminEmail { get; set; } = "";
}