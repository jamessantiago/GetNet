using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NLog;
using NLog.Targets;
using NLog.Config;
using NLog.Layouts;
using MailKit;
using MailKit.Net.Smtp;
using MimeKit;

namespace getnet.core.Logging
{

    [Target("MailKitTarget")]
    public sealed class MailKitTarget : Target
    {
        public MailKitTarget()
        {
            if (Current.Configuration["Data:Smtp:Server"].HasValue() &&
                Current.Configuration["Data:Smtp:From"].HasValue() &&
                Current.Configuration["Data:Smtp:SubjectTemplate"].HasValue())
            {
                try
                {
                    client = new SmtpClient();
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                    client.Connect(Current.Configuration["Data:Smtp:Server"]);
                    IsConfigured = true;
                } catch
                {

                }
            }
        }

        private Layout generalLayout { get; set; }

        private MimeMessage Make(LogEventInfo logEvent)
        {
            var mailDeterminer = new MailDeterminerRenderer();
            var toAddresses = mailDeterminer.Render(logEvent);
            var message = new MimeMessage();
            foreach (var ta in toAddresses.Split(';'))
                message.To.Add(new MailboxAddress(ta));
            message.To.Add(new MailboxAddress(Current.Configuration["Data:Smtp:From"]));
            generalLayout = Current.Configuration["Data:Smtp:SubjectTemplate"];
            message.Subject = generalLayout.Render(logEvent);
            var mailLayout = new WhistlerMailRenderer();
            message.Body = new TextPart("plain")
            {
                Text = mailLayout.Render(logEvent)
            };
            return message;
        }

        private SmtpClient client;
        private SmtpClient connectedClient
        {
            get
            {
                if (!client.IsConnected)
                    client.Connect(Current.Configuration["Data:Smtp:Server"]);
                return client;
            }
        }

        private bool IsConfigured = false;
        
        protected override void Write(LogEventInfo logEvent)
        {
            if (IsConfigured)
                connectedClient.Send(Make(logEvent));
        }
    }
}
