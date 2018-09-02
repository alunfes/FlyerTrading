using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyerTrading
{
    class SendOrderData
    {
        public string product_code { get; set; }
        public string child_order_type { get; set; }
        public string side { get; set; }
        public double price { get; set; }
        public double size { get; set; }
        public int minute_to_expire { get; set; }
        public string time_in_force { get; set; }
    }
}
