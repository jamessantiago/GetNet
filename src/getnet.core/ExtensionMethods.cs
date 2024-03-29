﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Data;
using System.Data.Common;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using System.Threading;
using System.Reflection;
using System.Security.Cryptography;
using System.Net;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Dynamic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using getnet.core.ssh;
using getnet.core.Model;
using getnet.core.Model.Entities;
using System.Text.RegularExpressions;
using System.Globalization;

namespace getnet
{
    public static class ExtensionMethods
    {
        public static bool IsNullOrEmpty(this string s)
        {
            return string.IsNullOrEmpty(s);
        }

        public static bool HasValue(this string s)
        {
            return !string.IsNullOrEmpty(s);
        }

        public static string IsNullOrEmptyReturn(this string s, params string[] otherPossibleResults)
        {
            if (s.HasValue())
                return s;

            if (otherPossibleResults == null)
                return "";

            foreach (var t in otherPossibleResults)
            {
                if (t.HasValue())
                    return t;
            }
            return "";
        }

        public static bool Contains(this string s, string value, bool ignoreCase)
        {
            if (!s.HasValue())
                return false;

            if (ignoreCase)
                return s.ToLower().Contains(value.ToLower());
            else
                return s.Contains(value);
        }

        public static long ToEpochTime(this DateTime dt, bool toMilliseconds = false)
        {
            var seconds = (long)(dt - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
            return toMilliseconds ? seconds * 1000 : seconds;
        }

        public static long? ToEpochTime(this DateTime? dt)
        {
            return
                dt.HasValue ?
                    (long?)ToEpochTime(dt.Value) :
                    null;
        }

        public static DateTime ToDateTime(this long epoch)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(epoch);
        }

        public static string ToRelativeTime(this DateTime dt, bool includeTime = true, bool asPlusMinus = false, DateTime? compareTo = null, bool includeSign = true)
        {
            var comp = (compareTo ?? DateTime.UtcNow);
            if (asPlusMinus)
            {
                return dt <= comp ? ToRelativeTimePastSimple(dt, comp, includeSign) : ToRelativeTimeFutureSimple(dt, comp, includeSign);
            }
            return dt <= comp ? ToRelativeTimePast(dt, comp, includeTime) : ToRelativeTimeFuture(dt, comp, includeTime);
        }
        public static string ToRelativeTime(this DateTime? dt, bool includeTime = true)
        {
            if (dt == null) return "";
            return ToRelativeTime(dt.Value, includeTime);
        }

        private static string ToRelativeTimePast(DateTime dt, DateTime utcNow, bool includeTime = true)
        {
            TimeSpan ts = utcNow - dt;
            double delta = ts.TotalSeconds;

            if (delta < 1)
            {
                return "just now";
            }
            if (delta < 60)
            {
                return ts.Seconds == 1 ? "1 sec ago" : ts.Seconds + " secs ago";
            }
            if (delta < 3600) // 60 mins * 60 sec
            {
                return ts.Minutes == 1 ? "1 min ago" : ts.Minutes + " mins ago";
            }
            if (delta < 86400)  // 24 hrs * 60 mins * 60 sec
            {
                return ts.Hours == 1 ? "1 hour ago" : ts.Hours + " hours ago";
            }

            var days = ts.Days;
            if (days == 1)
            {
                return "yesterday";
            }
            if (days <= 2)
            {
                return days + " days ago";
            }
            if (utcNow.Year == dt.Year)
            {
                return dt.ToString(includeTime ? "MMM %d 'at' %H:mmm" : "MMM %d");
            }
            return dt.ToString(includeTime ? @"MMM %d \'yy 'at' %H:mmm" : @"MMM %d \'yy");
        }

