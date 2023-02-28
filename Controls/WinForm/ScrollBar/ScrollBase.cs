using MiMFa.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiMFa.Controls.WinForm.ScrollBar
{
    public class ScrollBase
    {
        public event GenericEventListener<ScrollBase,long> ValueChanged = (s, e) => { };

        private long _Value = 0;
        public long Value
        {
            get => _Value;
            set
            {
                long v = _Value;
                if (value > Maximum) _Value = Maximum;
                else if (value < Minimum) _Value = Minimum;
                else _Value = value;
                if(v != _Value) ValueChanged(this, Value);
            }
        }
        public long Minimum { get; set; } = 0;
        public long Maximum { get; set; } = 1;
        public long ScopeLength => Maximum - Minimum - SlipperLenght;
        public long SlipperLenght { get; internal set; } = 15;
        public long SmallChange { get; set; } = 1;
        public long LargeChange { get; set; } = 10;

        public int GetValue(int min, int max) => Service.ConvertService.TryToInt(Value * ((max - min) / (ScopeLength * 1D)), 0);
        public long SetValue(int min, int max, int value) => Value = Convert.ToInt64(Math.Round(value * ((ScopeLength * 1D) / (max - min))));

        public long SmallChangeValue(int change) => Value += SmallChange * change;
        public long LargeChangeValue(int change) => Value += LargeChange * change;
    }
}
