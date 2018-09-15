using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyerTrading
{
    class SystemFlg
    {
        public static void initialize()
        {
            marketdata_flg = false;
            dbwriter_flg = false;
            mm_flg = false;
        }

        private static object lock_master = new object();
        private static bool master_flg;
        public static void setMasterFlg(bool flg)
        {
            lock (lock_master)
                master_flg = flg;
        }
        public static bool getMasterFlg()
        {
            lock (lock_master)
                return master_flg;
        }

        private static object lock_md = new object();
        private static bool marketdata_flg;
        public static void setMarketDataFlg(bool flg)
        {
            lock (lock_md)
                marketdata_flg = flg;
        }
        public static bool getMarketDataFlg()
        {
            lock (lock_md)
                return marketdata_flg;
        }
        

        private static object lock_dw = new object();
        private static bool dbwriter_flg;
        public static void setDBWriterFlg(bool flg)
        {
            lock (lock_dw)
                dbwriter_flg = flg;
        }
        public static bool getDBWriterFlg()
        {
            lock (lock_dw)
                return dbwriter_flg;
        }

        private static object lock_mm = new object();
        private static bool mm_flg;
        public static void setMMFlg(bool flg)
        {
            lock (lock_mm)
                mm_flg = flg;
        }
        public static bool getMMFlg()
        {
            lock (lock_mm)
                return mm_flg;
        }

    }
}
