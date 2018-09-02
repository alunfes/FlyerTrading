using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.Security.Cryptography;

namespace FlyerTrading
{
    class FlyerAPI
    {
        /// <summary>bitFlyer取引所のAPIを実行します。
        /// </summary>
        /// <param name="http">取引所と通信する HttpClient。</param>
        /// <param name="path">APIの通信URL（取引所サイトからの相対）。</param>
        /// <param name="apiKey">APIキー。</param>
        /// <param name="secret">秘密キー。</param>
        /// <param name="method">APIメソッド。"GET"か"POST"を指定します。</param>
        /// <param name="query">URLクエリ。</param>
        /// <param name="body">APIメソッドが"POST"のとき指定するリクエストボディ。</param>
        /// <returns>レスポンスとして返されるJSON形式の文字列。</returns>
        internal async Task<string> Send(HttpClient http, Uri path, string apiKey, string secret, string method, Dictionary<string, string> query = null, object body = null)
        {
            // 相対URLにクエリパラメータを追加
            if (query != null && query.Any())
            {
                var content = new FormUrlEncodedContent(query);
                string q = await content.ReadAsStringAsync();

                path = new Uri(path.ToString() + "?" + q, UriKind.Relative);
            }

            // リクエスト時のUNIXタイムスタンプ
            string timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();

            //リクエストボディをJSON化
            string jsonBody = body == null ? "" : JsonConvert.SerializeObject(body);

            // POSTするメッセージを作成
            string message = timestamp + method + path.ToString() + jsonBody;

            // メッセージをHMACSHA256で署名
            byte[] hash = new HMACSHA256(Encoding.UTF8.GetBytes(secret)).ComputeHash(Encoding.UTF8.GetBytes(message));
            string sign = BitConverter.ToString(hash).ToLower().Replace("-", "");//バイト配列をを16進文字列へ

            // HTTPヘッダをセット
            http.DefaultRequestHeaders.Clear();
            http.DefaultRequestHeaders.Add("ACCESS-KEY", apiKey);
            http.DefaultRequestHeaders.Add("ACCESS-TIMESTAMP", timestamp);
            http.DefaultRequestHeaders.Add("ACCESS-SIGN", sign);

            // 送信
            HttpResponseMessage res;
            if (method == "POST")
            {
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                res = await http.PostAsync(path, content);
            }
            else if (method == "GET")
            {
                res = await http.GetAsync(path);
            }
            else
            {
                throw new ArgumentException("method は POST か GET を指定してください。", method);
            }

            //返答内容を取得
            string text = await res.Content.ReadAsStringAsync();

            //通信上の失敗
            if (!res.IsSuccessStatusCode)
                return "";

            return text;
        }
    }
}
