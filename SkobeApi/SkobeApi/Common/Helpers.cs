using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Mail;

namespace SkobeApi.Common
{
	public class Helpers
	{
		public bool SendMail(string to, string subject, string body)
		{

			var fromAddress = new MailAddress("skobelimited@gmail.com", "Skobe Limited");
			var toAddress = new MailAddress(to);

			var smtp = new SmtpClient
			{
				Host = "smtp.gmail.com",
				Port = 587,
				EnableSsl = true,
				DeliveryMethod = SmtpDeliveryMethod.Network,
				UseDefaultCredentials = false,
				Credentials = new NetworkCredential("skobelimited", "Cag*1411+")
			};
			using (var message = new MailMessage(fromAddress, toAddress)
			{
				Subject = subject,
				Body = body,
				IsBodyHtml = true
			})
			{
				smtp.Send(message);
			}
			return true;
		}
		public string Clean(string StringToClean)
		{
			if (StringToClean != null)
			{
				StringToClean = StringToClean.Replace("'", "´");
				StringToClean = StringToClean.Replace("<", "");
				StringToClean = StringToClean.Replace(">", "");
				StringToClean = StringToClean.Trim();
			}
			else
				StringToClean = "";
			return StringToClean;
		}

		public string Capitalize(string StringToCapitalize)
		{
			return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(StringToCapitalize);
		}

	}
}