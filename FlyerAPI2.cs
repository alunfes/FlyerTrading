using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Net.Http.Headers;
using System.Net.Http;
using Newtonsoft.Json;
using System.Security.Cryptography;

namespace FlyerTrading
{
    class FlyerAPI2
    {
        static readonly Uri endpointUri = new Uri("https://api.bitflyer.jp");
        private static object lockobj_num_public_called = new object();
        private static object lockobj_num_private_called = new object();
        private static long num_public_called;
        private static long num_private_called;
        private static List<long> num_public_minit;
        private static List<long> num_private_minit;

        private static bool api_access_prohibition;
        private static object lockobj_monitor = new object();
        public static bool getApiAccessProhibition()
        {
            lock (lockobj_monitor)
                return api_access_prohibition;
        }
        private static void setApiAccessProhibition(bool flg)
        {
            lock (lockobj_monitor)
                api_access_prohibition = flg;
        }
        public static async Task<string> startFlyerAPIMonitoring()
        {
            setApiAccessProhibition(false);
            num_private_minit = new List<long>();
            num_public_minit = new List<long>();
            int num = 0;

            while (SystemFlg.getMasterFlg())
            {
                await Task.Run(async () =>
                {
                    num_public_minit.Add(num_public_called);
                    num_private_minit.Add(num_private_called);

                    if (num_public_minit.Count > 60 && num_private_minit.Count > 60)
                    {
                        if ((num_private_called - num_private_minit[0]) > 150 || (num_public_called + num_private_called - num_public_minit[0] - num_private_minit[0]) > 450)
                        {
                            setApiAccessProhibition(true);
                            //Form1.Form1Instance.Invoke((Action)(() => { Form1.Form1Instance.setLabel7("API Access Prohibited"); }));
                        }
                        else
                            setApiAccessProhibition(false);
                        num_private_minit.RemoveAt(0);
                        num_public_minit.RemoveAt(0);
                    }
                    Form1.Form1Instance.Invoke((Action)(() =>
                    {
                        Form1.Form1Instance.setLabel8("num private api acecss in a min=" + (num_private_called - num_private_minit[0]).ToString());
                        Form1.Form1Instance.setLabel9("num public api acecss in a min=" + (num_public_called - num_public_minit[0]).ToString());
                    }));
                    await Task.Delay(1000);
                    return 0;
                });
            }
            return "completed";
        }

        public static void addNumPublicCalled()
        {
            lock (lockobj_num_public_called)
                num_public_called++;
        }
        public static void addNumPrivateCalled()
        {
            lock (lockobj_num_private_called)
                num_private_called++;
        }
        public static long getNumPublicCalled()
        {
            lock (lockobj_num_public_called)
                return num_public_called;
        }
        public static long getNumPrivateCalled()
        {
            lock (lockobj_num_private_called)
                return num_private_called;
        }

        public static async Task<BoardData> getBoardAsync(string q)
        {
            var method = "GET";
            var path = "/v1/getboard";
            var query = "?product_code=" + q;

            var board = JsonConvert.DeserializeObject<BoardData>(await getFuncAsync(method, path, query));
            addNumPublicCalled();
            return board;
        }


        public static async Task<List<BalanceData>> getBalanceAsync()
        {
            var method = "GET";
            var path = "/v1/me/getbalance";
            var query = "";

            var balance = JsonConvert.DeserializeObject<List<BalanceData>>(await getFuncAsync(method, path, query));
            addNumPrivateCalled();
            return balance;
        }

        public static async Task<CollateralData> getCollateralAsync()
        {
            var method = "GET";
            var path = "/v1/me/getcollateral";
            var query = "";

            var res = JsonConvert.DeserializeObject<CollateralData>(await getFuncAsync(method, path, query));
            addNumPrivateCalled();
            return res;
        }

        public static async Task<List<ParentOrderData>> getParentOrderAsync()
        {
            var method = "GET";
            var path = "/v1/me/getparentorders";
            var query = "?product_code=FX_BTC_JPY";

            var res = JsonConvert.DeserializeObject<List<ParentOrderData>>(await getFuncAsync(method, path, query));
            addNumPrivateCalled();
            return res;
        }

        public static async Task<List<ChildOrderData>> getChildOrderAsync(string state)
        {
            var method = "GET";
            var path = "/v1/me/getchildorders";
            var constate = (state != "") ? "&child_order_state=" + state : "";
            var query = "?product_code=FX_BTC_JPY"+constate;

            var res = JsonConvert.DeserializeObject<List<ChildOrderData>>(await getFuncAsync(method, path, query));
            addNumPrivateCalled();
            return res;
        }


        public static async Task<List<PositionData>> getPositionsAsync()
        {
            var method = "GET";
            var path = "/v1/me/getpositions";
            var query = "?product_code=FX_BTC_JPY";


            var res = JsonConvert.DeserializeObject<List<PositionData>>(await getFuncAsync(method, path, query));
            addNumPrivateCalled();
            return res;
        }

