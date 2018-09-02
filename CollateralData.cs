using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyerTrading
{
    class CollateralData
    {
        public double collateral { get; set; } //預け入れた証拠金の評価額（円）です。
        public double open_position_pnl { get; set; } //建玉の評価損益（円）です。
        public double require_collateral { get; set; } //現在の必要証拠金（円）です。
        public double keep_rate { get; set; } //現在の証拠金維持率です。

    }
}
