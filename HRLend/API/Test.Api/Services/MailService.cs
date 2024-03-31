using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using TestApi.Settings;
using TestApi.Utils;

namespace TestApi.Services
{
    public interface IMailService
    {
        string SendCodeForPassageTest(string to);
    }

    public class MailService : IMailService
    {
        private readonly MailSetting _mailSettings;

        public MailService(IOptions<MailSetting> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }



        public string SendCodeForPassageTest(string to)
        {
            string code = GenerationCodeUtils.Generation(4);

            MailAddress fromAddress = new MailAddress(_mailSettings.Mail);
            MailAddress toAddress = new MailAddress(to);
            MailMessage message = new MailMessage(fromAddress, toAddress);

            string text = "Введите данный код для начала теста: " + code;
            string fileContent = File.ReadAllText("Resources/File/MailCode.txt");
            fileContent = fileContent.Replace("{subject}", "Код");
            fileContent = fileContent.Replace("{activationLink}", $"{text}");

            message.Subject = "Код для теста";
            message.Body = fileContent;
            message.IsBodyHtml = true;

            Send(message);


            return code;
        }

        private void Send(MailMessage message)
        {
            SmtpClient smtpClient = new SmtpClient();
            smtpClient.Host = "smtp.yandex.ru";
            smtpClient.Port = 587;
            smtpClient.EnableSsl = true;
            smtpClient.Credentials = new NetworkCredential(_mailSettings.Mail, _mailSettings.Password);
            smtpClient.Send(message);
        }


    }
}
