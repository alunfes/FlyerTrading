using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PubnubApi;
using Utf8Json;

namespace FlyerTrading
{
    class API
    {
        private readonly Pubnub _pubnub;

        public string TickerFxBtcJpy = "lightning_ticker_" + "FX_BTC_JPY";
        public string boardFxBtcJpy = "lightning_board_FX_BTC_JPY";
        public string ExecutionsFxBtcJpy = "lightning_executions_FX_BTC_JPY";

        public API()
        {
            var config = new PNConfiguration();
            config.SubscribeKey = "sub-c-52a9ab50-291b-11e5-baaa-0619f8945a4f";
            config.PublishKey = "";
            _pubnub = new Pubnub(config);
        }

        public void unsubscribe()
        {
            _pubnub.Unsubscribe<string>()
     .Channels(new string[] {TickerFxBtcJpy,boardFxBtcJpy,ExecutionsFxBtcJpy})
     .Execute();
        }

        public void Subscribe<T>(string channel, Action<T> onReceive, Action<string> onConnect, Action<string, Exception> onError)
        {
            _pubnub.AddListener(new SubscribeCallbackExt(
                (pubnubObj, message) =>
                {
                    if (message != null)
                    {
                        string json = message.Message.ToString();

                        T deserialized;
                        try
                        {
                            deserialized = JsonSerializer.Deserialize<T>(json);
                            onReceive(deserialized);
                        }
                        catch (Exception ex)
                        {
                            onError(ex.Message, ex);
                            return;
                        }
                    }
                },
                (pubnubObj, presence) => { },
                (pubnubObj, status) =>
                {
                    if (status.Category == PNStatusCategory.PNUnexpectedDisconnectCategory)
                    {
                        onError("unexpected disconnect.", null);
                    }
                    else if (status.Category == PNStatusCategory.PNConnectedCategory)
                    {
                        onConnect("connected.");
                    }
                    else if (status.Category == PNStatusCategory.PNReconnectedCategory)
                    {
                        onError("reconnected.", null);
                    }
                    else if (status.Category == PNStatusCategory.PNDecryptionErrorCategory)
                    {
                        onError("messsage decryption error.", null);
                    }
                }
            ));


            _pubnub.Subscribe<string>()
                .Channels(new[] { channel })
                .Execute();

        }
    }
}
