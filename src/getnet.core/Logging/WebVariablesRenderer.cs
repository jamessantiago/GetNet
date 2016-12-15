using System;
using System.Collections.Generic;
using System.Linq;
//using System.Web;
using System.Globalization;
using System.Text;
using System.Xml;
using NLog;
using NLog.Config;
using NLog.LayoutRenderers;

namespace getnet.core.Logging
{
	[LayoutRenderer("web_variables")]
	public class WebVariablesRenderer : LayoutRenderer
	{
		public WebVariablesRenderer()
		{
			this.Format = "";
			this.Culture = CultureInfo.InvariantCulture;			
		}

		
		//protected override int GetEstimatedBufferSize(LogEventInfo ev)
		//{
		//    return 10000;
		//}

		protected string Format { get; set; }
		public CultureInfo Culture { get; set; }

		protected override void Append(StringBuilder builder, LogEventInfo logEvent)
		{
			if (logEvent.Level > LogLevel.Info)
			{
				//StringBuilder sb = new StringBuilder();
				//XmlWriter writer = XmlWriter.Create(sb);

				//writer.WriteStartElement("error");

				//writer.WriteStartElement("serverVariables");
				//foreach (string key in HttpContext.Current.Request.ServerVariables.AllKeys)
				//{
				//	writer.WriteStartElement("item");
				//	writer.WriteAttributeString("name", key);
				//	writer.WriteStartElement("value");
				//	writer.WriteAttributeString("string", HttpContext.Current.Request.ServerVariables[key].ToString());
				//	writer.WriteEndElement();
				//}
				//writer.WriteEndElement();
				//writer.WriteStartElement("cookies");
				//foreach (string key in HttpContext.Current.Request.Cookies.AllKeys)
				//{
				//	writer.WriteStartElement("item");
				//	writer.WriteAttributeString("name", key);
				//	writer.WriteStartElement("value");
				//	writer.WriteAttributeString("string", HttpContext.Current.Request.Cookies[key].ToString());
				//	writer.WriteEndElement();
				//}
				//writer.WriteEndElement();

				//writer.WriteEndElement();

				//writer.Flush();
				//writer.Close();
				//string xml = sb.ToString();
				//builder.Append(xml);
			}
		}
	}
}