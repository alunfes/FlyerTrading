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




        public async static void startMMBot(double entry_spread_width, double size)
        {
            SystemFlg.setMMFlg(true);

            initialize();
            entry_spread = entry_spread_width;
            order_size = size;
            var ac = new Account();
            

            await Task.Delay(3000);
            while (SystemFlg.getMMFlg())
            {
                await Task.Run(async () =>
                {
                    if (FlyerAPI2.getApiAccessProhibition() == false)
                    {
                        await MMStrategy(ac);

                        string line = "";
                        for (int i = 0; i < ac.holding_acceptance_id.Count; i++)
                            line += ac.holding_side[i] + ":" + ac.holding_size[i] + "@" + ac.holding_price[i] + ", ";
                        Form1.Form1Instance.Invoke((Action)(() => 
                        {
                            Form1.Form1Instance.setLabel5(line);
                        }));
                    }
                    else
                    {
                        Form1.Form1Instance.Invoke((Action)(() => { Form1.Form1Instance.setLabel4("api prohibited"); }));
                        await Task.Delay(1000);
                    }
                    return 0;
                });
            }

            Form1.Form1Instance.addListBox2("Finishing MMBot");
            await ac.cancelAllOrders();
            await ac.checkExecutionAndUpdateOrders();
            await ac.startExitPriceTracingOrder();

            Form1.Form1Instance.addListBox2("Finished MMBot");
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
            if (board.Asks.Length > 0)
            {
                var bids = board.Bids.Select(x => x.Price).ToList();
                var asks = board.Asks.Select(x => x.Price).ToList();

                double bid_max = bids.Max();
                double ask_min = asks.Min();

                await ac.checkExecutionAndUpdateOrders();
                if (ac.holding_acceptance_id.Count == 0 && ac.order_id.Count == 0) //no positions, no orders
                {
                    if (board.spread >= entry_spread)
                    {
                        var sell_order = await ac.entry(ask_min - 1, order_size, "SELL");
                        Form1.Form1Instance.Invoke((Action)(() => { Form1.Form1Instance.addListBox2("ordered sell @" + (ask_min - 1)); }));
                        var buy_order = await ac.entry(bid_max + 1, order_size, "BUY");
                        Form1.Form1Instance.Invoke((Action)(() => { Form1.Form1Instance.addListBox2("ordered buy @" + bid_max + 1); }));

                    }
                    else if (current_positions.Count == 0 && current_orders.Count > 0) //no positions but some orders
                    {
                        if (board.spread < entry_spread)
                        {
                            await ac.cancelAllOrders();
                        }
                        else
                        {
                            for (int i = 0; i < ac.order_dt.Count; i++)
                            {
                                if (ac.order_side[i] == "BUY")
                                {
                                    if (ac.order_price[i] <= bid_max)
                                    {
                                        Form1.Form1Instance.Invoke((Action)(() => { Form1.Form1Instance.addListBox2("cancelled buy order, id=" + ac.order_id[i]); }));
                                        await ac.cancelOrder(i);
                                        Form1.Form1Instance.Invoke((Action)(() => { Form1.Form1Instance.addListBox2("ordered buy @" + (bid_max + 1)); }));
                                        await FlyerAPI2.sendChiledOrderAsync("BUY", bid_max + 1, ac.holding_size[i], 1);
                                    }
                                }
                                else
                                {
                                    if (ac.order_price[i] >= ask_min)
                                    {
                                        Form1.Form1Instance.Invoke((Action)(() => { Form1.Form1Instance.addListBox2("cancelled sell order, id=" + ac.order_id[i]); }));
                                        await FlyerAPI2.cancelChildOrdersAsync(ac.order_id[i]);
                                        Form1.Form1Instance.Invoke((Action)(() => { Form1.Form1Instance.addListBox2("ordered sell @" + (ask_min - 1)); }));
                                        await FlyerAPI2.sendChiledOrderAsync("SELL", ask_min - 1, order_size, 1);
                                        res = "";
                                    }
                                }
                            }
                        }
                    }
                    else if(ac.holding_acceptance_id.Count > 0)//holding positions, and orders
                    {
                        var com = await ac.startExitPriceTracingOrder();
                        res = "completed exit price tracing order";
                    }
                }
            }
            return res;
        }

        private static async Task<string> MMStrategy()
        {
            string res = "";
            current_positions = await FlyerAPI2.getPositionsAsync();
            current_orders = await FlyerAPI2.getChildOrderAsync("ACTIVE");
            var board = BoardDataUpdate.getCurrentBoard();
            var bids = board.Bids.Select(x => x.Price).ToList();
            var asks = board.Asks.Select(x => x.Price).ToList();

            double bid_max = bids.Max();
            double ask_min = asks.Min();

            if (current_positions.Count == 0 && current_orders.Count == 0) //no positions, no orders
            {
                if (board.spread >= entry_spread)
                {
                    var res_ask = await FlyerAPI2.sendChiledOrderAsync("SELL", ask_min - 1, order_size, 1);
                    //current_order_ids.Add(res_ask.order_id);
                    Form1.Form1Instance.addListBox2("ordered sell @" + (ask_min - 1));
                    var res_bid = await FlyerAPI2.sendChiledOrderAsync("BUY", bid_max + 1, order_size, 1);
                    //current_order_ids.Add(res_bid.order_id);
                    Form1.Form1Instance.addListBox2("ordered buy @" + bid_max + 1);
                }
            }
            else if (current_positions.Count == 0 && current_orders.Count > 0) //no positions but some orders
            {
                if (board.spread < entry_spread)
                {
                    await FlyerAPI2.cancelAllChildOrdersAsync();
                }
                else
                {
                    foreach (var v in current_orders)
                    {
                        if (v.side == "BUY")
                        {
                            if (v.price <= bid_max)
                            {
                                await FlyerAPI2.cancelChildOrdersAsync(v.child_order_acceptance_id);
                                Form1.Form1Instance.addListBox2("cancelled buy order, id=" + v.child_order_acceptance_id);
                                await FlyerAPI2.sendChiledOrderAsync("BUY", bid_max + 1, order_size, 1);
                                Form1.Form1Instance.addListBox2("ordered buy @" + (bid_max + 1));
                            }
                        }
                        else
                        {
                            if (v.price >= ask_min)
                            {
                                await FlyerAPI2.cancelChildOrdersAsync(v.child_order_acceptance_id);
                                Form1.Form1Instance.addListBox2("cancelled sell order, id=" + v.child_order_acceptance_id);
                                await FlyerAPI2.sendChiledOrderAsync("SELL", ask_min - 1, order_size, 1);
                                Form1.Form1Instance.addListBox2("ordered sell @" + (ask_min - 1));
                                res = "";
                            }
                        }
                    }
                }
            }
            else //holding positions, and orders
            {
                var com = await exitPriceTracingOrder();
                res = "completed exit price tracing order";
            }
            return res;
        }

        private static async Task<string> exitPriceTracingOrder() //continue price tracing order until exit all positions
        {
            Form1.Form1Instance.addListBox2("started exit price tracing order");
            do
            {
                current_positions = await FlyerAPI2.getPositionsAsync();
                var board = BoardDataUpdate.getCurrentBoard();
                if (current_positions.Count > 0)
                {
                    double bid_max = board.Bids.Select(x => x.Price).ToList().Max();
                    double ask_min = board.Asks.Select(x => x.Price).ToList().Min();
                    current_orders = await FlyerAPI2.getChildOrderAsync("ACTIVE");
                    var order_sides = current_orders.Select(x => x.side).ToList();
                    var order_prices = current_orders.Select(x => x.price).ToList();
                    foreach (var v in current_positions)
                    {
                        if (v.side == "BUY") //holding long position
                        {
                            if (order_sides.Contains("SELL")) //if already sell order to exit long position
                            {
                                for (int i = 0; i < order_sides.Count; i++)
                                {
                                    if (order_sides[i] == "SELL")
                                    {
                                        if (order_prices[i] >= ask_min)
                                        {
                                            await FlyerAPI2.cancelChildOrdersAsync(current_orders[i].child_order_acceptance_id);
                                            Form1.Form1Instance.addListBox2("cancelled buy order, id=" + current_orders[i].child_order_acceptance_id);
                                            await FlyerAPI2.sendChiledOrderAsync("SELL", ask_min - 1, v.size, 1);
                                            Form1.Form1Instance.addListBox2("ordered sell @" + (ask_min - 1));
                                            break;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                await FlyerAPI2.sendChiledOrderAsync("SELL", ask_min - 1, v.size, 1);
                                Form1.Form1Instance.addListBox2("ordered sell @" + (ask_min - 1));
                            }
                        }
                        else //holding short position
                        {
                            if (order_sides.Contains("BUY"))//if already buy order to exit short position
                            {
                                for (int i = 0; i < order_sides.Count; i++)
                                {
                                    if (order_sides[i] == "BUY") //alredy sell order to exit long position
                                    {
                                        if (order_prices[i] <= bid_max)
                                        {
                                            await FlyerAPI2.cancelChildOrdersAsync(current_orders[i].child_order_acceptance_id);
                                            Form1.Form1Instance.addListBox2("cancelled buy order, id=" + current_orders[i].child_order_acceptance_id);
                                            await FlyerAPI2.sendChiledOrderAsync("BUY", bid_max + 1, v.size, 1);
                                            Form1.Form1Instance.addListBox2("ordered buy @" + (bid_max + 1));
                                            break;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                await FlyerAPI2.sendChiledOrderAsync("BUY", bid_max+1, v.size, 1);
                                Form1.Form1Instance.addListBox2("ordered buy @" + (bid_max + 1));
                            }
                        }
                    }
                }
            } while (current_positions.Count > 0);

            Form1.Form1Instance.addListBox2("complated exit price tracing order");
            return "completed";
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
