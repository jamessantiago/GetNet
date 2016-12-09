using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Globalization;
using System.Text;
using System.Xml;
using NLog;
using NLog.Config;
using NLog.LayoutRenderers;

namespace getnet.logging
{
	[LayoutRenderer("mail_determiner")]
	public class MailDeterminerRenderer : LayoutRenderer
	{
		
		protected override void Append(StringBuilder builder, LogEventInfo logEvent)
		{
			//using (NetworkDataContext db = new NetworkDataContext())
			//{
			//	var predicates = PredicateBuilder.True<AlertRule>();

			//	int networkId = 0;

			//	if ((logEvent.Properties.ContainsKey("network") && int.TryParse(logEvent.Properties["network"].ToString(), out networkId)) && logEvent.Properties.ContainsKey("type"))
			//	{
			//		predicates = predicates.And(d => (d.Type == logEvent.Properties["type"].ToString() || d.Type == "All") && d.NetworkId == (int)logEvent.Properties["network"]);
			//		var net = db.Networks.FirstOrDefault(d => d.networkID == networkId);
			//	}
			//	else
			//		predicates = predicates.And(d => d.NetworkId == null);

			//	if (logEvent.Properties.ContainsKey("type") && !logEvent.Properties.ContainsKey("directory"))
			//		predicates = predicates.And(d => (d.Type == logEvent.Properties["type"].ToString() || d.Type == "All"));

			//	var alerts = db.AlertRules.Where(predicates);
			//	string finalemail = "";
			//	foreach (var alert in alerts)
			//	{

			//		if (alert.Level == "All" || LogLevel.FromString(alert.Level) <= logEvent.Level)
			//		{
			//			string email = db.Profiles.First(d => d.ProfileId == alert.ProfileId).Email;
			//			if (email.IsValidEmailAddress() && !finalemail.Contains(email))
			//				finalemail += email + ";";
			//		}
			//	}
			//	finalemail += "!";
			//	builder.Append(finalemail.Replace(";!", ""));
			//}
		}
	}
}