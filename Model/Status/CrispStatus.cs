using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiMFa.Model.Status
{
    [Serializable]
    public class CrispStatus : IStatus
    {
        public virtual bool IsVeryNegative { get => IsNegative && _IsVeryNegative; set { if (_IsVeryNegative = value) IsNegative = true; } }
        private bool _IsVeryNegative = false;
        public virtual bool IsNegative { get => _IsNegative; set { if (_IsNegative = value) IsMiddle = false; else _IsVeryNegative = value; } }
        private bool _IsNegative = false;
        public virtual bool IsMiddle { get; set; } = false;
        public virtual bool IsPositive { get => _IsPositive; set { if (_IsPositive = value) IsMiddle = false; else _IsVeryGood = value; } }
        private bool _IsPositive = false;
        public virtual bool IsVeryPositive { get => IsPositive && _IsVeryGood; set { if (_IsVeryGood = value) IsPositive = true; } }
        private bool _IsVeryGood = false;

        public virtual bool IsVeryUseful
        {
            get => !IsNegative && IsVeryPositive;
            set => IsVeryPositive = value;
        }
        public virtual bool IsUseful
        {
            get => !IsNegative && IsPositive;
            set => IsPositive = value;
        }

        public virtual bool IsStable
        {
            get => !IsNegative && !IsMiddle;
            set => IsNegative = !value;
        }
        public virtual bool IsRisky
        {
            get => (IsNegative && !IsPositive) || (IsVeryNegative && !IsVeryPositive);
            set => IsMiddle = !value;
        }

        public virtual bool IsHarmful
        {
            get => !IsPositive && IsNegative;
            set => IsNegative = value;
        }
        public virtual bool IsVeryHarmful
        {
            get => !IsPositive && IsVeryNegative;
            set => IsVeryNegative = value;
        }
    }
}
