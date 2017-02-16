using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Text;
using System.Xml;
using NLog;
using NLog.Config;
using NLog.LayoutRenderers;
using getnet.core.Model;
using getnet.core.Model.Entities;

namespace getnet.core.Logging
{
	[LayoutRenderer("mail_determiner")]
	public class MailDeterminerRenderer : LayoutRenderer
	{
		//TODO Fix mail determiner
		protected override void Append(StringBuilder builder, LogEventInfo logEvent)
		{
            using (UnitOfWork uow = new UnitOfWork())
            {
                var predicates = PredicateBuilder.True<AlertRule>();

                int SiteId = 0;

                if ((logEvent.Properties.ContainsKey("SiteId") && int.TryParse(logEvent.Properties["SiteId"].ToString(), out SiteId)) && logEvent.Properties.ContainsKey("type"))
                {
                    predicates = predicates.And(d => (d.Type == logEvent.Properties["type"].ToString() || d.Type == "All") && d.Site != null && d.Site.SiteId == (int)logEvent.Properties["SiteId"]);
                    var net = uow.Repo<Site>().GetByID(SiteId);
                }
                else
                    predicates = predicates.And(d => d.Site == null);

                if (logEvent.Properties.ContainsKey("type"))
                    predicates = predicates.And(d => (d.Type == logEvent.Properties["type"].ToString() || d.Type == "All"));

                var alerts = uow.Repo<AlertRule>().Get(predicates);
                string finalemail = "";
                foreach (var alert in alerts)
                {

                    if (alert.LogLevel == AlertLogLevel.All || LogLevel.FromString(alert.LogLevel.ToString()) <= logEvent.Level)
                    {
                        string email = alert.User.Email;
                        if (email.IsValidEmailAddress() && !finalemail.Contains(email))
                            finalemail += email + ";";
                    }
                }
                finalemail += "!";
                builder.Append(finalemail.Replace(";!", ""));
            }
        }
	}
}