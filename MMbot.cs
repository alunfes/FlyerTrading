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

            while (SystemFlg.getMMFlg())
            {
                await Task.Run(async () =>
                {
                    if (FlyerAPI2.getApiAccessProhibition() == false)
                    {
                        //var tdd = strategy(account ac)
                        //if(tdd.position == "BUY" || "SELL")
                        //else if(tdd.position == "Cancel")
                        //else if(tdd.position == "Cencel_ALL")
                        //else if(tdd.position == "ExitPriceTracingOrder")
                        //FlyerAPI2.sendChiledOrderAsync()
                    }
                    else
                    {
                        Form1.Form1Instance.setLabel4("api prohibited");
                        await Task.Delay(1000);
                    }
                    return 0;
                });
            }

            Form1.Form1Instance.addListBox2("Finished MMBot");
        }

        private static void initialize()
        {
            current_positions = new List<PositionData>();
            trade_log = new Dictionary<int, string>();
            current_order_ids = new List<string>();
            current_orders = new List<ChildOrderData>();
            num_log = 0;
        }

        private static async Task<string> MMStrategy(Account ac)
        {
            string res = "";

            var board = BoardDataUpdate.getCurrentBoard();
            var bids = board.Bids.Select(x => x.Price).ToList();
            var asks = board.Asks.Select(x => x.Price).ToList();

            double bid_max = bids.Max();
            double ask_min = asks.Min();

            if (ac.holding_acceptance_id.Count == 0 && ac.order_id.Count == 0) //no positions, no orders
            {
                if (board.spread >= entry_spread)
                {
                    var sell_order = ac.entry(ask_min - 1, order_size, "SELL");
                    var buy_order = ac.entry(bid_max + 1, order_size, "BUY");
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
