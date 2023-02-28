using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiMFa.Service;

namespace MiMFa.Exclusive.ProgramingTechnology.Tools.Pickup
{
    public class CommentProvider : Pickup
    {
        public CommentProvider() : base(
            ""
            , StartSignComment
            , EndSignComment
            , true)
        { }
    }
}