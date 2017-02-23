using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace getnet.Client
{
    public class ServiceClient
    {
        private HttpClient client;
        public Exception CreationError;
        public bool IsErrored => CreationError != null;

        public ServiceClient(ApiKeyType type)
        {
            try
            {
                var httpclient = new HttpClient();
                httpclient.BaseAddress = new Uri(CoreCurrent.Configuration["Data:GetNetService:Url"]);
                httpclient.DefaultRequestHeaders.Accept.Clear();
                httpclient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                switch (type)
                {
                    case ApiKeyType.Default:
                        httpclient.DefaultRequestHeaders.Add("PRIVATE_TOKEN", CoreCurrent.Configuration.GetSecure("Api:Keys:Default"));
                        break;
                    case ApiKeyType.Read:
                        httpclient.DefaultRequestHeaders.Add("PRIVATE_TOKEN", CoreCurrent.Configuration.GetSecure("Api:Keys:Read"));
                        break;
                    case ApiKeyType.Admin:
                        httpclient.DefaultRequestHeaders.Add("PRIVATE_TOKEN", CoreCurrent.Configuration.GetSecure("Api:Keys:Admin"));
                        break;
                    default:
                        break;
                }
                client = httpclient;
            } catch (Exception ex)
            {
                CreationError = ex;
            }
        }

        public bool IsOnline()
        {
            try
            {
                var response = client.GetAsync("/").Result;
                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    return result == "GetNet Service";
                }
                else
                {
                    return false;
                }
            }catch
            {
                return false;
            }
        }

        public bool IsConfigured()
        {
            return CoreCurrent.Configuration.GetSecure("Api:Keys:Admin").HasValue() && CoreCurrent.Configuration["Data:GetNetService:Url"].HasValue();
        }

        public string SetKeys(string AdminKey, string ReadKey)
        {
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("AdminKey", AdminKey),
                new KeyValuePair<string, string>("ReadKey", ReadKey)
            });
            var response = client.PostAsync("/setup/setkeys", content).Result;
            if (response.IsSuccessStatusCode)
            {
                var results = JsonConvert.DeserializeAnonymousType(response.Content.ReadAsStringAsync().Result, new { Status = "", Message = "" });
                if (results.Status == "Success")
                {
                    return results.Message;
                } else
                {
                    throw new Exception(results.Message);
                }
            } else
            {
                throw new Exception("HTTP error code: " + response.StatusCode);
            }
        }

        public IEnumerable<Tuple<string,string>> GetJobs()
        {
            HttpResponseMessage response = null;
            try
            {
                response = client.GetAsync("/scheduler/triggers").Result;
            }
            catch {
                yield break;
            }

            if (response.IsSuccessStatusCode)
            {
                var results = JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result);
                foreach (var result in (JArray)results)
                {
                    yield return new Tuple<string,string>(result["name"].Value<string>(), result["cronExpressionString"].Value<string>());
                }
            }
            else
            {
                yield break;
            }
        }

        public void RunJob(string jobname)
        {
            try
            {
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("Name", jobname)
                });
                client.PostAsync("/scheduler/run", content);
            } catch
            {

            }
        }

        public dynamic GetLogging()
        {
            HttpResponseMessage response = client.GetAsync("/logging/").Result;
            if (response.IsSuccessStatusCode)
            {
                var results = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                return results;
            }
            else
            {
                throw new Exception("HTTP error code: " + response.StatusCode);
            }
        }

        public void SetLogging(IFormCollection collection)
        {
            var content = new FormUrlEncodedContent(collection.Select(d => new KeyValuePair<string, string>(d.Key, d.Value)));
            client.PostAsync("/logging/set", content);
        }


    }

    public enum ApiKeyType
    {
        Default,
        Read,
        Admin
    }
}
