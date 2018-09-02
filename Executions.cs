using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utf8Json;
using System.Runtime.Serialization;


namespace FlyerTrading
{
    [DataContract]
    public class Executions
    {
        [DataMember(Name = "id")]
        public long id { get; set; }

        [DataMember(Name = "side")]
        public string side { get; set; }

        [DataMember(Name = "price")]
        public double price { get; set; }

        [DataMember(Name = "size")]
        public double size { get; set; }

        [DataMember(Name = "exec_date")]
        public DateTime exec_date { get; set; }

        [DataMember(Name = "buy_child_order_acceptance_id")]
        public string buy_child_order_acceptance_id { get; set; }

        [DataMember(Name = "sell_child_order_acceptance_id")]
        public string sell_child_order_acceptance_id { get; set; }

        public override string ToString()
        {
            return Encoding.UTF8.GetString(JsonSerializer.Serialize(this));
        }
    }
}
