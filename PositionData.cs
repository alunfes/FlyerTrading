using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyerTrading
{
    class PositionData
    {
        public string product_code { get; set; }
        public string side { get; set; }
        public double price { get; set; }
        public double size { get; set; }
        public double commission { get; set; }
        public double swap_point_accumulate { get; set; }
        public double required_collateral { get; set; }
        public DateTime open_date { get; set; }
        public double leverage { get; set; }
        public double pnl { get; set; }
        public double sfd { get; set; }

    }
}
