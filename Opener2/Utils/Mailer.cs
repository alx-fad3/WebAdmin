using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace Opener2.Utils
{

    public class Mailer
    {
        private string _SmtpServer;
        private string _Sender;
        private string _SenderPass;
        private string _Reciever;
        private Attachment _Attachment = null;
        private string _Subject;
        private string _Body;

        private const string _emailPattern = @"^(?("")(""[^""]+?""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9]{2,17}))$";

        public Mailer(string smtpServer, string sender, string senderPass, string reciever, string attachmentPath, string subject, string body)
        {
            if (smtpServer != string.Empty)
            {
                _SmtpServer = smtpServer;
            }
            else
            {
                throw new Exception("Не указан сервер smtp");
            }

            if (sender != string.Empty)
            {
                if (Regex.IsMatch(sender, _emailPattern, RegexOptions.IgnoreCase))
                {
                    _Sender = sender;
                }
                else
                {
                    throw new Exception("Некорректный Email");
                }
            }
            else
            {
                throw new Exception("Не указан отправитель");
            }

            if (reciever != string.Empty)
            {
                if (Regex.IsMatch(sender, _emailPattern, RegexOptions.IgnoreCase))
                {
                    _Reciever = reciever;
                }
                else
                {
                    throw new Exception("Некорректный Email");
                }
            }
            else
                throw new Exception("Не указан получатель");

            if (senderPass != string.Empty)
                _SenderPass = senderPass;
            else
                throw new Exception("Не указан пароль отправителя");

            if (attachmentPath != string.Empty)
                _Attachment = new Attachment(attachmentPath);
            else
                _Attachment = null;

            _Subject = subject;
            _Body = body;
        }

        public Mailer(string smtpServer, string sender, string senderPass, string reciever, string subject, string body)
        {
            if (smtpServer != string.Empty)
            {
                _SmtpServer = smtpServer;
            }
            else
            {
                throw new Exception("Не указан сервер smtp");
            }

            if (sender != string.Empty)
            {
                if (Regex.IsMatch(sender, _emailPattern, RegexOptions.IgnoreCase))
                {
                    _Sender = sender;
                }
                else
                {
                    throw new Exception("Некорректный Email");
                }
            }
            else
            {
                throw new Exception("Не указан отправитель");
            }

            if (reciever != string.Empty)
            {
                if (Regex.IsMatch(sender, _emailPattern, RegexOptions.IgnoreCase))
                {
                    _Reciever = reciever;
                }
                else
                {
                    throw new Exception("Некорректный Email");
                }
            }
            else
                throw new Exception("Не указан получатель");

            if (senderPass != string.Empty)
                _SenderPass = senderPass;
            else
                throw new Exception("Не указан пароль отправителя");

            _Subject = subject;
            _Body = body;
        }

        public void SendMail()
        {
            MailMessage m = new MailMessage();
            m.From = new MailAddress(_Sender);
            m.To.Add(new MailAddress(_Reciever));
            m.IsBodyHtml = true;
            m.Subject = _Subject;
            m.Body = _Body;
            if (_Attachment != null)
            {
                m.Attachments.Add(_Attachment);
            }

            // для гугла нужно включать доступ для приложений https://myaccount.google.com/lesssecureapps, smtp.gmail.com, порт 587
            int port;
            switch (_SmtpServer)
            {
                case "smtp.gmail.com": port = 587; break;
                default: port = 25; break;
            }
            SmtpClient smtp = new SmtpClient(_SmtpServer, port);
            smtp.Credentials = new NetworkCredential(_Sender, _SenderPass);
            smtp.EnableSsl = true;
            //smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.Send(m);
        }

        public async void SendMailAsync()
        {
            await Task.Run(() => SendMail());
        }
    }
}