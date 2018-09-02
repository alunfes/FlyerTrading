using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace FlyerTrading
{
    class ExchangeMonitor
    {
        private static double response_msec { get; set; }
        private static object lockobj = new object();

        private static void setResponseMsec(double d)
        {
            lock (lockobj)
                response_msec = d;
        }
        public static double getResponseMsec()
        {
            lock (lockobj)
                return response_msec;
        }

        public async static void startExchangeMonitor()
        {
            while (SystemFlg.getMarketDataFlg())
            {
                await Task.Run(async () =>
                {
                    if (FlyerAPI2.getApiAccessProhibition() == false)
                    {
                        Form1.Form1Instance.Invoke((Action)(() =>
                        {
                            
                        }));
                    }
                    await Task.Delay(10000);
                    return 0;
                });
            }
        }

        private async static Task<double> checkResponseMsec()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            sw.Stop();


            return 0;
        }

    }
}
