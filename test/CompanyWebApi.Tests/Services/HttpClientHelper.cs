using Newtonsoft.Json;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System;

namespace CompanyWebApi.Tests.Services
{
    /// <summary>
    /// HttpClient helper class
    /// </summary>
    public class HttpClientHelper
    {
        public HttpClientHelper(HttpClient httpHttpClient)
        {
            Client = httpHttpClient;
        }

        public HttpClient Client { get; }

        public async Task<HttpStatusCode> DeleteAsync(string path)
        {
            var response = await Client.DeleteAsync(path).ConfigureAwait(false);
            return response.StatusCode;
        }

        public async Task<T> GetAsync<T>(string path)
        {
            var response = await Client.GetAsync(path).ConfigureAwait(false);
            return await GetContentAsync<T>(response);
        }

        public async Task<T> PostAsync<T>(string path, T content)
        {
            return await PostAsync<T, T>(path, content).ConfigureAwait(false);
        }

        public async Task<TOut> PostAsync<TIn, TOut>(string path, TIn content)
        {
            if (content == null)
            {
                throw new ApplicationException("content is null");
            }
            var jsonString = JsonConvert.SerializeObject(content);
            var stringContent = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var response = await Client.PostAsync(path, stringContent).ConfigureAwait(false);
            return await GetContentAsync<TOut>(response);
        }

        public async Task<T> PutAsync<T>(string path, T content)
        {
            return await PutAsync<T, T>(path, content).ConfigureAwait(false);
        }

        public async Task<TOut> PutAsync<TIn, TOut>(string path, TIn content)
        {
            if (content == null)
            {
                throw new ApplicationException("content is null");
            }
            var jsonString = JsonConvert.SerializeObject(content);
            var stringContent = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var response = await Client.PutAsync(path, stringContent).ConfigureAwait(false);
            return await GetContentAsync<TOut>(response);
        }

        private static async Task<T> GetContentAsync<T>(HttpResponseMessage response)
        {
            response.EnsureSuccessStatusCode();
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                NullValueHandling = NullValueHandling.Ignore
            };
            var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonConvert.DeserializeObject<T>(responseString, settings);
        }
    }
}
