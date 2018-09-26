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
        private static List<PositionData> current_positions { get; set; }
        private static List<ChildOrderData> current_orders { get; set; }

        private static Dictionary<int, string> trade_log { get; set; }
        private static int num_log { get; set; }
        private static object lockob_log = new object();
        private static void addLog(string log)
        {
            lock (lockob_log)
            {
                trade_log.Add(num_log, log);
                num_log++;
                if (num_log > 199999999)
                {
                    num_log = 0;
                    trade_log = new Dictionary<int, string>();
                }
            }
        }
        private static Dictionary<int, string> getAllLog()
        {
            lock (lockob_log)
                return trade_log;
        }


        private static double entry_spread;
        private static double order_size;

        private static List<string> current_order_ids { get; set; }




        public async static Task<string> startMMBot(double entry_spread_width, double size)
        {
            string res = "";

            SystemFlg.setMMFlg(true);

            initialize();
            entry_spread = entry_spread_width;
            order_size = size;
            var ac = new Account();

            await Task.Delay(3000);


            await Task.Run(async () =>
            {
                while (SystemFlg.getMMFlg())
                {
                    if (FlyerAPI2.getApiAccessProhibition() == false)
                    {
                        await MMStrategy(ac);
                        
                        string line2 = "";
                        var ord = ac.getAllOrders();
                        for (int i = 0; i < ord.Count; i++)
                            line2 += ord[i].order_side + " - " + ord[i].order_lot + "@" + ord[i].order_price + ", ";

                        //ac.takeLog("orders:"+line2);
                        //ac.takeLog("holdings:"+ ac.holding_ave_side + ", hold ave price=" + ac.holding_ave_price + " x " + ac.holding_total_size);

                        Form1.Form1Instance.Invoke((Action)(() =>
                        {
                            Form1.Form1Instance.setLabel5(ac.holding_ave_side+ ", hold ave price="+ac.holding_ave_price + " x " +ac.holding_total_size);
                            Form1.Form1Instance.setLabel4("num trade=" + ac.num_trade);
                            Form1.Form1Instance.setLabel6(line2);
                        }));
                    }
                    else
                    {
                        Form1.Form1Instance.Invoke((Action)(() => { Form1.Form1Instance.setLabel4("api prohibited"); }));
                        await Task.Delay(1000);
                    }
                }
            });

            Form1.Form1Instance.Invoke((Action)(() => { Form1.Form1Instance.addListBox2("Finishing MMBot"); }));
            await ac.cancelAllOrders();
            await ac.checkExecutionAndUpdateOrders();
            await ac.startExitPriceTracingOrder();

            Form1.Form1Instance.Invoke((Action)(() => { Form1.Form1Instance.addListBox2("Finished MMBot"); }));

            //ac.displayAllLog();
            ac.writeLog();

            return res;
        }

        private static void initialize()
        {
            current_positions = new List<PositionData>();
            trade_log = new Dictionary<int, string>();
            current_order_ids = new List<string>();
            current_orders = new List<ChildOrderData>();
            num_log = 0;
            entry_spread = 999999;
        }

        private static async Task<string> MMStrategy(Account ac)
        {
            string res = "";

            var board = BoardDataUpdate.getCurrentBoard();
            //if (board.Asks.Length > 0)
            {
                var bids = board.Bids.Select(x => x.Price).ToList();
                var asks = board.Asks.Select(x => x.Price).ToList();

                double bid_max = bids.Max();
                double ask_min = asks.Min();

                await ac.checkExecutionAndUpdateOrders();
                var ord = ac.getAllOrders();
                if (ac.holding_total_size== 0 && ord.Count == 0) //no positions, no orders
                {
                    if (board.spread >= entry_spread)
                    {
                        //entry for both of bid and ask
                        var sell_order = await ac.entry(ask_min - 1, order_size, "SELL");
                        if (sell_order.order_id != "")
                            Form1.Form1Instance.Invoke((Action)(() => { Form1.Form1Instance.addListBox2("ordered sell @" + (ask_min - 1)); }));
                        var buy_order = await ac.entry(bid_max + 1, order_size, "BUY");
                        if (buy_order.order_id != "")
                            Form1.Form1Instance.Invoke((Action)(() => { Form1.Form1Instance.addListBox2("ordered buy @" + (bid_max + 1)); }));

                        //check execution and start exit price tracing when executed
                        bool flg = true;
                        do
                        {
                            await ac.checkExecutionAndUpdateOrders();
                            if (ac.holding_ave_side != "")
                                await ac.startExitPriceTracingOrder();
                            else if (BoardDataUpdate.getCurrentBoard().spread < entry_spread)
                            {
                                await ac.cancelAllOrders();
                                flg = false;
                            }
                        } while (flg);
                    }
                }
                else if (ac.holding_total_size == 0 && ord.Count > 0) //no positions but some orders
                {
                    if (board.spread < entry_spread)
                    {
                        var res_cancel = await ac.cancelAllOrders();
                        if(res_cancel!="error")
                        {
                            //ac.takeLog("cancelling all orders");
                            //Form1.Form1Instance.Invoke((Action)(() => { Form1.Form1Instance.addListBox2("cancelling all orders"); }));
                        }
                    }
                    else
                    {
                        for (int i = 0; i < ord.Count; i++)
                        {
                            if (ord[i].order_side == "BUY")
                            {
                                if (ord[i].order_price <= bid_max)
                                {
                                    double size = ord[i].order_lot;
                                    Form1.Form1Instance.Invoke((Action)(() => { Form1.Form1Instance.addListBox2("cancelling buy order, id=" + ord[i].order_id); }));
                                    var res_cancel = await ac.cancelOrder(ord[i].order_id);
                                    if (res_cancel != "error")
                                    {
                                        Form1.Form1Instance.Invoke((Action)(() => { Form1.Form1Instance.addListBox2("ordered buy @" + (bid_max + 1)); }));
                                        await ac.entry(bid_max + 1, size, "BUY");
                                    }
                                }
                            }
                            else
                            {
                                if (ord[i].order_price >= ask_min)
                                {
                                    double size = ord[i].order_lot;
                                    Form1.Form1Instance.Invoke((Action)(() => { Form1.Form1Instance.addListBox2("cancelling sell order, id=" + ord[i].order_id); }));
                                    var res_cancel = await ac.cancelOrder(ord[i].order_id);
                                    if (res_cancel != "error")
                                    {
                                        Form1.Form1Instance.Invoke((Action)(() => { Form1.Form1Instance.addListBox2("ordered sell @" + (ask_min - 1)); }));
                                        await ac.entry(ask_min - 1, size, "SELL");
                                    }
                                }
                            }
                        }
                    }
                }
                else if (ac.holding_total_size > 0)//holding positions, and orders
                {
                    var com = await ac.startExitPriceTracingOrder();
                    res = "completed exit price tracing order";
                }
            }
            return res;
        }
        
        

        class TradeDecisionData
        {
            string position { get; set; }
            double ask_price { get; set; }
            double bid_price { get; set; }
            double ask_lot { get; set; }
            double bid_lot { get; set; }
        }
    }
}
