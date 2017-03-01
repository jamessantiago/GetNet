using MailKit.Net.Smtp;
using MimeKit;
using NLog;
using NLog.Layouts;
using NLog.Targets;
using System.Threading.Tasks;

namespace getnet.core.Logging
{
    [Target("MailKitTarget")]
    public sealed class MailKitTarget : Target
    {
        private SmtpClient _client;
        private bool? _enabled;
        private SmtpClient Client => _client ?? (_client = LoadClient());
        private bool Enabled => _enabled ?? IsEnabled();
        private Layout GeneralLayout { get; set; }

        protected override void Write(LogEventInfo logEvent)
        {
            if (Enabled)
                Task.Run(() =>
                {
                    try
                    {
                        Client.SendAsync(Make(logEvent).Result);
                    }
                    catch
                    {
                        // ignored
                    }
                });
        }

        private static SmtpClient LoadClient()
        {
            if (CoreCurrent.Configuration["Whistler:Smtp:Enabled"] != "true") return new SmtpClient();

            try
            {
                var newclient = new SmtpClient();
                newclient.ServerCertificateValidationCallback = (s, c, h, e) => true;
                return newclient;
            }
            catch
            {
                // ignored
            }
            return new SmtpClient();
        }

        private bool IsEnabled()
        {
            _enabled = CoreCurrent.Configuration["Whistler:Smtp:Enabled"] == "true";
            return _enabled.Value;
        }

        private async Task<MimeMessage> Make(LogEventInfo logEvent)
        {
            if (!Client.IsConnected)
                await Client.ConnectAsync(CoreCurrent.Configuration["Whistler:Smtp:Server"]);
            var mailDeterminer = new MailDeterminerRenderer();
            var toAddresses = mailDeterminer.Render(logEvent);
            var message = new MimeMessage();
            foreach (var ta in toAddresses.Split(';'))
                message.To.Add(new MailboxAddress(ta));
            message.From.Add(new MailboxAddress(CoreCurrent.Configuration["Whistler:Smtp:From"]));
            GeneralLayout = CoreCurrent.Configuration["Whistler:Smtp:SubjectLayout"];
            message.Subject = GeneralLayout.Render(logEvent);
            var mailLayout = new WhistlerMailRenderer();
            message.Body = new TextPart("plain")
            {
                Text = mailLayout.Render(logEvent)
            };
            return message;
        }
    }
}