        private static string ToRelativeTimeFuture(DateTime dt, DateTime utcNow, bool includeTime = true)
        {
            TimeSpan ts = dt - utcNow;
            double delta = ts.TotalSeconds;

            if (delta < 1)
            {
                return "just now";
            }
            if (delta < 60)
            {
                return ts.Seconds == 1 ? "in 1 second" : "in " + ts.Seconds + " seconds";
            }
            if (delta < 3600) // 60 mins * 60 sec
            {
                return ts.Minutes == 1 ? "in 1 minute" : "in " + ts.Minutes + " minutes";
            }
            if (delta < 86400) // 24 hrs * 60 mins * 60 sec
            {
                return ts.Hours == 1 ? "in 1 hour" : "in " + ts.Hours + " hours";
            }

            // use our own rounding so we can round the correct direction for future
            var days = (int)Math.Round(ts.TotalDays, 0);
            if (days == 1)
            {
                return "tomorrow";
            }
            if (days <= 10)
            {
                return "in " + days + " day" + (days > 1 ? "s" : "");
            }
            // if the date is in the future enough to be in a different year, display the year
            if (utcNow.Year == dt.Year)
            {
                return "on " + dt.ToString(includeTime ? "MMM %d 'at' %H:mmm" : "MMM %d");
            }
            return "on " + dt.ToString(includeTime ? @"MMM %d \'yy 'at' %H:mmm" : @"MMM %d \'yy");
        }

        private static string ToRelativeTimePastSimple(DateTime dt, DateTime utcNow, bool includeSign)
        {
            TimeSpan ts = utcNow - dt;
            var sign = includeSign ? "-" : "";
            double delta = ts.TotalSeconds;
            if (delta < 1)
                return "< 1 sec";
            if (delta < 60)
                return sign + ts.Seconds + " sec" + (ts.Seconds == 1 ? "" : "s");
            if (delta < 3600) // 60 mins * 60 sec
                return sign + ts.Minutes + " min" + (ts.Minutes == 1 ? "" : "s");
            if (delta < 86400) // 24 hrs * 60 mins * 60 sec
                return sign + ts.Hours + " hour" + (ts.Hours == 1 ? "" : "s");
            return sign + ts.Days + " days";
        }

        private static string ToRelativeTimeFutureSimple(DateTime dt, DateTime utcNow, bool includeSign)
        {
            TimeSpan ts = dt - utcNow;
            double delta = ts.TotalSeconds;
            var sign = includeSign ? "+" : "";

            if (delta < 1)
                return "< 1 sec";
            if (delta < 60)
                return sign + ts.Seconds + " sec" + (ts.Seconds == 1 ? "" : "s");
            if (delta < 3600) // 60 mins * 60 sec
                return sign + ts.Minutes + " min" + (ts.Minutes == 1 ? "" : "s");
            if (delta < 86400) // 24 hrs * 60 mins * 60 sec
                return sign + ts.Hours + " hour" + (ts.Hours == 1 ? "" : "s");
            return sign + ts.Days + " days";
        }

        public static string ToTimeStringMini(this TimeSpan span, int maxElements = 2)
        {
            var sb = new StringBuilder();
            var elems = 0;
            Action<string, int> add = (s, i) =>
            {
                if (elems < maxElements && i > 0)
                {
                    sb.AppendFormat("{0:0}{1} ", i, s);
                    elems++;
                }
            };
            add("d", span.Days);
            add("h", span.Hours);
            add("m", span.Minutes);
            add("s", span.Seconds);
            add("ms", span.Milliseconds);

            if (sb.Length == 0) sb.Append("0");

            return sb.ToString().Trim();
        }

        public static string ToLocalTimeString(this DateTime date, string format = "G")
        {
            if (!CoreCurrent.Configuration["Data:Timezone"].HasValue())
                return date.ToLocalTime().ToString(format);

            var timezones = TimeZoneInfo.GetSystemTimeZones();
            foreach (var tz in timezones)
            {
                if (tz.DisplayName == CoreCurrent.Configuration["Data:LocalOffset"])
                    return TimeZoneInfo.ConvertTime(date, tz).ToString(format);
            }

            return date.ToLocalTime().ToString(format);
        }

