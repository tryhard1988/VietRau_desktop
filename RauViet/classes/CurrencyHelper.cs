using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

public class CurrencyHelper
{
    public static async Task<decimal> GetCHFtoVND_FreeAsync()
    {
        using (HttpClient client = new HttpClient())
        {
            string url = "https://open.er-api.com/v6/latest/CHF";

            HttpResponseMessage response = await client.GetAsync(url);
            string json = await response.Content.ReadAsStringAsync();
            Console.WriteLine(json);

            if (!response.IsSuccessStatusCode)
                throw new Exception($"HTTP {response.StatusCode}: {json}");

            JObject obj = JObject.Parse(json);
            if (obj["rates"]?["VND"] != null && decimal.TryParse(obj["rates"]["VND"].ToString(), out decimal rate))
                return rate;

            throw new Exception("Không tìm thấy tỷ giá VND trong dữ liệu trả về.");
        }
    }
    public static async Task<decimal> GetVNDtoCHF_FreeAsync()
    {
        using (HttpClient client = new HttpClient())
        {
            string url = "https://open.er-api.com/v6/latest/VND";
            HttpResponseMessage response = await client.GetAsync(url);
            string json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"HTTP {response.StatusCode}: {json}");

            JObject obj = JObject.Parse(json);
            var rateToken = obj["rates"]?["CHF"];

            if (rateToken == null)
                throw new Exception("Không tìm thấy tỷ giá CHF trong dữ liệu trả về.");

            decimal rate = rateToken.Value<decimal>();
            Console.WriteLine($"1 VND = {rate} CHF");
            Console.WriteLine($"1 CHF = {1 / rate} VND");

            return rate;
        }
    }

    public static async Task<decimal> GetUSDtoCHF_FreeAsync()
    {
        using (HttpClient client = new HttpClient())
        {
            string url = "https://open.er-api.com/v6/latest/USD";
            HttpResponseMessage response = await client.GetAsync(url);
            string json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"HTTP {response.StatusCode}: {json}");

            JObject obj = JObject.Parse(json);
            var rateToken = obj["rates"]?["CHF"];

            if (rateToken == null)
                throw new Exception("Không tìm thấy tỷ giá CHF trong dữ liệu trả về.");

            decimal rate = rateToken.Value<decimal>();
            Console.WriteLine($"1 USD = {rate} CHF");
            Console.WriteLine($"1 CHF = {1 / rate} USD");

            return rate;
        }
    }
}
