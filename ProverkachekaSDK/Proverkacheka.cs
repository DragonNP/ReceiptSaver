using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ProverkachekaSDK
{
    public class Proverkacheka
    {
        private readonly string apiToken;
        private static HttpClient client = new HttpClient();
        private static string url = "https://proverkacheka.com/api/v1/check/get";

        public Proverkacheka(string apiToken)
        {
            this.apiToken = apiToken;
        }

        public Receipt Get(string qrRaw)
        {
            return Get(apiToken, qrRaw);
        }

        public static Receipt Get(string apiToken, string qrRaw)
        {
            return GetAsync(apiToken, qrRaw).GetAwaiter().GetResult();
        }

        public Task<Receipt> GetAsync(string qrRaw)
        {
            return _GetAsync(apiToken, qrRaw);
        }

        public static Task<Receipt> GetAsync(string apiToken, string qrRaw)
        {
            return _GetAsync(apiToken, qrRaw);
        }



        private static async Task<Receipt> _GetAsync(string apiToken, string qrRaw)
        {
            var values = new Dictionary<string, string>
            {
                { "token", apiToken},
                { "qrraw", qrRaw }
            };

            var content = new FormUrlEncodedContent(values);
            var response = await client.PostAsync(url, content);

            var json = JObject.Parse(await response.Content.ReadAsStringAsync());

            if (json["code"].ToString() != "1")
            {
                Console.WriteLine("Error");
                Console.WriteLine(json.ToString());
                return new Receipt();
            }

            JObject data = (JObject)json["data"]["json"];
            JArray items = (JArray)json["data"]["json"]["items"];

            List<Product> goods = GetGoods(items);
            return JsonToReceipt(data, goods);
        }

        private static List<Product> GetGoods(JArray items)
        {
            List<Product> goods = new List<Product>();

            for (int i = 0; i < items.Count; i++)
            {
                var product = items[0];
                goods.Add(new Product()
                {
                    Sum = Convert.ToInt32(product["sum"]),
                    Name = Convert.ToString(product["name"]),
                    Price = Convert.ToInt32(product["price"]),
                    Quantity = Convert.ToInt32(product["quantity"])
                });
            }

            return goods;
        }

        private static Receipt JsonToReceipt(JObject data, List<Product> goods)
        {
            Receipt receipt = new Receipt()
            {
                User = Convert.ToString(data["user"]),
                Address = Convert.ToString(data["metadata"]["address"]),
                OperationType = Convert.ToByte(data["operationType"]),
                NdsNo = Convert.ToInt32(data["ndsNo"]),
                Nds10 = Convert.ToInt32(data["nds"]),
                Nds20 = Convert.ToInt32(data["nds18"]),
                TotalSum = Convert.ToInt32(data["totalSum"]),
                CashTotalSum = Convert.ToInt32(data["cashTotalSum"]),
                EcashTotalSum = Convert.ToInt32(data["ecashTotalSum"]),
                TaxationType = Convert.ToByte(data["taxationType"]),
                Region = Convert.ToByte(data["region"]),
                Date = Convert.ToDateTime(data["dateTime"]),
                RetailPlace = Convert.ToString(data["retailPlace"]),
                Goods = goods
            };

            return receipt;

        }
    }
}
