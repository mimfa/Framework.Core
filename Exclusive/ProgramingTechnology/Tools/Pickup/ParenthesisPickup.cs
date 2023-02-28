using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiMFa.Service;

namespace MiMFa.Exclusive.ProgramingTechnology.Tools.Pickup
{
    public class ParenthesisPickup : Pickup
    {
        public ParenthesisPickup() : base(
            "" + "PRNTS" + ""/*(Alt + 900) text (Alt + 900)*/
            , StartSignParenthesis
            , EndSignParenthesis
            , true)
        { }    
    }
}
