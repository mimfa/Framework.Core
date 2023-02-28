using MiMFa.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiMFa.Model
{
    [Serializable]
    public class SmartQueue<T> : Queue<T>
    {
        public string SplitSign { get; set; } = Environment.NewLine;

        public override string ToString()
        {
            return CollectionService.GetAllItems(this.ToArray(), SplitSign,0);
        }
    }
}
