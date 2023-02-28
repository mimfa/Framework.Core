using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiMFa.Exclusive.ProgramingTechnology.Tools
{
    public delegate void ActionEventHandler(object sender,string action);
    public delegate void ActionResultEventHandler(object sender,string action,object result);
    public delegate void ActionErrorEventHandler(object sender,string action,Exception ex);
}
