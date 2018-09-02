using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyerTrading
{
    class Account
    {
        public int num_trade { get; set; }
        public double total_pl { get; set; }
        public double ave_pl { get; set; }

        public List<DateTime> order_dt { get; set; }
        public List<double> order_price { get; set; }
        public List<double> order_lot { get; set; }
        public List<string> order_id { get; set; }
        public List<string> order_status { get; set; } //ordering, placed, executed, cancelled


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

            price_tracing_order = false;
            price_tracing_order_exit_sec = 3;
            price_tracing_order_target_lot = 0;
        }



    }
}
