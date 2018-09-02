using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FlyerTrading
{
    [JsonObject]
    class OrderData
    {
        [JsonProperty("child_order_acceptance_id")]
        public string order_id { get; set; }
        
    }
}
