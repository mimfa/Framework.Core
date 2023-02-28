using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiMFa.Exclusive.DateAndTime
{
    [Serializable]
    public class SmartIntervalDate
    {
        public SmartDate FromDate = new SmartDate();
        public SmartDate ToDate = new SmartDate();
        public SmartDate Date
        {
            get { return FromDate.GetLengthDate(ToDate); }
        }
    }

}
