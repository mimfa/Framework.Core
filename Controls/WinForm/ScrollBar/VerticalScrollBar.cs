using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MiMFa.Controls.WinForm.ScrollBar
{
    [DefaultProperty("Value")]
    [DefaultEvent("Scroll")]
    public partial class VerticalScrollBar : UserControl
    {
        public event EventHandler ValueChanged = (s, e) => { };
        public new event ScrollEventHandler Scroll = (s, e) => { };
        public ScrollBase Scroller = new ScrollBase();

        public int SlipperLocation
        {
            get => Slipper.Location.Y;//Scroller.GetValue(0, IntMaximum);
            private set => UpdateSlipper(value);
        }
        public long SlipperLenght { get => Scroller.SlipperLenght; set { Scroller.SlipperLenght = value; UpdateSlipperSize(); } }
        public int SlipperThickness => Scroller.SlipperLenght<1?15: Math.Max(15, Convert.ToInt32(Height * Scroller.SlipperLenght / Math.Max(Scroller.SlipperLenght,Scroller.ScopeLength)));
        public long LongValue
        {
            get => Scroller.Value;
            set => UpdateValue(value);
        }
        public int Value
        {
            get => LongValue < int.MaxValue ? Convert.ToInt32(LongValue) : int.MaxValue;
            set => LongValue = value;
        }
        public long LongMinimum { get => Scroller.Minimum; set { Scroller.Minimum = value; UpdateSlipperSize(); } }
        public int Minimum
        {
            get => LongMinimum < int.MaxValue ? Convert.ToInt32(LongMinimum) : int.MaxValue;
            set => LongMinimum = value;
        }
        public long LongMaximum { get => Scroller.Maximum; set { Scroller.Maximum = value; UpdateSlipperSize(); } }
        public int Maximum
        {
            get => LongMaximum < int.MaxValue ? Convert.ToInt32(LongMaximum) : int.MaxValue;
            set => LongMaximum = value;
        }
        public long LargeChange { get => Scroller.LargeChange; set => Scroller.LargeChange = value; }
        public long SmallChange { get => Scroller.SmallChange; set => Scroller.SmallChange = value; }
        public int ShowThickness { get; set; } = 17;
        public bool AutoShow { get; set; } = true;
        public bool Hideable { get; set; } = false;

        public int TrackerIteration { get => Tracer.Interval; set => Tracer.Interval = value; }
        public int TrackDelay { get; set; } = 20;


        public VerticalScrollBar()
        {
            InitializeComponent();

            MouseWheel += VerticalScrollBar_MouseWheel;
            p_Slipper.MouseWheel += VerticalScrollBar_MouseWheel;
            Slipper.MouseWheel += VerticalScrollBar_MouseWheel;
            BackButton.MouseWheel += VerticalScrollBar_MouseWheel;
            NextButton.MouseWheel += VerticalScrollBar_MouseWheel;
            ValueChanged += (s,e)=> UpdateSlipper();
            Scroller.ValueChanged += (s, e) => Scroll(this, new ScrollEventArgs(ScrollEventType.ThumbTrack, Value));
        }

        private void VerticalScrollBar_Load(object sender, EventArgs e)
        {
            UpdateValue(LongValue);
        }

        int DraggedTol = 0;
        private void Slipper_MouseDown(object sender, MouseEventArgs e)
        {
            DraggedTol = e.Y;
            Scroll(this, new ScrollEventArgs(ScrollEventType.ThumbPosition, Value));
            Scroll(this, new ScrollEventArgs(ScrollEventType.First, Value));
        }
        private void Slipper_MouseMove(object sender, MouseEventArgs e)
        {
            if (DraggedTol != 0)
            {
                SlipperLocation += e.Y - DraggedTol;
                Scroll(this, new ScrollEventArgs(ScrollEventType.ThumbTrack, Value));
            }
        }
        private void Slipper_MouseUp(object sender, MouseEventArgs e)
        {
            DraggedTol = 0;
            ValueChanged(this,EventArgs.Empty);
            Scroll(this,new ScrollEventArgs(ScrollEventType.EndScroll,Value));
        }
        private void Slipper_LocationChanged(object sender, EventArgs e)
        {
        }

        int _TrackDelay = 20;
        bool _TrackLargeChange = false;
        int _TrackChange = 1;

        private void p_Slipper_MouseClick(object sender, MouseEventArgs e)
        {

        }
        private void p_Slipper_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            SlipperLocation = e.Y;
            ValueChanged(this, EventArgs.Empty);
        }
        private void p_Slipper_MouseDown(object sender, MouseEventArgs e)
        {
            float tol = e.Location.Y - (Slipper.Location.Y + (Slipper.Height / 2F));
            if (tol == 0) return;
            _TrackChange = Convert.ToInt32(tol/10);
            if (tol > 0)
                if (e.Location.Y >= p_Slipper.Height - ((p_Slipper.Height - (Slipper.Location.Y + Slipper.Height)) / 2))
                    _TrackLargeChange = true;
                else
                    _TrackLargeChange = false;
            else if (e.Location.Y <= Slipper.Location.Y / 2)
                _TrackLargeChange = true;
            else
                _TrackLargeChange = false;
            _TrackDelay = TrackDelay;
            Tracer.Enabled = true;
        }
        private void p_Slipper_MouseUp(object sender, MouseEventArgs e)
        {
            _TrackLargeChange = false;
            Tracer.Enabled = false;
            ValueChanged(this, EventArgs.Empty);
        }
        private void BackButton_MouseDown(object sender, MouseEventArgs e)
        {
            _TrackLargeChange = false;
            _TrackChange = -1;
            _TrackDelay = TrackDelay;
            Tracer.Enabled = true;
        }
        private void BackButton_MouseUp(object sender, MouseEventArgs e)
        {
            Tracer.Enabled = false;
            ValueChanged(this, EventArgs.Empty);
        }
        private void BackButton_Click(object sender, EventArgs e)
        {
            Scroller.SmallChangeValue(-1);
            ValueChanged(this, EventArgs.Empty);
        }

        private void NextButton_MouseDown(object sender, MouseEventArgs e)
        {
            _TrackLargeChange = false;
            _TrackChange = 1;
            _TrackDelay = TrackDelay;
            Tracer.Enabled = true;
        }
        private void NextButton_MouseUp(object sender, MouseEventArgs e)
        {
            Tracer.Enabled = false;
            ValueChanged(this, EventArgs.Empty);
        }
        private void NextButton_Click(object sender, EventArgs e)
        {
            Scroller.SmallChangeValue(1);
            ValueChanged(this, EventArgs.Empty);
        }
        private void Tracer_Tick(object sender, EventArgs e)
        {
            if (_TrackDelay-- > 0) return;
            if (_TrackLargeChange)
            {
                Scroller.LargeChangeValue(_TrackChange);
                Scroll(this, new ScrollEventArgs(ScrollEventType.LargeIncrement, Value));
            }
            else
            {
                Scroller.SmallChangeValue(_TrackChange);
                Scroll(this, new ScrollEventArgs(ScrollEventType.SmallIncrement, Value));
            }
            UpdateSlipper();
        }


        private void VerticalScrollBar_MouseWheel(object sender, MouseEventArgs e)
        {
            LongValue -= (SmallChange * e.Delta) / 100;
        }


        public void UpdateSlipper(int value)
        {
            int max = p_Slipper.Height - Slipper.Height;
            int val = value > 0 ? (value < max ? value : max) : 0;
            Scroller.SetValue(0, max, val);
            UpdateSlipper();
        }
        public void UpdateSlipper()
        {
            Slipper.Location = new Point(Slipper.Location.X, Scroller.GetValue(0, p_Slipper.Height - Slipper.Height));
            Scroll(this,new ScrollEventArgs(ScrollEventType.Last,Value));
        }
        public void UpdateValue(long value)
        {
            Scroller.Value = value;
            ValueChanged(this, EventArgs.Empty);
        }

        public void UpdateSlipperSize()
        {
            if (AutoShow) 
                if (Scroller.ScopeLength < Scroller.Maximum)
                    Show();
                else Hide();
            Slipper.Height = SlipperThickness;
        }

        int LatestWidth = 5;
        private void VerticalScrollBar_Enter(object sender, EventArgs e)
        {
            if (!Hideable||Width == ShowThickness) return;
            LatestWidth = Width;
            Width = ShowThickness;
        }
        private void VerticalScrollBar_Leave(object sender, EventArgs e)
        {
            if (!Hideable || Width == LatestWidth) return;
            Width = LatestWidth;
        }

        private void VerticalScrollBar_SizeChanged(object sender, EventArgs e)
        {
            UpdateSlipperSize();
        }
    }
}
