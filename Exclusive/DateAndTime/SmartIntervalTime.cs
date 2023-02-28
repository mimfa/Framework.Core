using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiMFa.Exclusive.DateAndTime
{
    [Serializable]
    public class SmartIntervalTime
    {
        public SmartTime FromTime = new SmartTime();
        public SmartTime ToTime = new SmartTime();
        public SmartTime Time
        {
            get { return FromTime.GetLengthTime(ToTime); }
        }
    }

}
