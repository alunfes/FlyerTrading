using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyerTrading
{
    static class DateTimeOffsetExtension
    {
        static readonly DateTimeOffset unixEpoch = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);

        public static long ToUnixTimeSeconds(this DateTimeOffset dt)
        {
            return (long)dt.Subtract(unixEpoch).TotalSeconds;
        }
    }
}
