using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace PM.Business.Email
{
    public static class Helper
    {
        public static void Send(IConfiguration configuration, string subject, string body, List<string> to)
        {
            var smtpServerUrl = configuration.GetValue<string>("Email:SmtpServerUrl");
            var smtpServerPort = configuration.GetValue<int?>("Email:SmtpServerPort");
            var emailFrom = configuration.GetValue<string>("Email:From");
            var password = configuration.GetValue<string>("Email:Password");

            var smtp = new SmtpClient(smtpServerUrl);

            if (smtpServerPort.HasValue)
                smtp.Port = smtpServerPort.Value;

            if (string.IsNullOrEmpty(password) == false)
                smtp.Credentials = new NetworkCredential(emailFrom, password);

            var message = new MailMessage
            {
                From = new MailAddress(emailFrom),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            foreach (var item in to)
            {
	            if (item != null)
	            {
					message.To.Add(new MailAddress(item));
				}
            }

            smtp.Send(message);
        }
    }
}
