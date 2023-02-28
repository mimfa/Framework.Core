using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiMFa.Exclusive.ProgramingTechnology
{
    public interface IProgramingTechnology
    {
        string Name {get;}
        string Description { get;}
        Version Version { get; }
    }
}
