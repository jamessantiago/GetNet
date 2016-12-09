using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Text;
using System.Xml;
using NLog;
using NLog.Config;
using NLog.LayoutRenderers;

namespace getnet.logging
{
	[LayoutRenderer("whistler_mail")]
	public class WhistlerMailRenderer : LayoutRenderer
	{

		protected override void Append(StringBuilder builder, LogEventInfo logEvent)
		{
			//if (logEvent.Properties.ContainsKey("network"))
			//{
			//	using (NetAM.Models.NetworkRepository db = new Models.NetworkRepository())
			//	{
			//		builder.AppendLine("Network: " + db.FindOneNetwork((int)logEvent.Properties["network"]).Name);
			//		builder.AppendLine();
			//	}
			//}
			//builder.AppendLine(logEvent.Message);
			//if (logEvent.Properties.ContainsKey("details"))
			//{
			//	builder.AppendLine();
			//	builder.AppendLine("Details:");
			//	builder.AppendLine();
			//	builder.AppendLine(logEvent.Properties["details"].ToString());
			//}
			//if (logEvent.Level > LogLevel.Info)
			//{
			//	builder.AppendLine("Web Variables:");
			//	builder.AppendLine();
			//	var servervars = HttpContext.Current.Request.ServerVariables;
			//	builder.AppendLine("Source: " + servervars["REMOTE_ADDR"]);
			//	builder.AppendLine("Agent: " + servervars["HTTP_USER_AGENT"]);
			//	builder.AppendLine("User: " + servervars["AUTH_USER"]);
			//	builder.AppendLine("URL: " + HttpContext.Current.Request.Url.ToString());
			//	builder.AppendLine("Referer: " + servervars["HTTP_REFERER"]);
			//}
		}
	}
}