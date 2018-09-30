using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace FlyerTrading
{
    class Log
    {
        private static object lockobj = new object();
        private static List<string> keys { get; set; }
        private static List<string> values { get; set; }
        private static List<DateTime> dt { get; set; }
        private static int num_log { get; set; }

        public static void addLog(string key, string v)
        {
            lock (lockobj)
            {
                keys.Add(num_log.ToString()+"-"+ key);
                values.Add(v);
                dt.Add(DateTime.Now);
                num_log++;
            }
        }

        public static void initialize()
        {
            keys = new List<string>();
            values = new List<string>();
            dt = new List<DateTime>();
            num_log = 0;
        }

        public static void writeLog()
        {
            using (StreamWriter sw = new StreamWriter("./all log.csv", false, Encoding.Default))
            {
                sw.WriteLine("datetime,log key,log data");
                for(int i=0; i< num_log; i++)
                {
                    sw.WriteLine(dt[i].ToString("yyyy:MM:dd:HH:mm:ss:fff") + "," + keys[i] + "," + values[i]);
                }
            }
        }
    }
}
