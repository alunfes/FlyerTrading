using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyerTrading
{
    class ExecutionData
    {
        public int id { get; set; }
        public string child_order_id { get; set; }
        public string side { get; set; }
        public double price { get; set; }
        public double size { get; set; }
        public double commission { get; set; }
        public DateTime exec_date { get; set; }
        public string child_order_accesptance_id { get; set; }

    }
}
