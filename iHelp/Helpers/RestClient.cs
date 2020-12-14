using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using iHelp.Models;
using Newtonsoft.Json;

namespace iHelp.Helpers
{
    public class RestClient
    {
        public static string Token { private get; set; }

        private readonly HttpClient _client;

        public static Uri Uri = new Uri("http://192.168.43.206:5005/api/");

        public RestClient()
        {
            _client = new HttpClient();
            _client.BaseAddress = Uri;
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Token);
        }

        public async Task<ResponseModel> GetAsync(string url)
        {
            var response = await _client.GetAsync(url);
            var json = response.StatusCode == HttpStatusCode.OK ? await response.Content.ReadAsStringAsync() : null;

            return new ResponseModel { Code = response.StatusCode, Body = json };
        }

        public async Task<ResponseModel> DeleteAsync(string url)
        {
            var response = await _client.DeleteAsync(url);
            var responseModel = response.StatusCode == HttpStatusCode.OK ? await response.Content.ReadAsStringAsync() : null;
            return new ResponseModel { Code = response.StatusCode, Body = responseModel };
        }

        public async Task<ResponseModel> PostAsync(string url, object model)
        {
            var json = JsonConvert.SerializeObject(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync(url, content);

            var responseModel = response.StatusCode == HttpStatusCode.OK ? await response.Content.ReadAsStringAsync() : null;

            return new ResponseModel { Code = response.StatusCode, Body = responseModel };
        }
    }
}
