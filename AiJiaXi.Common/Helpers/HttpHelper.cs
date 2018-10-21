using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Project.Common.Helpers
{
    public static class HttpHelper
    {
        public static string Get<T>(string url, T t)
            where T : class 
        {
            var client = new HttpClient();
            StringBuilder urlparams = new StringBuilder(url);
            Type type = typeof(T);
            List<PropertyInfo> properties = new List<PropertyInfo>();
            Array.ForEach(type.GetProperties(), p => properties.Add(p));

            urlparams.Append("?");
            foreach (var property in properties)
            {
                urlparams.AppendFormat("{0}={1}", property.Name, (string) property.GetValue(t, null));
                urlparams.Append("&");
            }

            string urlfull = urlparams.ToString();
            urlfull = urlfull.Substring(0, urlfull.LastIndexOf('&'));
            
            var result = client.GetStringAsync(new Uri(urlfull));

            return result.Result;
        }

        public static string Post<T>(string url, T t)
            where T : class 
        {
            Type type = typeof (T);
            List<PropertyInfo> properties = new List<PropertyInfo>();
            Array.ForEach(type.GetProperties(), p => properties.Add(p));

            HttpClient client = new HttpClient();
            var keyvalues = properties.Select(property => new KeyValuePair<string, string>(property.Name, (string) property.GetValue(t, null))).ToList();
            var result = client.PostAsync(new Uri(url), new FormUrlEncodedContent(keyvalues));

            var response_string = result.Result.Content.ReadAsStringAsync();

            return response_string.Result;
        }

        public static string PostJson<T>(string url, T t)
            where T : class
        {
            HttpClient client = new HttpClient();
            string json = JsonConvert.SerializeObject(t);
            var result = client.PostAsync(new Uri(url), new ByteArrayContent(Encoding.UTF8.GetBytes(json)));

            var response_string = result.Result.Content.ReadAsStringAsync();

            return response_string.Result;
        }

        public static async Task<string> PostAsync(string url, List<KeyValuePair<string, string>> formBody)
        {
            HttpClient client = new HttpClient();
            var result = await client.PostAsync(new Uri(url), new FormUrlEncodedContent(formBody));
            var task = await result.Content.ReadAsStringAsync();

            return task;
        }
    }
}