        public static async Task<List<ExecutionData>> getExecutionsAsync()
        {
            var method = "GET";
            var path = "/v1/me/getexecutions";
            var query = "?product_code=FX_BTC_JPY";


            var res = JsonConvert.DeserializeObject<List<ExecutionData>>(await getFuncAsync(method, path, query));
            addNumPublicCalled();
            return res;
        }

        public static async Task<List<ExecutionData>> getExecutionsAcceptanceIDAsync(string acceptance_id)
        {
            var method = "GET";
            var path = "/v1/me/getexecutions";
            var id = "?child_order_acceptance_id="+acceptance_id;
            var query = "?product_code=FX_BTC_JPY"+id;


            var res = JsonConvert.DeserializeObject<List<ExecutionData>>(await getFuncAsync(method, path, query));
            if (res == null)
                res = new List<ExecutionData>();
            addNumPublicCalled();
            return res;
        }



        public static async Task<string> cancelAllChildOrdersAsync()
        {
            var method = "POST";
            var path = "/v1/me/cancelallchildorders";
            var query = "";
            var body = "{\"product_code\" : \"FX_BTC_JPY\"}";

            var res = await postFuncAsync(method, path, query, body);
            addNumPrivateCalled();
            return res;
        }

        public static async Task<string> cancelChildOrdersAsync(string order_id)
        {
            var method = "POST";
            var path = "/v1/me/cancelchildorder";
            var query = "";
            var body = "{\"product_code\" : \"FX_BTC_JPY\",\"child_order_acceptance_id\" : \""+order_id+"\"}";

            var res = await postFuncAsync(method, path, query, body);
            addNumPrivateCalled();
            return res;
        }

        public static async Task<OrderData> sendChiledOrderAsync(string side, double price, double size, int minutes_to_expire)
        {
            var method = "POST";
            var path = "/v1/me/sendchildorder";
            var query = "";
            //var body = @"";

            var body = new SendOrderData();
            body.product_code =  "FX_BTC_JPY";
            body.child_order_type = "LIMIT";
            body.side = side;
            body.price = price;
            body.size = size;
            body.time_in_force = "GTC";
            body.minute_to_expire = minutes_to_expire;

            string jsonBody = (body == null) ? "" : JsonConvert.SerializeObject(body);

            var res = JsonConvert.DeserializeObject<OrderData>(await postFuncAsync(method, path, query, jsonBody));
            if (res == null)
                res = new OrderData();
            addNumPrivateCalled();
            return res;
        }

        private static async Task<string> getFuncAsync(string method, string path, string query)
        {
            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage(new HttpMethod(method), path + query))
            {
                string response="";
                try
                {
                    client.BaseAddress = endpointUri;
                    client.Timeout = TimeSpan.FromMilliseconds(3000);

                    var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
                    var data = timestamp + method + path + query;
                    var hash = SignWithHMACSHA256(data, SystemData.secret_key);
                    request.Headers.Add("ACCESS-KEY", SystemData.api_key);
                    request.Headers.Add("ACCESS-TIMESTAMP", timestamp);
                    request.Headers.Add("ACCESS-SIGN", hash);

                    var message = await client.SendAsync(request);
                    response = await message.Content.ReadAsStringAsync();
                }
                catch(TaskCanceledException e)
                {
                    System.Diagnostics.Debug.WriteLine("FlyerAPI2 - "+e.Message);
                }
                return response;
            }
        }

        private static async Task<string> postFuncAsync(string method, string path, string query, string body)
        {
            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage(new HttpMethod(method), path + query))
            using (var content = new StringContent(body))
            {
                string response = "";
                try
                {
                    client.Timeout = TimeSpan.FromMilliseconds(10000);
                    client.BaseAddress = endpointUri;
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    request.Content = content;

                    var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
                    var data = timestamp + method + path + query + body;
                    var hash = SignWithHMACSHA256(data, SystemData.secret_key);
                    request.Headers.Add("ACCESS-KEY", SystemData.api_key);
                    request.Headers.Add("ACCESS-TIMESTAMP", timestamp);
                    request.Headers.Add("ACCESS-SIGN", hash);

                    var message = await client.SendAsync(request);
                    response = await message.Content.ReadAsStringAsync();
                }
                catch (TaskCanceledException e)
                {
                    System.Diagnostics.Debug.WriteLine("FlyerAPI2 - " + e.Message);
                    response = "error";
                }

                return response;
            }
        }



        private static string SignWithHMACSHA256(string data, string secret)
        {
            using (var encoder = new HMACSHA256(Encoding.UTF8.GetBytes(secret)))
            {
                var hash = encoder.ComputeHash(Encoding.UTF8.GetBytes(data));
                return ToHexString(hash);
            }
        }

        private static string ToHexString(byte[] bytes)
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