        public static string ToHumanReadableSize(this long size)
        {
            return string.Format(new FileSizeFormatProvider(), "{0:fs}", size);
        }

        public static string ToComma(this int? number, string valueIfZero = null)
        {
            return number.HasValue ? ToComma(number.Value, valueIfZero) : "";
        }

        public static string ToComma(this int number, string valueIfZero = null)
        {
            if (number == 0 && valueIfZero != null) return valueIfZero;
            return $"{number:n0}";
        }

        public static string ToComma(this long? number, string valueIfZero = null)
        {
            return number.HasValue ? ToComma(number.Value, valueIfZero) : "";
        }

        public static string ToComma(this long number, string valueIfZero = null)
        {
            if (number == 0 && valueIfZero != null) return valueIfZero;
            return $"{number:n0}";
        }

        public static string ToComma(this double? number, string valueIfZero = null)
        {
            return number.HasValue ? ToComma(number.Value, valueIfZero) : "";
        }

        public static string ToComma(this double number, string valueIfZero = null)
        {
            if (number == 0 && valueIfZero != null) return valueIfZero;
            return $"{number:n0}";
        }

        public static T AddLoggedData<T>(this T ex, string key, string value) where T : Exception
        {
            ex.Data["ErrorLog-" + key] = value;
            return ex;
        }

        public static List<T> AsList<T>(this IEnumerable<T> source)
        {
            if (source != null && !(source is List<T>))
                return Enumerable.ToList<T>(source);
            return (List<T>)source;
        }

        public static async Task<List<T>> AsList<T>(this Task<IEnumerable<T>> source)
        {
            var result = await source;
            if (result != null && !(result is List<T>))
                return Enumerable.ToList<T>(result);
            return (List<T>)(result);
        }

        public static async Task<List<T>> AsList<T>(this ConfiguredTaskAwaitable<IEnumerable<T>> source)
        {
            var result = await source;
            if (result != null && !(result is List<T>))
                return Enumerable.ToList<T>(result);
            return (List<T>)(result);
        }

