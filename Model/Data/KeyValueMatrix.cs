using MiMFa.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiMFa.Model
{
    [Serializable]
    public class KeyValueMatrix<XTKey, YTKey, TValue> : Matrix<TValue> where XTKey : class where YTKey : class
    {
        public List<XTKey> XKeys { get; set; }
        public List<YTKey> YKeys { get; set; }
        public TValue this[XTKey ykey, YTKey xkey]
        {
            get { return this[YKeys.FindIndex(v => v == ykey), XKeys.FindIndex(v => v == xkey)]; }
            set { this[YKeys.FindIndex(v => v == ykey), XKeys.FindIndex(v => v == xkey)] = value; }
        }

        public KeyValueMatrix() : base()
        {

        }
        public KeyValueMatrix(IEnumerable<YTKey> yKeys, IEnumerable<XTKey> xKeys, TValue defval) : base(yKeys.Count(), xKeys.Count(), defval)
        {
            YKeys = yKeys.ToList();
            XKeys = xKeys.ToList();
        }
    }
}
