using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nancy;
using Quartz.Impl.Matchers;
using System.Threading;
using Quartz;
using Nancy.ModelBinding;
using Nancy.Authentication.Stateless;
using Nancy.Responses;

namespace getnet.service.Api
{
    public class LoggingModule : NancyModule
    {
        public LoggingModule() : base("/logging")
        {
            StatelessAuthentication.Enable(this, Current.StatelessConfig);
            Before += ctx => (Context.CurrentUser == null) ? new HtmlResponse(HttpStatusCode.Unauthorized) : null;

            Get("/", args => GetLogging());
            Post("/set", args =>
            {
                Logging logging = this.Bind<Logging>();
                ConfigureLogging(logging);
                return string.Empty;
            });
        }

        public void ConfigureLogging(Logging logging)
        {
            CoreCurrent.Configuration.Set("Whistler:File:Enabled", logging.fileenabled == "on" ? "true" : "false");
            CoreCurrent.Configuration.Set("Whistler:File:Layout", logging.filelayout);
            CoreCurrent.Configuration.Set("Whistler:File:FileName", logging.filename);
            CoreCurrent.Configuration.Set("Whistler:Smtp:Enabled", logging.smtpenabled == "on" ? "true" : "false");
            CoreCurrent.Configuration.Set("Whistler:Smtp:Server", logging.smtpserver);
            CoreCurrent.Configuration.Set("Whistler:Smtp:From", logging.smtpfrom);
            CoreCurrent.Configuration.Set("Whistler:Smtp:SubjectLayout", logging.smtpsubject);
            CoreCurrent.Configuration.Set("Whistler:Db:Enabled", logging.databaseenabled == "on" ? "true" : "false");
        }

        public dynamic GetLogging()
        {
            return new
            {
                FileEnabled = CoreCurrent.Configuration["Whistler:File:Enabled"],
                FileLayout = CoreCurrent.Configuration["Whistler:File:Layout"],
                FileName = CoreCurrent.Configuration["Whistler:File:FileName"],
                SmtpEnabled = CoreCurrent.Configuration["Whistler:Smtp:Enabled"],
                SmtpServer = CoreCurrent.Configuration["Whistler:Smtp:Server"],
                SmtpFrom = CoreCurrent.Configuration["Whistler:Smtp:From"],
                SmtpSubjectLayout = CoreCurrent.Configuration["Whistler:Smtp:SubjectLayout"],
                DbEnabled = CoreCurrent.Configuration["Whistler:Db:Enabled"]
            };
        }
    }
}
