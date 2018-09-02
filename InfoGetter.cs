using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

namespace FlyerTrading
{
    class InfoGetter
    {
        public string getBoard()
        {
            //bitFlyer取引所にアクセスする HttpClient
            HttpClient http = new HttpClient();
            http.BaseAddress = new Uri("https://api.bitflyer.jp");

            //URLのクエリパラメータ
            var query = new Dictionary<string, string>
{
    { "product_code", "BTC_JPY" },//通貨の種類
};

            //板情報を取得
  //          Uri path = new Uri("/v1/board", UriKind.Relative);//APIの通信URL

            Uri path = new Uri("/v1/me/getbalance", UriKind.Relative);//APIの通信URL
            string method = "GET";//APIメソッド
            var fp = new FlyerAPI();
            var json = fp.Send(http, path, SystemData.api_key, SystemData.secret_key, method, query);
            return json.Result;
        }
    }
}
