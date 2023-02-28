using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace MiMFa.Exclusive.Animate
{
    public class FormMove
    {
        public Control MainControl { get; set; } = null;
        public List<Control> Controls { get; private set; } = new List<Control>();
        public void AddToControl(params Control[] controls)
        {
            for (int i = 0; i < controls.Length; i++)
            {
                controls[i].MouseDown += MouseDragedDown;
                controls[i].MouseMove += MouseDragedMove;
                controls[i].MouseUp += MouseDragedUp;
                controls[i].MouseLeave += MouseDragedLeave;
            }
            Controls.AddRange(controls);
        }
        public void RemoveFromControl(params Control[] controls)
        {
            for (int i = 0; i < controls.Length; i++)
            {
                controls[i].MouseDown -= MouseDragedDown;
                controls[i].MouseMove -= MouseDragedMove;
                controls[i].MouseUp -= MouseDragedUp;
                controls[i].MouseLeave -= MouseDragedLeave;
                Controls.Remove(controls[i]);
            }
        }

        public FormMove(Form mainControl = null, params Control[] controls)
        {
            MainControl = mainControl;
            AddToControl(controls);
        }

        //--------------------------------------------------------------
        public bool MouseDraging = false; // جابجا کردن صفحه
        public Point StartPoint = new Point(0, 0);// جابجا کردن صفحه 
        public int x, y;// جابجا کردن صفحه
        
        //target
        public void MouseDragedDown(object sender, MouseEventArgs e)
        {
            if (MainControl == null) MainControl = ((Control)sender).FindForm();
            MouseDragedDown(e);
        }
        public void MouseDragedMove(object sender, MouseEventArgs e)
        {
            if (MainControl == null) MainControl = ((Control)sender).FindForm();
            MouseDragedMove(e);
        }
        public void MouseDragedUp(object sender, MouseEventArgs e)
        {
            if (MainControl == null) MainControl = ((Control)sender).FindForm();
            MouseDragedUp();
        }
        public void MouseDragedLeave(object sender, EventArgs e)
        {
            if (MainControl == null) MainControl = ((Control)sender).FindForm();
            MouseDragedLeave();
        }

        //action
        public void MouseDragedDown(MouseEventArgs e)
        {
            MouseDraging = true;
            StartPoint = new Point(e.X, e.Y);
        }
        public void MouseDragedMove(MouseEventArgs e)
        {
            if (MouseDraging)
                try
                {
                    Point PTS = MainControl.PointToScreen(e.Location);
                    x = PTS.X - StartPoint.X;
                    y = PTS.Y - StartPoint.Y;
                    MainControl.Location = new Point(x, y);
                }
                catch { }
        }
        public void MouseDragedUp()
        {
            MouseDraging = false;
        }
        public void MouseDragedLeave()
        {
            MouseDraging = false;
        }
    }
}
