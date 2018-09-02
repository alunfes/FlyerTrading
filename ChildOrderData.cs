using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyerTrading
{
    class ChildOrderData
    {
        public int id { get; set; }
        public string child_order_id { get; set; }
        public string product_code { get; set; }
        public string side { get; set; }
        public string child_order_type { get; set; }
        public double price { get; set; }
        public double average_price { get; set; }
        public double size { get; set; }
        public string child_order_state { get; set; }
        public DateTime expired_date { get; set; }
        public DateTime child_order_date { get; set; }
        public string child_order_acceptance_id { get; set; }
        public double outstanding_size { get; set; }
        public double cancel_size { get; set; }
        public double executed_size { get; set; }
        public double total_commission { get; set; }
    }
}
