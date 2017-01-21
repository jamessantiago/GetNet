using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            Console.WriteLine("You are now in edit mode.\n");
            Console.WriteLine("Changes here will be persisted in appsettings.json.  Some settings can be modified directly in appsettings.json while others which required encryption must be done here.");

            string editResponse;
            do
            {
                Console.Write("Edit datastore configuration?");
                Console.Write(" [y/n]:");
                editResponse = Console.ReadLine();
            } while (!IsValidYesNo(editResponse));
            if (IsYes(editResponse))
            {
                string storeResponse = null;
                do
                {
                    if (storeResponse.HasValue()) Console.WriteLine("Invalid selection");
                    Console.Write("Choose the datastore type to use: ");
                    Console.Write(" [mssql/postgres]:");
                    storeResponse = Console.ReadLine();
                } while (!Regex.IsMatch(storeResponse, "mssql|postgres", RegexOptions.IgnoreCase));
                storeResponse = storeResponse.Equals("mssql", StringComparison.CurrentCultureIgnoreCase) ? "MSSQL" : "Postgres";
                CoreCurrent.Configuration.Set("Data:DataStore", storeResponse);

                Console.Write("Connection string: ");
                var conString = Console.ReadLine().Trim();
                if (storeResponse == "Postgres")
                    CoreCurrent.Configuration.SetSecure("Data:PostgresConnectionString", conString);
                else
                    CoreCurrent.Configuration.SetSecure("Data:SqlServerConnectionString", conString);
            }
            
            do
            {
                Console.Write("Edit SSH configuration?");
                Console.Write(" [y/n]:");
                editResponse = Console.ReadLine();
            } while (!IsValidYesNo(editResponse));

            if (IsYes(editResponse))
            {
                Console.Write("Username: ");
                CoreCurrent.Configuration.SetSecure("SSH:Username", Console.ReadLine());
                Console.Write("Password: ");
                //ReadPassword('*')
                CoreCurrent.Configuration.SetSecure("SSH:Password", Console.ReadLine());
                Console.Write("Port: ");
                CoreCurrent.Configuration.Set("SSH:Port", Console.ReadLine());
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
                Console.WriteLine("Invalid response");
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

            while ((chr = Console.ReadKey(true).KeyChar) != ENTER)
            {
                if (chr == BACKSP)
                {
                    if (pass.Count > 0)
                    {
                        Console.Write("\b \b");
                        pass.Pop();
                    }
                }
                else if (chr == CTRLBACKSP)
                {
                    while (pass.Count > 0)
                    {
                        Console.Write("\b \b");
                        pass.Pop();
                    }
                }
                else if (FILTERED.Count(x => chr == x) > 0) { }
                else
                {
                    pass.Push((char)chr);
                    Console.Write(mask);
                }
            }

            Console.WriteLine();

            return new string(pass.Reverse().ToArray());
        }
    }
}
