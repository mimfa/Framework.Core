using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiMFa.Service;

namespace MiMFa.Exclusive.ProgramingTechnology.Tools.Pickup
{
    public class StringPickup : Pickup
    {
        public StringPickup() : base(
            "" + "STR" + ""/*(Alt + 900) text (Alt + 900)*/
            , StartSignString
            , EndSignString
            ,true)
        { }
    }
}