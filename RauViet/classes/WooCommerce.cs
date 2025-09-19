using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RauViet.classes
{
    public class Product
    {
        public int id { get; set; }
        public string name { get; set; }
        public string permalink { get; set; }
        public string price { get; set; }
    }

    public class WooCommerceApi
    {
        private readonly string baseUrl = "https://vietrau.com//wp-json/wc/v3/";
        private readonly string consumerKey = "ck_cdd210a6633b65e9fe8d29a13659d62192d0c91c";
        private readonly string consumerSecret = "cs_fe959a73532699a9e6d2714c4c9144d27e10ec54";

        public async Task<List<Product>> GetProducts()
        {
            using (var client = new HttpClient())
            {
                var byteArray = Encoding.ASCII.GetBytes($"{consumerKey}:{consumerSecret}");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                HttpResponseMessage response = await client.GetAsync(baseUrl + "products");
                response.EnsureSuccessStatusCode();

                string result = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<Product>>(result);
            }
        }
    }
}
