using MiMFa.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiMFa.Model
{
    [Serializable]
    public class SmartStack<T> : Stack<T>
    {
        public string SplitSign { get; set; } = Environment.NewLine;

        public override string ToString()
        {
            return CollectionService.GetAllItems(this, SplitSign,0);
        }
    }
}
