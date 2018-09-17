using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;

namespace FlyerTrading
{
    class DBWriter
    {
        public static void startDBWriter()
        {
            SystemFlg.setDBWriterFlg(true);
            initialize();
            dbWriterThread();
            //var th = new Thread(dbWriterThread);
            //th.Start();
        }

        private static void initialize()
        {
            if (File.Exists(SystemData.db_name) == false)
            {
                DBManager.createDB(SystemData.db_name);
                DBManager.createTables();
            }
        }

        private async static void dbWriterThread()
        {
            await Task.Run(async () =>
            {
                while (SystemFlg.getDBWriterFlg())
                {
                    writeExecutionsData();
                    writeBoardData();
                    await Task.Delay(500);
                }
            });
        }


        private static void writeExecutionsData()
        {
            if (MarketDataLog.getNumExecutionsLog() > 10000)
            {
                DBManager.insertExecutions(MarketDataLog.getExecutionsData());
            }
        }

        private static void writeBoardData()
        {
            if (MarketDataLog.getNumBoardData() > 100)
            {
                DBManager.insertBoardData(MarketDataLog.getAllBoardData());
            }
        }

    }
}
