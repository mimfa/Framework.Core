using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiMFa.Exclusive.DateAndTime
{
    [Serializable]
    public class SmartIntervalMeetTime
    {
        public SmartDate Date = new SmartDate();
        public SmartTime Time = new SmartTime();
    }

}