        public static async Task<IDisposable> EnsureOpenAsync(this DbConnection connection)
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));
            switch (connection.State)
            {
                case ConnectionState.Open:
                    return null;
                case ConnectionState.Closed:
                    await connection.OpenAsync().ConfigureAwait(false);
                    try
                    {
                        //await connection.SetReadUncommitted();
                        return new ConnectionCloser(connection);
                    }
                    catch
                    {
                        try { connection.Close(); }
                        catch { }
                        throw;
                    }

                default:
                    throw new InvalidOperationException("Cannot use EnsureOpen when connection is " + connection.State);
            }
        }

        private static readonly ConcurrentDictionary<int, string> _markedSql = new ConcurrentDictionary<int, string>();

        private static string MarkSqlString(string sql, string path, int lineNumber, string comment)
        {
            if (path.IsNullOrEmpty() || lineNumber == 0)
            {
                return sql;
            }

            int key = 17;
            unchecked
            {
                key = key * 23 + sql.GetHashCode();
                key = key * 23 + path.GetHashCode();
                key = key * 23 + lineNumber.GetHashCode();
                if (comment.HasValue()) key = key * 23 + comment.GetHashCode();
            }

            string output;
            if (_markedSql.TryGetValue(key, out output))
            {
                return output;
            }

            var commentWrap = " ";
            var i = sql.IndexOf(Environment.NewLine);

            if (i < 0 || i == sql.Length - 1)
            {
                i = sql.IndexOf(' ');
                commentWrap = Environment.NewLine;
            }

            if (i < 0) return sql;

            var split = path.LastIndexOf('\\') - 1;
            if (split < 0) return sql;
            split = path.LastIndexOf('\\', split);

            if (split < 0) return sql;

            split++;

            var sqlComment = " /* " + path.Substring(split) + "@" + lineNumber + (comment.HasValue() ? " - " + comment : "") + " */" + commentWrap;

            var ret =
                sql.Substring(0, i) +
                sqlComment +
                sql.Substring(i);

            _markedSql[key] = ret;

            return ret;
        }

        public static async Task<int> SetReadUncommitted(this DbConnection connection)
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED";
                await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
            return 1;
        }
        private class ConnectionCloser : IDisposable
        {
            DbConnection _connection;
            public ConnectionCloser(DbConnection connection)
            {
                _connection = connection;
            }
            public void Dispose()
            {
                var cn = _connection;
                _connection = null;
                if (cn != null)
                {
                    try { cn.Close(); }
                    catch { }
                }
            }
        }

        private static int totalGetSetSync, totalGetSetAsyncSuccess, totalGetSetAsyncError;

        public static Tuple<int, int, int> GetGetSetStatistics()
        {
            return Tuple.Create(Interlocked.CompareExchange(ref totalGetSetSync, 0, 0),
                Interlocked.CompareExchange(ref totalGetSetAsyncSuccess, 0, 0),
                Interlocked.CompareExchange(ref totalGetSetAsyncError, 0, 0));
        }

        public static string Pluralize(this int number, string item, bool includeNumber = true)
        {
            var numString = includeNumber ? number.ToComma() + " " : "";
            return number == 1
                       ? numString + item
                       : numString + (item.EndsWith("y") ? item.Remove(item.Length - 1) + "ies" : item + "s");
        }

        public static string Pluralize(this long number, string item, bool includeNumber = true)
        {
            var numString = includeNumber ? number.ToComma() + " " : "";
            return number == 1
                       ? numString + item
                       : numString + (item.EndsWith("y") ? item.Remove(item.Length - 1) + "ies" : item + "s");
        }

        public static string Pluralize(this int number, string single, string plural, bool includeNumber = true)
        {
            var numString = includeNumber ? number.ToComma() + " " : "";
            return number == 1 ? numString + single : numString + plural;
        }

        public static string Pluralize(this string noun, int number, string pluralForm = null)
        {
            return number == 1 ? noun : pluralForm.IsNullOrEmptyReturn((noun ?? "") + "s");
        }

        public static string Truncate(this string s, int maxLength)
        {
            if (s.IsNullOrEmpty()) return s;
            return (s.Length > maxLength) ? s.Remove(maxLength) : s;
        }

        public static string TruncateWithEllipsis(this string s, int maxLength)
        {
            if (s.IsNullOrEmpty()) return s;
            if (s.Length <= maxLength) return s;

            return $"{Truncate(s, Math.Max(maxLength, 3) - 3)}...";
        }

        public static string ReverseTruncate(this string s, int maxLength)
        {
            if (s.IsNullOrEmpty()) return s;
            return (s.Length > maxLength) ? s.Remove(0, s.Length - maxLength) : s;
        }

        public static string ReverseTruncateWithEllipsis(this string s, int maxLength)
        {
            if (s.IsNullOrEmpty()) return s;
            if (s.Length <= maxLength) return s;

            return $"...{ReverseTruncate(s, Math.Max(maxLength, 3) - 3)}";
        }

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source, Random rng)
        {
            T[] elements = source.ToArray();
            for (int i = elements.Length - 1; i >= 0; i--)
            {
                int swapIndex = rng.Next(i + 1);
                yield return elements[swapIndex];
                elements[swapIndex] = elements[i];
            }
        }

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
        {
            Random rng = new Random();
            return source.Shuffle(rng);
        }

        public static bool IsValidEmailAddress(this string email)
        {
            //System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$");
            //return regex.IsMatch(email);
            return email.Count(c => c == '@') == 1; //hey hey
        }

        public static uint IP2Int(this string IPAddress)
        {
            uint ip = 0;
            string[] elements = IPAddress.Split(new Char[] { '.' });
            if (elements.Length == 4)
            {
                ip = Convert.ToUInt32(elements[0]) << 24;
                ip += Convert.ToUInt32(elements[1]) << 16;
                ip += Convert.ToUInt32(elements[2]) << 8;
                ip += Convert.ToUInt32(elements[3]);
            }
            return ip;
        }

        public static long IpToInt(this string ip)
        {
            return IPAddress.Parse(ip).ToInt();
        }

        public static long ToInt(this IPAddress ip)
        {
            var address = ip.GetAddressBytes();
            if (!BitConverter.IsLittleEndian)
                Array.Reverse(address);
            return BitConverter.ToUInt32(address, 0);
        }

        public static string GetSecure(this IConfiguration config, string key)
        {
            //var cipherText = config[key];
            //if (!cipherText.HasValue())
            //    return string.Empty;
            //string description = string.Empty;
            //return DPAPI.Decrypt(cipherText, CoreCurrent.ENTROPY, out description);
            if (CoreCurrent.Protector == null)
                return "";
            return CoreCurrent.Protector.UnProtect(config[key]);
        }

        public static void SetSecure(this IConfiguration config, string key, string value)
        {
            //var secureText = DPAPI.Encrypt(DPAPI.KeyType.MachineKey, value, CoreCurrent.ENTROPY);
            var secureText = CoreCurrent.Protector.Protect(value);
            config.Set(key, secureText);
        }

        private static void CreateKey(string key)
        {
            string json = File.ReadAllText(CoreCurrent.ConfigFile);
            dynamic jsonObj = JsonConvert.DeserializeObject(json);
            var keypath = key.Split(':');

            switch (keypath.Count())
            {
                case 1:
                    jsonObj[keypath[0]] = new JRaw("{}");
                    break;
                case 2:
                    jsonObj[keypath[0]][keypath[1]] = new JRaw("{}");
                    break;
                case 3:
                    jsonObj[keypath[0]][keypath[1]][keypath[2]] = new JRaw("{}");
                    break;
                case 4:
                    jsonObj[keypath[0]][keypath[1]][keypath[2]][keypath[3]] = new JRaw("{}");
                    break;
                case 5:
                    jsonObj[keypath[0]][keypath[1]][keypath[2]][keypath[3]][keypath[4]] = new JRaw("{}");
                    break;
                default:
                    break;
            }
            string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(CoreCurrent.ConfigFile, output);
        }

        public static void VerifyKey(string key)
        {
            string json = File.ReadAllText(CoreCurrent.ConfigFile);
            dynamic jsonObj = JsonConvert.DeserializeObject(json);
            var keypath = key.Split(':');
            for (int i = 1; i < keypath.Count(); i++)
            {
                switch (i)
                {
                    case 1:
                        if (jsonObj[keypath[0]] == null)
                        {
                            CreateKey(string.Join(":", keypath.Take(i)));
                            jsonObj = JsonConvert.DeserializeObject(File.ReadAllText(CoreCurrent.ConfigFile));
                        }
                        break;
                    case 2:
                        if (jsonObj[keypath[0]][keypath[1]] == null)
                        {
                            CreateKey(string.Join(":", keypath.Take(i)));
                            jsonObj = JsonConvert.DeserializeObject(File.ReadAllText(CoreCurrent.ConfigFile));
                        }
                        break;
                    case 3:
                        if (jsonObj[keypath[0]][keypath[1]][keypath[2]] == null) { 
                            CreateKey(string.Join(":", keypath.Take(i)));
                            jsonObj = JsonConvert.DeserializeObject(File.ReadAllText(CoreCurrent.ConfigFile));
                        }
                        break;
                    case 4:
                        if (jsonObj[keypath[0]][keypath[1]][keypath[2]][keypath[3]] == null)
                        {
                            CreateKey(string.Join(":", keypath.Take(i)));
                            jsonObj = JsonConvert.DeserializeObject(File.ReadAllText(CoreCurrent.ConfigFile));
                        }
                        break;
                    case 5:
                        if (jsonObj[keypath[0]][keypath[1]][keypath[2]][keypath[3]][keypath[4]] == null)
                        {
                            CreateKey(string.Join(":", keypath.Take(i)));
                            jsonObj = JsonConvert.DeserializeObject(File.ReadAllText(CoreCurrent.ConfigFile));
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        public static void Set(this IConfiguration config, string key, string value)
        {
            VerifyKey(key);
            string json = File.ReadAllText(CoreCurrent.ConfigFile);
            dynamic jsonObj = JsonConvert.DeserializeObject(json);
            var keypath = key.Split(':');
            var current = (JObject)jsonObj;

            if (keypath.Count() > 5)
                throw new NotSupportedException("Key depth can only be 5");
            

            switch (keypath.Count())
            {
                case 1:
                    jsonObj[keypath[0]] = value;
                    break;
                case 2:
                    jsonObj[keypath[0]][keypath[1]] = value;
                    break;
                case 3:
                    jsonObj[keypath[0]][keypath[1]][keypath[2]] = value;
                    break;
                case 4:
                    jsonObj[keypath[0]][keypath[1]][keypath[2]][keypath[3]] = value;
                    break;
                case 5:
                    jsonObj[keypath[0]][keypath[1]][keypath[2]][keypath[3]][keypath[4]] = value;
                    break;
                default:
                    break;
            }
            string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(CoreCurrent.ConfigFile, output);
            config[key] = value;
        }

        public static bool AllSame(this IEnumerable<int> list)
        {
            bool first = true;
            int comparand = 0;
            foreach (int i in list)
            {
                if (first) comparand = i;
                else if (i != comparand) return false;
                first = false;
            }
            return true;
        }

        public static void AddOrNew<T>(this ICollection<T> entity, T item)
        {
            if (entity != null)
                entity.Add(item);
            else
                entity = new List<T> { item };
        }

        public static string Summary(this NetworkCapabilities caps)
        {
            List<string> capList = new List<string>();
            foreach (int flag in Enum.GetValues(typeof(NetworkCapabilities)))
            {
                if (((int)caps & flag) != 0)
                {
                    capList.Add(Enum.GetName(typeof(NetworkCapabilities), flag));
                }
            }
            return string.Join(", ", capList);
        }

        public static string Substring(this string value, string stopstring)
        {
            if (value.IndexOf(stopstring) == -1)
                return value;
            else
                return value.Substring(0, value.IndexOf(stopstring));
        }

        public static string CamelToSpace(this string value)
        {
            return Regex.Replace(value, "(\\B[A-Z])", " $1");
        }

        public static IPAddress DecrementIPbyOne(this IPAddress address)
        {
            IPAddress result;

            byte[] bytes = address.GetAddressBytes();

            for (int i = bytes.Length - 1; i >= 0; i--)
            {
                if (bytes[i] == byte.MaxValue)
                {
                    bytes[i] = 0;
                    continue;
                }
                bytes[i]--;

                result = new IPAddress(bytes);
                return result;
            }
            //failure
            return address;
        }

        public static IPAddress IncrementIPbyOne(this IPAddress address)
        {
            IPAddress result;

            byte[] bytes = address.GetAddressBytes();

            for (int i = bytes.Length - 1; i >= 0; i--)
            {
                if (bytes[i] == byte.MaxValue)
                {
                    bytes[i] = 0;
                    continue;
                }
                bytes[i]++;

                result = new IPAddress(bytes);
                return result;
            }
            //failure
            return address;
        }

        public static IPAddress IncrementIP(this IPAddress address, int numIPs)
        {

            for (int i = 0; i < numIPs; i++)
            {
                address = IncrementIPbyOne(address);
            }
            return address;
        }

        public static bool Like(this string value, string expression)
        {
            return value.ToLower().Contains(expression);
        }

        public static T[] SubArray<T>(this T[] data, int index, int length)
        {
            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }

    }

    
}
