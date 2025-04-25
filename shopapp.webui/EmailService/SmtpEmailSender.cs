using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using shopapp.webui.EmailService;

public class SmtpEmailSender : IEmailSender
{
    private readonly string _host;
    private readonly int _port;
    private readonly bool _enableSSL;
    private readonly string _username;
    private readonly string _password;

    public SmtpEmailSender(string host, int port, bool enableSSL, string username, string password)
    {
        _host = host;
        _port = port;
        _enableSSL = enableSSL;
        _username = username;
        _password = password;
    }

    public async Task SendEmailAsync(string email, string subject, string message)
    {
        var smtpClient = new SmtpClient(_host)
        {
            Port = _port,
            Credentials = new NetworkCredential(_username, _password),
            EnableSsl = _enableSSL
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress("altinayutku0@gmail.com"), // Burada kendi e-posta adresinizi girin
            Subject = subject,
            Body = message,
            IsBodyHtml = true,
        };

        mailMessage.To.Add(email);

        await smtpClient.SendMailAsync(mailMessage);
    }
}