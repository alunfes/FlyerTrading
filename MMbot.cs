using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyerTrading
{
    class MMbot
    {
        private static object lockob = new object();


        public async static void startMMBot()
        {
            SystemFlg.setMMFlg(true);
            while (SystemFlg.getMMFlg())
            {
                await Task.Run(async () =>
                {
                    var price = MarketDataLog.getLatestBoardData();
                    if (price[2] > 100)
                    {
                        if (FlyerAPI2.getApiAccessProhibition() == false)
                        {
                            //FlyerAPI2.sendChiledOrderAsync()
                        }
                    }
                    //await Task.Delay(10000);
                    return 0;
                });
            }
        }
    }
}
