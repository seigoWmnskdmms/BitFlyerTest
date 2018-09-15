using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Security.Cryptography;


namespace Util
{
    class Program
    {
        static readonly Uri endpointUri = new Uri("https://api.bitflyer.jp");
        static readonly string apiKey = "{{ YOUR API KEY }}";
        static readonly string apiSecret = "{{ YOUR API SECRET }}";

        static void Main(string[] args)
        {
            var task = ApiTestUsing();
            Thread.Sleep(50000);
        }

        public static void CreateDirectory()
        {
            var n = 50;
            var dirName = @"C:\Users\seigo\Desktop\Programing Test\BTCData";
            var startDate = new DateTime(2018, 8, 2);
            var count = 0;

            while (count < n)
            {
                Directory.CreateDirectory(Path.Combine(dirName, startDate.ToString("yyyyMMdd")));
                startDate = startDate.AddDays(1);
                count++;
            }
        }
        public static async Task ApiTestUsing()
        {
            var method = "POST";
            var path = "/v1/me/sendchildorder";
            var query = "";
            var body = @"";

            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage(new HttpMethod(method), path + query))
            using (var content = new StringContent(body))
            {
                client.BaseAddress = endpointUri;
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                request.Content = content;

                var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
                var data = timestamp + method + path + query + body;
                var hash = SignWithHMACSHA256(data, apiSecret);
                request.Headers.Add("ACCESS-KEY", apiKey);
                request.Headers.Add("ACCESS-TIMESTAMP", timestamp);
                request.Headers.Add("ACCESS-SIGN", hash);

                var message = await client.SendAsync(request);
                var response = await message.Content.ReadAsStringAsync();

                Console.WriteLine(response);

            }
        }
        static string SignWithHMACSHA256(string data, string secret)
        {
            using (var encoder = new HMACSHA256(Encoding.UTF8.GetBytes(secret)))
            {
                var hash = encoder.ComputeHash(Encoding.UTF8.GetBytes(data));
                return ToHexString(hash);
            }
        }

        static string ToHexString(byte[] bytes)
        {
            var sb = new StringBuilder(bytes.Length * 2);
            foreach (var b in bytes)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }
    }


}
