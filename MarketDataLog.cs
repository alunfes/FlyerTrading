using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyerTrading
{
    class MarketDataLog
    {

        public static void initialize()
        {
            executions_log = new List<Executions>();
            execution_id_log = new List<string>();
            board_data_price_log = new List<double[]>();
            board_data_dt_log = new List<DateTime>();
            tick_log = new List<Tick>();
        }
        
        
        private static List<Executions> executions_log;
        private static List<string> execution_id_log;
        private static object lockobj_executuons = new object();

        public static void addExecutionsData(List<Executions> data)
        {
            lock (lockobj_executuons)
            {
                foreach (var v in data)
                {
                    executions_log.Add(v);
                    //execution_id_log.Add(v.buy_child_order_acceptance_id);
                    //execution_id_log.Add(v.sell_child_order_acceptance_id);
                }
                if (executions_log.Count > SystemSettings.max_log_index)
                {
                    executions_log.RemoveRange(0, 100000);
                    //execution_id_log.RemoveRange(0, 100000);
                }
            }
        }
        public static List<Executions> getExecutionsData()
        {
            lock (lockobj_executuons)
            {
                var res = new List<Executions>(executions_log);
                executions_log = new List<Executions>();
                return res;
            }
        }
        public static Executions getLastExecutionsData()
        {
            lock (lockobj_executuons)
            {
                return executions_log[executions_log.Count-1];
            }
        }
        public static List<Executions> getExecutionsDataRange(int last_ind)
        {
            lock (lockobj_executuons)
            {
                var res = new List<Executions>(executions_log.GetRange(last_ind, executions_log.Count-last_ind));
                return res;
            }
        }
        public static bool getExecutionStatus(string acceptance_id)
        {
            lock(lockobj_executuons)
            {
                if (execution_id_log.Contains(acceptance_id))
                    return true; //executed
                else
                    return false; //not executed
            }
        }
        public static int getNumExecutionsLog()
        {
            lock (lockobj_executuons)
                return executions_log.Count;
        }
        


        //private static Dictionary<DateTime, double[]> board_data_log; //double[bid,ask,spread]
        private static List<double[]> board_data_price_log;
        private static List<DateTime> board_data_dt_log;
        private static object lockobj_boarddata = new object();

        public static void addBoardData(DateTime dt, double[] d)
        {
            lock (lockobj_boarddata)
            {
                board_data_dt_log.Add(dt);
                board_data_price_log.Add(d);
            }
        }
        public static double[] getLatestBoardData()
        {
            lock (lockobj_boarddata)
            {
                if (board_data_price_log.Count > 0)
                    return board_data_price_log[board_data_price_log.Count-1];
                else
                    return new double[] { 0, 0, 0 };
            }
        }
        public static Dictionary<DateTime, double[]> getAllBoardData()
        {
            lock (lockobj_boarddata)
            {
                var dic = new Dictionary<DateTime, double[]>();
                for(int i=0; i<board_data_dt_log.Count; i++)
                {
                    dic.Add(board_data_dt_log[i], board_data_price_log[i]);
                }
                board_data_dt_log = new List<DateTime>();
                board_data_price_log = new List<double[]>();
                return dic;
            }
        }
        public static int getNumBoardData()
        {
            lock (lockobj_boarddata)
                return board_data_dt_log.Count; ;
        }

        private static List<Tick> tick_log;
        private static object lockobj_tick = new object();

        public static void addTickData(Tick data)
        {
            lock (lockobj_tick)
            {
                tick_log.Add(data);
                if (tick_log.Count > SystemSettings.max_log_index)
                    tick_log.RemoveRange(0, 100000);
            }
        }
        public static List<Tick> getTickData()
        {
            lock (lockobj_tick)
            {
                var res = tick_log.ToList();
                tick_log = new List<Tick>();
                return res;
            }
        }
        public static int getNumTickLog()
        {
            lock (lockobj_tick)
                return tick_log.Count;
        }

    }
}
