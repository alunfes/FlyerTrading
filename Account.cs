using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyerTrading
{
    class Account
    {
        /*Performance Variance*/
        public int num_trade { get; set; }
        public double total_pl { get; set; }
        public double ave_pl { get; set; }

        /*Current Order Data*/
        public List<DateTime> order_dt { get; set; }
        public List<double> order_price { get; set; }
        public List<double> order_lot { get; set; }
        public List<string> order_id { get; set; }
        public List<string> order_status { get; set; } //ordering, placed, executed, cancelled
        public List<string> order_side { get; set; }
        private void addOrder(DateTime dt, double price, double lot, string id, string status, string side)
        {
            order_dt.Add(dt);
            order_price.Add(price);
            order_lot.Add(lot);
            order_id.Add(id);
            order_status.Add(status);
            order_side.Add(side);
        }
        private void removeOrder(int index)
        {
            order_dt.RemoveAt(index);
            order_price.RemoveAt(index);
            order_lot.RemoveAt(index);
            order_id.RemoveAt(index);
            order_status.RemoveAt(index);
            order_side.RemoveAt(index);
        }
        private void removeAllOrders()
        {
            order_dt = new List<DateTime>();
            order_price = new List<double>();
            order_lot = new List<double>();
            order_id = new List<string>();
            order_status = new List<string>();
            order_side = new List<string>();
        }

        /*Holding Data*/
        public List<double> holding_price { get; set; }
        public List<string> holding_acceptance_id { get; set; }
        public List<double> holding_size { get; set; }
        public List<string> holding_side { get; set; }
        private void addHolding(double price, string id, double size, string side)
        {
            holding_acceptance_id.Add(id);
            holding_price.Add(price);
            holding_side.Add(side);
            holding_size.Add(size);
        }
        private void removeHolding(int index)
        {
            holding_acceptance_id.RemoveAt(index);
            holding_price.RemoveAt(index);
            holding_side.RemoveAt(index);
            holding_size.RemoveAt(index);
        }

        /*Log*/
        private Dictionary<long, string> action_log;
        private Dictionary<long, DateTime> action_dt;
        private long num_actions;


        /*Price Tracing Order*/
        public bool price_tracing_order { get; set; }
        private double price_tracing_order_target_lot { get; set; }
        public int price_tracing_order_exit_sec { get; set; } //sec to exit exiting order and place a new order


        public Account()
        {
            initialzie();
        }

        private void initialzie()
        {
            num_trade = 0;
            total_pl = 0;
            ave_pl = 0;

            action_log = new Dictionary<long, string>();
            action_dt = new Dictionary<long, DateTime>();
            num_actions = 0;

            order_id = new List<string>();
            order_dt = new List<DateTime>();
            order_lot = new List<double>();
            order_status = new List<string>();
            order_price = new List<double>();
            order_side = new List<string>();

            price_tracing_order = false;
            price_tracing_order_exit_sec = 3;
            price_tracing_order_target_lot = 0;

            holding_acceptance_id = new List<string>();
            holding_price = new List<double>();
            holding_side = new List<string>();
            holding_size = new List<double>();
        }

        public void takeLog(string log)
        {
            action_log.Add(num_actions, log);
            action_dt.Add(num_actions, DateTime.Now);
            num_actions++;
        }

        public async Task<OrderData> entry(double p, double size, string order)
        {
            var res = await FlyerAPI2.sendChiledOrderAsync(order, p, size, 10);
            if (res.order_id == "")
            {
                takeLog("failed new entry, price=" + p);
            }
            else
            {
                addOrder(DateTime.Now, p, size, res.order_id, "ACTIVE", order);
                takeLog("new entry for " + order + ": price=" + p + ": size=" + size);
            }
            return res;
        }

        public async Task<string> cancelOrder(int order_index)
        {
            var res = await FlyerAPI2.cancelChildOrdersAsync(order_id[order_index]);
            if (res == "")
            {
                takeLog("cancelled " + order_side[order_index] + " for " + order_price[order_index] + " x " + order_lot[order_index]);
                removeOrder(order_index);
            }
            else
            {
                takeLog("failed cancel order: " + res);
            }
            return res;
        }

        public async Task<string> cancelAllOrders()
        {
            var res = await FlyerAPI2.cancelAllChildOrdersAsync();
            if (res == "")
            {
                takeLog("cancelled all orders");
                removeAllOrders();
            }
            else
            {
                takeLog("failed cancel all orders: " + res);
            }
            return res;
        }

        public async Task<string> startExitPriceTracingOrder()
        {
            string res = "";
            takeLog("started exit price tracing order");

            do
            {
                if (holding_size.Count > 0) //if holding position
                {
                    var board = BoardDataUpdate.getCurrentBoard();
                    double bid_max = board.Bids.Select(x => x.Price).ToList().Max();
                    double ask_min = board.Asks.Select(x => x.Price).ToList().Min();
                    for (int i = 0; i < holding_size.Count; i++)
                    {
                        if (holding_side[i] == "BUY") //hodling long position
                        {
                            int index = order_side.IndexOf("SELL");
                            if (index >= 0) //exit order is already exist
                            {
                                if (order_price[index] >= ask_min)
                                {
                                    await FlyerAPI2.cancelChildOrdersAsync(holding_acceptance_id[i]);
                                    removeOrder(index);
                                    takeLog("PirceTracingOrder - cancelled sell order " + order_price[i] + " x " + order_lot[i]);
                                    Form1.Form1Instance.addListBox2("PirceTracingOrder - cancelled buy order, id=" + holding_acceptance_id[i]);
                                    var order = await FlyerAPI2.sendChiledOrderAsync("SELL", ask_min - 1, holding_size[i], 1);
                                    addOrder(DateTime.Now, ask_min - 1, holding_size[i], order.order_id, "ACTIVE", "SELL");
                                    takeLog("PirceTracingOrder - entry sell order " + (ask_min - 1).ToString() + " x " + order_lot[i]);
                                    Form1.Form1Instance.addListBox2("PirceTracingOrder - ordered sell @" + (ask_min - 1));
                                    break;
                                }
                            }
                            else //exit order it not yet exit
                            {
                                var order = await FlyerAPI2.sendChiledOrderAsync("SELL", ask_min - 1, holding_size[i], 1);
                                addOrder(DateTime.Now, ask_min - 1, holding_size[i], order.order_id, "ACTIVE", "SELL");
                                takeLog("PirceTracingOrder - entry sell order " + (ask_min - 1).ToString() + " x " + order_lot[i]);
                            }
                        }
                        else if (holding_side[i] == "SHORT") //holding short position
                        {
                            int index = order_side.IndexOf("BUY");
                            if (index >= 0) //exit order is already exist
                            {
                                if (order_price[index] <= bid_max)
                                {
                                    await FlyerAPI2.cancelChildOrdersAsync(holding_acceptance_id[i]);
                                    removeOrder(index);
                                    takeLog("PirceTracingOrder - cancelled buy order " + order_price[i] + " x " + order_lot[i]);
                                    Form1.Form1Instance.addListBox2("PirceTracingOrder - cancelled buy order, id=" + holding_acceptance_id[i]);
                                    var order = await FlyerAPI2.sendChiledOrderAsync("BUY", bid_max + 1, holding_size[i], 1);
                                    addOrder(DateTime.Now, bid_max + 1, holding_size[i], order.order_id, "ACTIVE", "BUY");
                                    takeLog("PirceTracingOrder - entry buy order " + (bid_max + 1).ToString() + " x " + order_lot[i]);
                                    Form1.Form1Instance.addListBox2("PirceTracingOrder - ordered sell @" + (ask_min - 1));
                                    break;
                                }
                            }
                            else //exit order it not yet exit
                            {
                                var order = await FlyerAPI2.sendChiledOrderAsync("BUY", bid_max + 1, holding_size[i], 1);
                                addOrder(DateTime.Now, bid_max + 1, holding_size[i], order.order_id, "ACTIVE", "BUY");
                                takeLog("PirceTracingOrder - entry buy order " + (bid_max + 1).ToString() + " x " + order_lot[i]);
                            }
                        }
                    }
                }
                await checkExecutionAndUpdateOrders();
            } while (holding_size.Count > 0);
            takeLog("Completed exit price tracing order");
            Form1.Form1Instance.addListBox2("Completed exit price tracing order");
            return res;
        }


        public async Task<string> checkExecutionAndUpdateOrders()
        {
            var res = "";
            for(int i=0; i<order_dt.Count; i++)
            {
                var execution = await FlyerAPI2.getExecutionsAcceptanceIDAsync(order_id[i]);
                if(execution.Count > 0)
                {
                    addHolding(execution[0].price, execution[0].child_order_accesptance_id, execution[0].size, execution[0].side);
                    removeOrder(i);
                    takeLog("executed "+execution[0].side + " for "+execution[0].price+" x "+execution[0].size);
                    Form1.Form1Instance.addListBox2("executed " + execution[0].side + " for " + execution[0].price + " x " + execution[0].size);
                }
            }
            return res;
        }
    }
}
