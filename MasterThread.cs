using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

namespace FlyerTrading
{
    class MasterThread
    {
        private static DateTime start_dt;
        private static object lockobj_timeelapsed = new object();

        public static TimeSpan getTotalTimeElapsed()
        {
            lock (lockobj_timeelapsed)
                return (DateTime.Now - start_dt);
        }

        public async static void startMasterThread()
        {
            SystemFlg.setMasterFlg(true);
            start_dt = new DateTime();
            start_dt = DateTime.Now;

            while (SystemFlg.getMasterFlg())
            {
                await Task.Run(async () =>
                {
                    Form1.Form1Instance.Invoke((Action)(() =>
                    {
                        Form1.Form1Instance.setLabel10("system time elapsed (min)=" + Math.Round((DateTime.Now - start_dt).TotalMinutes, 2));
                    }));
                    await Task.Delay(100);
                    return 0;
                });
            }
        }


        public static void finishMasterThread()
        {
            SystemFlg.setMMFlg(false);
            SystemFlg.setMarketDataFlg(false);
            SystemFlg.setMasterFlg(false);
            SystemFlg.setDBWriterFlg(false);
            Form1.Form1Instance.setLabel12("Finished master thread");
        }
    }
}
