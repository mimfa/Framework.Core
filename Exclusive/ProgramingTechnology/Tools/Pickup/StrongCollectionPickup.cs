using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiMFa.Service;

namespace MiMFa.Exclusive.ProgramingTechnology.Tools.Pickup
{
    public class StrongCollectionPickup : Pickup
    {
        public StrongCollectionPickup() : base(
            "" + "STRCOLL" + ""/*(Alt + 900) text (Alt + 900)*/
            , StartSignStrongCollection
            , EndSignStrongCollection
            , true)
        { }    
    }
}
