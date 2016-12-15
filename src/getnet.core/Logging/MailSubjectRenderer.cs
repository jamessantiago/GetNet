using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Text;
using System.Xml;
using NLog;
using NLog.Config;
using NLog.LayoutRenderers;

namespace getnet.core.Logging
{
	[LayoutRenderer("mail_subject")]
	public class MailSubjectRenderer : LayoutRenderer
	{
		protected override void Append(StringBuilder builder, LogEventInfo logEvent)
		{
			builder.Append(logEvent.Message.Truncate(20));
		}
	}
}