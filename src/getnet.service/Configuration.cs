using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Drawing;
using c = Colorful.Console;
using System.Text.RegularExpressions;
using System.Text;
using System.Globalization;
using System.Linq.Expressions;

namespace getnet.service
{
    public static class Configuration
    {
        public static void StartConfig()
        {
            c.WriteLine("You are now in edit mode.\n", Color.Red);
            c.WriteLine("Changes here will be persisted in appsettings.json.  Some settings can be modified directly in appsettings.json while others which required encryption must be done here.");

            string editResponse;
            do
            {
                c.Write("Edit datastore configuration?");
                c.Write(" [y/n]:", Color.LightYellow);
                editResponse = c.ReadLine();
            } while (!IsValidYesNo(editResponse));
            if (IsYes(editResponse))
            {
                string storeResponse = null;
                do
                {
                    if (storeResponse.HasValue()) c.WriteLine("Invalid selection", Color.Red);
                    c.Write("Choose the datastore type to use: ");
                    c.Write(" [mssql/postgres]:", Color.LightYellow);
                    storeResponse = c.ReadLine();
                } while (!Regex.IsMatch(storeResponse, "mssql|postgres", RegexOptions.IgnoreCase));
                storeResponse = storeResponse.Equals("mssql", StringComparison.CurrentCultureIgnoreCase) ? "MSSQL" : "Postgres";
                CoreCurrent.Configuration.Set("Data:DataStore", storeResponse);

                c.Write("Connection string: ");
                var conString = c.ReadLine().Trim();
                if (storeResponse == "Postgres")
                    CoreCurrent.Configuration.SetSecure("Data:PostgresConnectionString", conString);
                else
                    CoreCurrent.Configuration.SetSecure("Data:SqlServerConnectionString", conString);
            }
            
            do
            {
                c.Write("Edit SSH configuration?");
                c.Write(" [y/n]:", Color.LightYellow);
                editResponse = c.ReadLine();
            } while (!IsValidYesNo(editResponse));

            if (IsYes(editResponse))
            {
                c.Write("Username: ");
                CoreCurrent.Configuration.SetSecure("SSH:Username", c.ReadLine());
                c.Write("Username: ");
                CoreCurrent.Configuration.SetSecure("SSH:Password", ReadPassword('*'));
                c.Write("Port: ");
                CoreCurrent.Configuration.Set("SSH:Port", c.ReadLine());
            }

            //todo logging config
        }

        public static bool VerifyConfig()
        {
            try
            {
                List<bool> goods = new List<bool>();
                var store = Regex.Match(CoreCurrent.Configuration["Data:DataStore"], "MSSQL|Postgres");
                goods.Add(store.Success);
                if (store.Value == "MSSQL")
                    goods.Add(CoreCurrent.Configuration.GetSecure("Data:SqlServerConnectionString").HasValue());
                else if (store.Value == "Postgres")
                    goods.Add(CoreCurrent.Configuration.GetSecure("Data:PostgresConnectionString").HasValue());

                goods.Add(CoreCurrent.Configuration.GetSecure("SSH:Username").HasValue());
                goods.Add(CoreCurrent.Configuration.GetSecure("SSH:Password").HasValue());
                goods.Add(CoreCurrent.Configuration["SSH:Port"].HasValue());
                return goods.All(d => d);
            } catch
            {
                return false;
            }
        }

        private static bool IsYes(string response)
        {
            return Regex.IsMatch(response, @"y|yes", RegexOptions.IgnoreCase);
        }

        private static bool IsValidYesNo(string response)
        {
            if (!Regex.IsMatch(response, @"y|yes|n|no", RegexOptions.IgnoreCase))
            {
                c.WriteLine("Invalid response", Color.Red);
                return false;
            }
            else
            {
                return true;
            }
        }

        private static string ToTitle(string text)
        {
            return text[0].ToString().ToUpper() + text.Substring(1).ToLower();
        }

        public static string ReadPassword(char mask)
        {
            const int ENTER = 13, BACKSP = 8, CTRLBACKSP = 127;
            int[] FILTERED = { 0, 27, 9, 10  };

            var pass = new Stack<char>();
            char chr = (char)0;

            while ((chr = c.ReadKey(true).KeyChar) != ENTER)
            {
                if (chr == BACKSP)
                {
                    if (pass.Count > 0)
                    {
                        c.Write("\b \b");
                        pass.Pop();
                    }
                }
                else if (chr == CTRLBACKSP)
                {
                    while (pass.Count > 0)
                    {
                        c.Write("\b \b");
                        pass.Pop();
                    }
                }
                else if (FILTERED.Count(x => chr == x) > 0) { }
                else
                {
                    pass.Push((char)chr);
                    c.Write(mask);
                }
            }

            c.WriteLine();

            return new string(pass.Reverse().ToArray());
        }
    }
}
