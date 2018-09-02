using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace FlyerTrading
{
    //[DataContract]
    class BalanceData
    {
        //[DataMember(Name="currency_code")]
        public string currency_code { get; set; }

        //[DataMember(Name ="amount")]
        public double amount { get; set; }

        //[DataMember(Name = "available")]
        public double available { get; set; } 
    }
}
