using MiMFa.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiMFa.Model
{
    [Serializable]
    public class Empty<T>
    {
        public override string ToString()
        {
            return string.Empty;
        }
    }
}
