using MiMFa.Model.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiMFa.Framework.ModelFormatLayer.Model
{
    public class HeavyData : StructureBase
    {
        public HeavyData(string fromTable, string fromColumnv, string fromRecord, object value)
        {
            this.FromTable = fromTable;
            this.FromColumn = fromColumnv;
            this.FromRecord = fromRecord;
            this.Value = value;
            ValueType = value.GetType().Name;
        }
        public HeavyData(string fromTable, string fromColumnv, string fromRecord, object value,string valueType)
        {
            this.FromTable = fromTable;
            this.FromColumn = fromColumnv;
            this.FromRecord = fromRecord;
            this.Value = value;
            ValueType = valueType;
        }
        public HeavyData() { }

        public string FromTable { get; set; }
        public string FromColumn { get; set; }
        public string FromRecord { get; set; }
        public object Value { get; set; } = null;
        public string ValueType { get; set; }
    }
}
