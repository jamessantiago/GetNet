using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Net;
using getnet.Model;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace getnet
{
    public static class ExtensionMethods
    {
        public static bool IsLocal(this HttpRequest req)
        {
            var connection = req.HttpContext.Connection;
            if (connection.RemoteIpAddress != null)
            {
                if (connection.LocalIpAddress != null)
                {
                    return connection.RemoteIpAddress.Equals(connection.LocalIpAddress);
                }
                else
                {
                    return IPAddress.IsLoopback(connection.RemoteIpAddress);
                }
            }

            // for in memory TestServer or when dealing with default connection info
            if (connection.RemoteIpAddress == null && connection.LocalIpAddress == null)
            {
                return true;
            }

            return false;
        }

        public static List<SnackMessage> GetSnackMessages(this ISession session)
        {
            try
            {
                var messages = JsonConvert.DeserializeObject<List<SnackMessage>>(session.GetString("SnackMessages"));
                session.Remove("SnackMessages");
                return messages;

            } catch
            {
                return new List<SnackMessage>();
            }
        }

        public static void AddSnackMessage(this ISession session, string message, params string[] args)
        {
            var snack = new SnackMessage() { message = args != null ? string.Format(message, args) : message };
            session.AddSnackMessage(snack);
        }

        public static void AddSnackMessage(this ISession session, SnackMessage message)
        {
            var messages = session.GetSnackMessages();
            messages.Add(message);
            if (messages.Count > 1)
            {
                messages = PaginateSnacks(messages);
            }
            session.SetString("SnackMessages", JsonConvert.SerializeObject(messages));
        }

        private static List<SnackMessage> PaginateSnacks(List<SnackMessage> messages)
        {
            var pagedMessages = new List<SnackMessage>();
            for (int i = 0; i < messages.Count;i++)
            {
                var snack = messages[i];
                snack.message = Regex.Replace(snack.message, @" \(\d+/\d+\)", "");
                snack.message += string.Format(" ({0}/{1})", i + 1, messages.Count);
                pagedMessages.Add(snack);
            }
            return pagedMessages;
        }
        
    }
}
