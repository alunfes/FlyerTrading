using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace FlyerTrading
{
    class Account
    {
        /*Performance Variance*/
        public int num_trade { get; set; }
        public double total_pl { get; set; }
        public double ave_pl { get; set; }

        /*Current Order Data*/
        public List<order> current_orders { get; set; }
        public bool cancelling { get; set; }
        private object lockorder = new object();
        public class order
        {
            public DateTime order_dt { get; set; }
            public double order_price { get; set; }
            public double order_lot { get; set; }
            public string order_id { get; set; }
            public string order_status { get; set; } //ordering, placed, executed, cancelled
            public string order_side { get; set; }

            public order()
            {
                order_id = "";
                order_dt = new DateTime();
                order_lot = new double();
                order_status = "";
                order_price = new double();
                order_side = "";
            }
        }
        private void addOrder(DateTime dt, double price, double lot, string id, string status, string side)
        {
            var ord = new order();
            ord.order_dt=dt;
            ord.order_price=price;
            ord.order_lot=lot;
            ord.order_id=id;
            ord.order_status=status;
            ord.order_side=side;
            lock (lockorder)
            {
                current_orders.Add(ord);
            }
        }
        private void removeOrder(int index)
        {
            lock (lockorder)
            {
                current_orders.RemoveAt(index);
            }
        }
        private void removeAllOrders()
        {
            lock (lockorder)
            {
                current_orders = new List<order>();
            }
        }
        public List<order> getAllOrders()
        {
            lock (lockorder)
                return current_orders;
        }
        private void updateOrderStatus(string status, string id)
        {
            lock (lockorder)
            {
                var index = current_orders.Select(x => x.order_id).ToList().IndexOf(id);
                current_orders[index].order_status = status;
            }
        }
        private void updateOrderSize(double size, string id)
        {
            lock(lockorder)
            {
                var index = current_orders.Select(x => x.order_id).ToList().IndexOf(id);
                current_orders[index].order_lot= size;
            }
        }
        private int getNumCurrentOrders()
        {
            lock (lockorder)
                return current_orders.Count();
        }

        /*Holding Data*/
        public List<double> holding_price { get; set; }
        public List<double> holding_size { get; set; }
        public List<string> holding_side { get; set; }
        public double holding_ave_price { get; set; }
        public double holding_total_size { get; set; }
        public string holding_ave_side { get; set; }

        private void addHolding(double price, double size, string side)
        {
            //holding_price.Add(price);
            //holding_side.Add(side);
            //holding_size.Add(size);

            updateAveHolding(price, size, side);
        }
        private void updateAveHolding(double price, double size, string side)
        {

            if (holding_ave_side == "")
            {
                holding_ave_price = price;
                holding_ave_side = side;
                holding_total_size = size;
            }
            else if (holding_ave_side == "BUY")
            {
                if (side == "BUY")
                {
                    holding_ave_price = (holding_ave_price * holding_total_size + price * size) / (holding_total_size + size);
                    holding_total_size += size;
                }
                else if (side == "SELL")
                {
                    if (holding_total_size > size)
                    {
                        holding_total_size -= size;
                    }
                    else if (holding_total_size < size)
                    {
                        holding_ave_price = price;
                        holding_total_size = (size - holding_total_size);
                        holding_ave_side = "SELL";
                    }
                    else
                    {
                        holding_ave_price = 0;
                        holding_ave_side = "";
                        holding_total_size = 0;
                    }
                    total_pl += (price - holding_ave_price) * size;
                }
            }
            else if (holding_ave_side == "SELL")
            {
                if (side == "SELL")
                {
                    holding_ave_price = (holding_ave_price * holding_total_size + price * size) / (holding_total_size + size);
                    holding_total_size += size;
                }
                else if (side == "BUY")
                {
                    if (holding_total_size > size)
                    {
                        holding_total_size -= size;
                    }
                    else if (holding_total_size < size)
                    {
                        holding_ave_price = price;
                        holding_total_size = (size - holding_total_size);
                        holding_ave_side = "BUY";
                    }
                    else
                    {
                        holding_ave_price = 0;
                        holding_ave_side = "";
                        holding_total_size = 0;
                    }
                    total_pl += (price - holding_ave_price) * size;
                }
            }
            Form1.Form1Instance.Invoke((Action)(() => { Form1.Form1Instance.addListBox2("holding_ave_side="+holding_ave_side + ", holding total size="+holding_total_size+", hodling ave price="+holding_ave_price); }));
        }
        private void removeHolding(int index)
        {
            holding_price.RemoveAt(index);
            holding_side.RemoveAt(index);
            holding_size.RemoveAt(index);
            Form1.Form1Instance.Invoke((Action)(() => { Form1.Form1Instance.addListBox2("removed holding"); }));
        }

        /*Log*/
        private Dictionary<long, string> action_log;
        private Dictionary<long, DateTime> action_dt;
        private long num_actions;


        /*Price Tracing Order*/
        public bool price_tracing_order { get; set; }
        private double price_tracing_order_target_lot { get; set; }
        public int price_tracing_order_exit_sec { get; set; } //sec to exit exiting order and place a new order

        public int last_ind_marketdata { get; set; }



        public Account()
        {
            initialzie();
        }

        private async void initialzie()
        {
            num_trade = 0;
            total_pl = 0;
            ave_pl = 0;

            action_log = new Dictionary<long, string>();
            action_dt = new Dictionary<long, DateTime>();
            num_actions = 0;

            current_orders = new List<order>();
            cancelling = false;
            
            last_ind_marketdata = 0;

            price_tracing_order = false;
            price_tracing_order_exit_sec = 3;
            price_tracing_order_target_lot = 0;

            holding_price = new List<double>();
            holding_side = new List<string>();
            holding_size = new List<double>();
            holding_ave_price = 0;
            holding_ave_side = "";
            holding_total_size = 0;

            await checkCancel();
        }
        private async Task<string> checkCancel()
        {
            await Task.Run(async () =>
            {
                while (SystemFlg.getMMFlg())
                {
                    if (cancelling)
                    {
                        var order = await FlyerAPI2.sendChiledOrderAsync("BUY", MarketDataLog.getLastExecutionsData().price - 100000, 0.01, 1); //send dummy order
                        var ord_dt = DateTime.Now;
                        if (order.order_id.Contains("JRF"))
                        {
                            bool flg = true;
                            do
                            {
                                var orders = await FlyerAPI2.getChildOrderAsync("ACTIVE");//get current active orders
                                var dt = DateTime.Now;
                                var orders_id = orders.Select(x => x.child_order_acceptance_id).ToList();
                                if (orders_id.Contains(order.order_id)) //when the dummy order is in the active order list
                                {
                                    //treat all cancelling status orders not exist in order list as properlly cancelled
                                    var ord = getAllOrders();
                                    int num_cancelling = 0;
                                    for (int i = 0; i < ord.Count; i++)
                                    {
                                        if (ord[i].order_status == "CANCELLING" && dt > ord[i].order_dt)
                                        {
                                            num_cancelling++;
                                            //if (orders_id.Contains(ord[i].order_id))//
                                            {
                                                takeLog(DateTime.Now+": cancelled " + ord[i].order_side + ", price=" + ord[i].order_price + " x " + ord[i].order_lot);
                                                Form1.Form1Instance.Invoke((Action)(() => { Form1.Form1Instance.addListBox2("cancelled " + ord[i].order_side + ", price=" + ord[i].order_price + " x " + ord[i].order_lot); }));
                                                removeOrder(i);
                                            }
                                        }
                                    }
                                    if (num_cancelling == 0)//cancel dummy order and stop current checking
                                    {
                                        string res = "error";
                                        do
                                        {
                                            res = await FlyerAPI2.cancelChildOrdersAsync(order.order_id);
                                            flg = false;
                                            cancelling = false;
                                        } while (res == "error");
                                    }
                                    else if((DateTime.Now - ord_dt).Seconds > 60)
                                    {
                                        flg = false;
                                    }
                                }
                                await Task.Delay(500);
                            } while (flg);
                        }
                    }
                }
            });
            return "";
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
            if (res.order_id.Contains("JRF"))
            {
                addOrder(DateTime.Now, p, size, res.order_id, "ACTIVE", order);
                Form1.Form1Instance.Invoke((Action)(() => { Form1.Form1Instance.addListBox2("new entry for " + order + ": price=" + p + ": size=" + size + ": id=" + res.order_id); }));
                takeLog(DateTime.Now + ": new entry for " + order + ": price=" + p + ": size=" + size + ": id=" + res.order_id);
            }
            else
            {
                takeLog("failed new entry, price=" + p);
                Form1.Form1Instance.Invoke((Action)(() => { Form1.Form1Instance.addListBox2("failed new entry for " + order + ": price=" + p + ": size=" + size + ": id=" + res.order_id); }));
            }
            return res;
        }

        public async Task<string> cancelOrder(string id)
        {
            var res = await FlyerAPI2.cancelChildOrdersAsync(id);
            if (res == "")
            {
                var ord = getAllOrders().Where(x=>x.order_id ==id).ToList();
                if (ord.Count == 0)
                {
                    takeLog(DateTime.Now + ": cancelling " + ord[0].order_side + " for " + ord[0].order_price + " x " + ord[0].order_lot + " id=" + ord[0].order_id);
                    Form1.Form1Instance.Invoke((Action)(() => { Form1.Form1Instance.addListBox2("cancelling " + ord[0].order_side + " for " + ord[0].order_price + " x " + ord[0].order_lot + " id=" + ord[0].order_id); }));
                    updateOrderStatus("CANCELLING", id);
                    cancelling = true;
                }
            }
            else
            {
                takeLog("failed cancel order: " + res);
                res = "error";
            }
            return res;
        }

        public async Task<string> cancelAllOrders()
        {
            var ord = getAllOrders();
            string res="";
            for (int i = 0; i < ord.Count; i++)
            {
                if (ord[i].order_status != "CANCELLING")
                {
                    res = await FlyerAPI2.cancelChildOrdersAsync(ord[i].order_id);
                    if (res == "")
                    {
                        updateOrderStatus("CANCELLING", ord[i].order_id);
                        takeLog(DateTime.Now + ": cancelling " + ord[i].order_side + " for " + ord[i].order_price + " x " + ord[i].order_lot + " id=" + ord[i].order_id);
                        Form1.Form1Instance.Invoke((Action)(() => { Form1.Form1Instance.addListBox2("cancelling " + ord[i].order_side + " for " + ord[i].order_price + " x " + ord[i].order_lot + " id=" + ord[i].order_id); }));
                        cancelling = true;
                    }
                    else
                    {
                        takeLog("failed cancel all orders: " + res);
                    }
                }
            }
            return res;
        }


        public async Task<string> startExitPriceTracingOrder()
        {
            string res = "";
            takeLog(DateTime.Now + ": started exit price tracing order");
            Form1.Form1Instance.Invoke((Action)(() => { Form1.Form1Instance.addListBox2("started price tracing order"); }));

            await Task.Run(async () =>
            {
                do
                {
                    await checkExecutionAndUpdateOrders();
                    if (holding_total_size > 0) //if holding position
                    {
                        var board = BoardDataUpdate.getCurrentBoard();
                        double bid_max = board.Bids.Select(x => x.Price).ToList().Max();
                        double ask_min = board.Asks.Select(x => x.Price).ToList().Min();
                        var ord = getAllOrders();

                        if (holding_ave_side == "BUY") //hodling long position
                        {
                            int index = ord.Select(x=>x.order_side).ToList().IndexOf("SELL");
                            if (index >= 0) //exit order is already exist
                            {
                                if (ord[index].order_price >= ask_min)
                                {
                                    var res_cancel = await FlyerAPI2.cancelChildOrdersAsync(ord[index].order_id);
                                    if (res_cancel != "error")
                                    {
                                        takeLog(DateTime.Now + ": PirceTracingOrder - cancelling sell order " + ord[index].order_price + " x " + ord[index].order_lot);
                                        Form1.Form1Instance.Invoke((Action)(() => { Form1.Form1Instance.addListBox2("PirceTracingOrder - cancelling buy order : " + ord[index].order_price+ " x " + ord[index].order_lot); }));
                                        var order = await FlyerAPI2.sendChiledOrderAsync("SELL", ask_min - 1, holding_total_size, 1);
                                        if (order.order_id != "")
                                        {
                                            addOrder(DateTime.Now, ask_min - 1, holding_total_size, order.order_id, "ACTIVE", "SELL");
                                            takeLog(DateTime.Now + ": PirceTracingOrder - entry sell order " + (ask_min - 1).ToString() + " x " + holding_total_size);
                                            Form1.Form1Instance.Invoke((Action)(() => { Form1.Form1Instance.addListBox2("PirceTracingOrder - ordered sell @" + (ask_min - 1)); }));
                                        }
                                    }
                                    break;
                                }
                            }
                            else //if no exit order 
                            {
                                var order = await FlyerAPI2.sendChiledOrderAsync("SELL", ask_min - 1, holding_total_size, 1);
                                if (order.order_id != "")
                                {
                                    addOrder(DateTime.Now, ask_min - 1, holding_total_size, order.order_id, "ACTIVE", "SELL");
                                    takeLog(DateTime.Now + ": PirceTracingOrder - entry sell order " + (ask_min - 1).ToString() + " x " + holding_total_size);
                                    Form1.Form1Instance.Invoke((Action)(() => { Form1.Form1Instance.addListBox2("PirceTracingOrder - ordered sell @" + (ask_min - 1)); }));
                                }
                            }
                        }
                        else if (holding_ave_side == "SELL") //holding short position
                        {
                            int index = ord.Select(x=>x.order_side).ToList().IndexOf("BUY");
                            if (index >= 0) //exit order is already exist
                            {
                                if (ord[index].order_price <= bid_max)
                                {
                                    var res_cancel = await FlyerAPI2.cancelChildOrdersAsync(ord[index].order_id);
                                    if (res_cancel != "error")
                                    {
                                        takeLog(DateTime.Now + ": PirceTracingOrder - cancelling buy order " + ord[index].order_price + " x " + ord[index].order_lot);
                                        Form1.Form1Instance.Invoke((Action)(() => { Form1.Form1Instance.addListBox2("PirceTracingOrder - cancelling buy order : " + ord[index].order_price + " x " + ord[index].order_lot); }));
                                        var order = await FlyerAPI2.sendChiledOrderAsync("BUY", bid_max + 1, holding_total_size, 1);
                                        if (order.order_id != "")
                                        {
                                            addOrder(DateTime.Now, bid_max + 1, holding_total_size, order.order_id, "ACTIVE", "BUY");
                                            takeLog(DateTime.Now + ": PirceTracingOrder - entry buy order " + (bid_max + 1).ToString() + " x " + holding_total_size);
                                            Form1.Form1Instance.Invoke((Action)(() => { Form1.Form1Instance.addListBox2("PirceTracingOrder - ordered buy @" + (bid_max + 1)); }));
                                        }
                                    }
                                }
                            }
                            else //exit order it not yet exit
                            {
                                var order = await FlyerAPI2.sendChiledOrderAsync("BUY", bid_max + 1, holding_total_size, 1);
                                if (order.order_id != "")
                                {
                                    addOrder(DateTime.Now, bid_max + 1, holding_total_size, order.order_id, "ACTIVE", "BUY");
                                    takeLog(DateTime.Now + ": PirceTracingOrder - entry buy order " + (bid_max + 1).ToString() + " x " + holding_total_size);
                                    Form1.Form1Instance.Invoke((Action)(() => { Form1.Form1Instance.addListBox2("PirceTracingOrder - ordered buy @" + (bid_max + 1)); }));
                                }
                            }
                        }
                    }
                } while (holding_size.Count > 0);
            });

            takeLog("Completed exit price tracing order");
            Form1.Form1Instance.Invoke((Action)(() => { Form1.Form1Instance.addListBox2("Completed exit price tracing order"); }));
            return res;
        }


        public async Task<string> checkExecutionAndUpdateOrders()
        {
            var res = "";
            if (MarketDataLog.getNumExecutionsLog() < last_ind_marketdata)
                last_ind_marketdata = 0;

            var exe_data = MarketDataLog.getExecutionsDataRange(last_ind_marketdata);
            last_ind_marketdata += exe_data.Count;
            var ord = getAllOrders();
            for (int i = 0; i < ord.Count; i++)
            {
                for (int j = 0; j < exe_data.Count; j++)
                {
                    if (exe_data[j].buy_child_order_acceptance_id == ord[i].order_id || exe_data[j].sell_child_order_acceptance_id == ord[i].order_id)
                    {
                        ord[i].order_lot -= exe_data[j].size;
                        addHolding(ord[i].order_price, exe_data[j].size, ord[i].order_side);
                        takeLog(DateTime.Now + ": executed " + ord[i].order_side + " for " + exe_data[j].price + " x " + exe_data[j].size);
                        Form1.Form1Instance.Invoke((Action)(() => { Form1.Form1Instance.addListBox2("executed " + ord[i].order_side + " for " + exe_data[j].price + " x " + exe_data[j].size); }));
                        if (ord[i].order_lot <= 0)
                        {
                            takeLog(DateTime.Now + ": order " + ord[i].order_id+" was full filled and removed");
                            Form1.Form1Instance.Invoke((Action)(() => { Form1.Form1Instance.addListBox2("order " + ord[i].order_id + " was full filled and removed"); }));
                            removeOrder(i);                            
                        }
                        else
                        {
                            updateOrderSize(ord[i].order_lot - exe_data[j].size, ord[i].order_id);
                        }
                    }
                }
            }
            return res;
        }
        

        public async Task<string> updateCurrentPositions()
        {
            var res = "";
            holding_price = new List<double>();
            holding_side = new List<string>();
            holding_size = new List<double>();

            var positions = await FlyerAPI2.getPositionsAsync();
            foreach (var v in positions)
            {
                addHolding(v.price, v.size, v.side);
            }
            Form1.Form1Instance.Invoke((Action)(() => { Form1.Form1Instance.addListBox2("initialized holding and update positions"); }));
            return res;
        }


        public void writeLog()
        {
            using (StreamWriter sw = new StreamWriter("./account log.csv", false, Encoding.Default))
            {
                var max = action_log.Keys.ToList().Max();
                for (int i = 0; i <= max; i++)
                {
                    if (action_log.ContainsKey(i))
                    {
                        sw.WriteLine(action_log[i]);
                    }
                }
            }
        }

        public void displayAllLog()
        {
            var max = action_log.Keys.ToList().Max();
            for (int i = 0; i <= max; i++)
            {
                if (action_log.ContainsKey(i))
                {
                    Form1.Form1Instance.addListBox(action_log[i]);
                }
            }
        }
    }
}
