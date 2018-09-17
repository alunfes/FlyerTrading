using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
//using Utf8Json;
//using System.Runtime.Serialization;

namespace FlyerTrading
{
    [JsonObject]
    class BoardData
    {
        [JsonProperty("mid_price")]
        public double MidPrice { get; set; }

        [JsonProperty("bids")]
        public Quote[] Bids { get; set; }

        [JsonProperty("asks")]
        public Quote[] Asks { get; set; }

        [JsonIgnore]
        public double spread { get; set; }

        [JsonIgnore]
        public DateTime dt { get; set; }

        [JsonObject]
        public class Quote
        {
            [JsonProperty("price")]
            public double Price { get; set; }

            [JsonProperty("size")]
            public double Size { get; set; }
        }
        
    }
    /*
    [DataContract]
    public class BoardData
    {
        [DataMember(Name = "mid_price")]
        public double MidPrice { get; set; }

        [DataMember(Name = "bids")]
        public Quote[] Bids { get; set; }

        [DataMember(Name = "asks")]
        public Quote[] Asks { get; set; }

        [IgnoreDataMember]
        public double spread { get; set; }
        [IgnoreDataMember]
        public DateTime dt { get; set; }

        [DataContract]
        public class Quote
        {
            [DataMember(Name = "price")]
            public double Price { get; set; }

            [DataMember(Name = "size")]
            public double Size { get; set; }
        }
        public override string ToString()
        {
            return Encoding.UTF8.GetString(JsonSerializer.Serialize(this));
        }*/



}
