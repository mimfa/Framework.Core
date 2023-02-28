using MiMFa.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiMFa.Model.Status
{
    [Serializable]
    public class FuzzyStatus :  CrispStatus
    {
        public double Value { get; set; } = 0;

        public double Tolerance { get; set; } = 0.01;

        public double Maximum { get; set; } = 0;
        public double MiddleMaximum { get; set; } = 0;
        public double Middle { get; set; } = 0;
        public double MiddleMinimum { get; set; } = 0;
        public double Minimum { get; set; } = 0;

        /// <summary>
        /// Fuzzy Status
        /// </summary>
        /// <param name="value">A number between minimum-maximum</param>
        /// <param name="tolerance"></param>
        /// <param name="maximum"></param>
        /// <param name="minimum"></param>
        public FuzzyStatus(double value = 0, double tolerance = 0.01, double maximum = 1, double minimum = -1):base()
        {
            Value = value;
            Maximum = maximum;
            Minimum = minimum;
            Middle = (Maximum + Minimum) / 2d;
            MiddleMaximum = (Maximum + Middle) / 2d;
            MiddleMinimum = (Minimum + Middle) / 2d;
            Tolerance = tolerance;
        }

        public override bool IsVeryNegative { get => Value < Middle && Value < MiddleMinimum; set { if (value) { Value = Minimum; } } }
        public override bool IsNegative { get => Value < Middle; set { if (value) Value = MiddleMinimum;  else Value = Middle; } }
        public override bool IsMiddle { get => MathService.IsAround(Value, Middle,Tolerance); set { if (value) Value= Middle; } }
        public override bool IsPositive { get => Value > Middle; set { if (value) Value = MiddleMaximum;  else Value = Middle; } }
        public override bool IsVeryPositive { get => Value > Middle && Value > MiddleMaximum; set { if (value) Value = Maximum; } }

        public override bool IsVeryUseful
        {
            get => Value > Middle && Value > MiddleMaximum;
            set => IsVeryPositive = value;
        }
        public override bool IsUseful
        {
            get => Value > Middle;
            set => IsPositive = value;
        }

        public override bool IsHarmful
        {
            get => Value < Middle;
            set => IsNegative = value;
        }
        public override bool IsVeryHarmful
        {
            get => Value < Middle && Value < MiddleMinimum;
            set => IsVeryNegative = value;
        }
    }
}
