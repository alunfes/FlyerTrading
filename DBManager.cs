using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.IO;

namespace FlyerTrading
{
    class DBManager
    {
        private static object lockobj = new object();


        public static void createDB(string name)
        {
            SQLiteConnection.CreateFile(name);
        }

        public static void pushSql(string db_path, string sql)
        {
            lock (lockobj)
            {
                var sqlConnectionSb = new SQLiteConnectionStringBuilder { DataSource = db_path };
                SQLiteConnection cn = new SQLiteConnection(sqlConnectionSb.ToString());
                cn.Open();
                try
                {
                    using (var cmd = new SQLiteCommand(cn))
                    {
                        cmd.CommandText = sql;
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (SQLiteException exc)
                {

                    System.Diagnostics.Debug.WriteLine(exc.Message);
                }
                finally
                {
                    cn.Close();
                }
            }
        }

        
        public static void insertExecutions(List<Executions> data)
        {
            lock (lockobj)
            {
                var con = new SQLiteConnection("Data Source=" + SystemData.db_name);
                con.Open();
                try
                {
                    using (var cmd = new SQLiteCommand(con))
                    {
                        cmd.Transaction = con.BeginTransaction();
                        foreach(var v in data)
                        {
                            cmd.CommandText = "INSERT INTO MarketDataLogExecutions (id, side, price, size, exec_date, buy_child_order_acceptance_id, sell_child_order_acceptance_id) values('" +
                                v.id+"','"+v.side + "','" + v.price + "','" + v.size + "','" + v.exec_date.ToString("yyyy:MM:dd:HH:mm:ss:fff") + "','" + v.buy_child_order_acceptance_id + "','" + v.sell_child_order_acceptance_id+"')";
                            cmd.ExecuteNonQuery();
                        }
                        cmd.Transaction.Commit();
                    }
                }
                catch (SQLiteException exc)
                {

                    System.Diagnostics.Debug.WriteLine(exc.Message);
                }
                finally
                {
                    con.Close();
                }
            }
        }

        public static List<Executions> getAllExecutions()
        {
            lock (lockobj)
            {
                var con = new SQLiteConnection("Data Source=" + SystemData.db_name);
                con.Open();
                var res = new List<Executions>();
                string[] expectedFormats = { "yyyy:MM:dd:HH:mm:ss:fff" };
                try
                {
                    using (var cmd = new SQLiteCommand(con))
                    {

                        cmd.Transaction = con.BeginTransaction();
                        cmd.CommandText = "select * from MarketDataLogExecutions";
                        SQLiteDataReader sdr = cmd.ExecuteReader();
                        while (sdr.Read() == true)
                        {
                            var r = System.DateTime.ParseExact(sdr["exec_date"].ToString(), expectedFormats,
                            System.Globalization.DateTimeFormatInfo.InvariantInfo,
                            System.Globalization.DateTimeStyles.None);//

                            var exe = new Executions(); //id int, side text, price real, size real, exec_date text, buy_child_order_acceptance_id text, sell_child_order_acceptance_id text
                            exe.exec_date = r;
                            exe.id = (int)sdr["id"];
                            exe.side = (string)sdr["side"];
                            exe.price = (double)sdr["price"];
                            exe.size = (double)sdr["size"];
                            exe.buy_child_order_acceptance_id = (string)sdr["buy_child_order_acceptance_id"];
                            exe.sell_child_order_acceptance_id = (string)sdr["sell_child_order_acceptance_id"];
                            res.Add(exe);
                        }
                    }
                }
                catch (SQLiteException exc)
                {
                    System.Diagnostics.Debug.WriteLine(exc.Message);
                }
                finally
                {
                    con.Close();
                }
                return res;
            }
        }


        public static void insertBoardData(Dictionary<DateTime, double[]> data)
        {
            lock (lockobj)
            {
                var con = new SQLiteConnection("Data Source=" + SystemData.db_name);
                con.Open();
                try
                {
                    using (var cmd = new SQLiteCommand(con))
                    {
                        cmd.Transaction = con.BeginTransaction();
                        var p = data.Values.ToList();
                        var dt = data.Keys.ToList();
                        for(int i=0; i<p.Count; i++)
                        {
                            cmd.CommandText = "INSERT INTO Board(datetime,bid_price,ask_price,spread) values('" +
                                dt[i].ToString("yyyy:MM:dd:HH:mm:ss:fff") + "','" + p[i][1] + "','" + p[i][0] + "','" + p[i][2] + "')";
                            cmd.ExecuteNonQuery();
                        }
                        cmd.Transaction.Commit();
                    }
                }
                catch (SQLiteException exc)
                {

                    System.Diagnostics.Debug.WriteLine(exc.Message);
                }
                finally
                {
                    con.Close();
                }
            }
        }


        public static void insertBoardDiff(Dictionary<DateTime, double[]> data)
        {
            lock (lockobj)
            {
                var con = new SQLiteConnection("Data Source=" + SystemData.db_name);
                con.Open();
                try
                {
                    using (var cmd = new SQLiteCommand(con))
                    {
                        cmd.Transaction = con.BeginTransaction();
                        var p = data.Values.ToList();
                        var dt = data.Keys.ToList();
                        for (int i = 0; i < p.Count; i++)
                        {
                            cmd.CommandText = "INSERT INTO MarketDataLogBoardDiff(datetime,bid_price,ask_price,spread) values('" +
                                dt[i].ToString("yyyy:MM:dd:HH:mm:ss:fff") + "','" + p[i][1] + "','" + p[i][0] + "','" + p[i][2] + "')";
                            cmd.ExecuteNonQuery();
                        }
                        cmd.Transaction.Commit();
                    }
                }
                catch (SQLiteException exc)
                {
                    System.Diagnostics.Debug.WriteLine(exc.Message);
                }
                finally
                {
                    con.Close();
                }
            }
        }

        public static Dictionary<DateTime, double[]> getAllBoardData()
        {
            lock (lockobj)
            {
                var con = new SQLiteConnection("Data Source=" + SystemData.db_name);
                con.Open();
                var res = new Dictionary<DateTime, double[]>();
                string[] expectedFormats = { "yyyy:MM:dd:HH:mm:ss:fff" };
                try
                {
                    using (var cmd = new SQLiteCommand(con))
                    {
                        
                        cmd.Transaction = con.BeginTransaction();
                        cmd.CommandText = "select * from Board";
                        SQLiteDataReader sdr = cmd.ExecuteReader();
                        while (sdr.Read() == true)
                        {
                            var r = System.DateTime.ParseExact(sdr["datetime"].ToString(),expectedFormats,
                            System.Globalization.DateTimeFormatInfo.InvariantInfo,
                            System.Globalization.DateTimeStyles.None);//"2018,09,01,16,24,38"  "2018,09,01,16,24,39"
                            var b = (double)sdr["bid_price"];
                            var a = (double)sdr["ask_price"];
                            var s = (double)sdr["spread"];
                            res.Add(r, new double[] {b,a,s });   
                        }
                    }
                }
                catch (SQLiteException exc)
                {

                    System.Diagnostics.Debug.WriteLine(exc.Message);
                }
                finally
                {
                    con.Close();
                }
                return res;
            }
        }

        public static void createTables()
        {
            if (File.Exists(SystemData.db_name))
            {
                //MarketDataLog Executions
                pushSql(SystemData.db_name, "create table if not exists MarketDataLogExecutions(no INTEGER NOT NULL PRIMARY KEY,id int, side text, price real, size real, exec_date text, buy_child_order_acceptance_id text, sell_child_order_acceptance_id text)");
                pushSql(SystemData.db_name, "create table if not exists Board(no INTEGER NOT NULL PRIMARY KEY,datetime text,bid_price real,ask_price real,spread real)");
                pushSql(SystemData.db_name, "create table if not exists MarketDataLogBoardDiff(no INTEGER NOT NULL PRIMARY KEY,datetime text,bid_price real,ask_price real,spread real)");
            }
        }

    }
}
