using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiMFa.Service;

namespace MiMFa.Exclusive.ProgramingTechnology.Tools.Pickup
{
    public class StrongParenthesisPickup : Pickup
    {
        public StrongParenthesisPickup() : base(
            "" + "STRPRNTS" + ""/*(Alt + 900) text (Alt + 900)*/
            , StartSignStrongParenthesis
            , EndSignStrongParenthesis
            , true)
        { }    
    }
}
