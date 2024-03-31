using AuthorizationApi.Authorization;
using AuthorizationApi.Models;
using AuthorizationApi.Models.DTO.Request;
using AuthorizationApi.Models.DTO.Session;
using AuthorizationApi.Settings;
using AuthorizationApi.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace AuthorizationApi.Services
{
    public interface IMailService
    {
        void SendMessage(MessageRequest mes);
        string SendKeyAuthorization(RegistrationRequest model);
        string SendCodeForUpdadePassword(User user);
    }

    public class MailService : IMailService
    {
        private readonly MailSetting _mailSettings;

        public MailService(IOptions<MailSetting> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }

        public string SendKeyAuthorization(RegistrationRequest model)
        {
            Guid key = Guid.NewGuid();

            MailAddress fromAddress = new MailAddress(_mailSettings.Mail);
            MailAddress toAddress = new MailAddress(model.Email, model.Username);
            MailMessage message = new MailMessage(fromAddress, toAddress);

            string text = "Для подтверждения авторизации перейдите по ссылке: " + model.PageСonfirmationLink + key;
            string fileContent = File.ReadAllText("Resources/File/MailAuthorization.txt");
            fileContent = fileContent.Replace("{subject}", "Подтверждение");
            fileContent = fileContent.Replace("{activationLink}", $"{text}");

            message.Subject = "Авторизация";
            message.Body = fileContent;
            message.IsBodyHtml = true;

            Send(message);

            return key.ToString();
        }



        public string SendCodeForUpdadePassword(User user)
        {
            string code = GenerationCodeUtils.Generation(4);

            MailAddress fromAddress = new MailAddress(_mailSettings.Mail);
            MailAddress toAddress = new MailAddress(user.Email, user.Username);
            MailMessage message = new MailMessage(fromAddress, toAddress);

            string text = "Код для смены пароля: " + code;
            string fileContent = File.ReadAllText("Resources/File/MailAuthorization.txt");
            fileContent = fileContent.Replace("{subject}", "Смена пароля");
            fileContent = fileContent.Replace("{activationLink}", $"{text}");

            message.Subject = "Настройки аккаунта";
            message.Body = fileContent;
            message.IsBodyHtml = true;

            Send(message);


            return code;
        }


        public void SendMessage(MessageRequest mes)
        {

            MailAddress fromAddress = new MailAddress(_mailSettings.Mail);
            MailAddress toAddress = new MailAddress(mes.Email, mes.Username);
            MailMessage message = new MailMessage(fromAddress, toAddress);

            string text = mes.Message;
            string fileContent = File.ReadAllText("Resources/File/MailAuthorization.txt");
            fileContent = fileContent.Replace("{subject}", "Сообщение");
            fileContent = fileContent.Replace("{activationLink}", $"{text}");

            message.Subject = mes.Subject;
            message.Body = fileContent;
            message.IsBodyHtml = true;

            Send(message);
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
