using MiMFa.Model.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiMFa.Model
{
    [Serializable]
    public class ObjectDetail : StructureBase 
    {
        public string AliasName { get; set; } = "";
        public string Description { get; set; } = "";
    }
}
