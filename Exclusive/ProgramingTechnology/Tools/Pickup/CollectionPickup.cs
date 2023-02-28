using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiMFa.Service;

namespace MiMFa.Exclusive.ProgramingTechnology.Tools.Pickup
{
    public class CollectionPickup : Pickup
    {
        public CollectionPickup() : base(
            "" + "COLL" + ""/*(Alt + 900) text (Alt + 900)*/
            , StartSignCollection
            , EndSignCollection
            , true)
        { }    
    }
}
