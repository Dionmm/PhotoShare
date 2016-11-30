using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Services.Protocols;
using SendGrid;

namespace PhotoShare.EmailProvider
{
    public class Email
    {
        private string _recipient;
        private string _apiKey;
        public Email()
        {
            Message = new SendGridMessage
            {
                From = new MailAddress("confirm@photoshare.party"),
                Subject = "Confirm your email address"
            };

        }

        public string Recipient
        {
            get { return _recipient; }
            set
            {
                _recipient = value;
                Message.AddTo(value);
            }
        }

        public string ConfirmationCode { get; set; }

        public SendGridMessage Message { get; set; }

        private string ApiKey
        {
            get { return _apiKey ?? ConfigurationManager.AppSettings.Get("SendGridApiKey"); }
            set { _apiKey = value; }
        }

        public Task Send()
        {
            var url = string.Format("https://www.photoshare.party/api/user/emailconfirm?code={0}", HttpUtility.UrlEncode(ConfirmationCode));
            Message.Html = $"<h2>Welcome to PhotoShare</h2><br/><a href=\"{url}\">Please confirm your email address here</a>";
            var transportWeb = new Web(ApiKey);
            return transportWeb.DeliverAsync(Message);
        }
    }
}