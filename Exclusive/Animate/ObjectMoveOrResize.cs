using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Globalization;
using MiMFa.Service;

namespace MiMFa.Exclusive.Animate
{
    public class ObjectMoveOrResize
    {
        public enum MoveOrResize
        {
            Move = -1,
            MoveAndResize = 0,
            Resize = 1
        }

        public ObjectMoveOrResize()
        {
        }
        public ObjectMoveOrResize(MoveOrResize moveOrResize = MoveOrResize.MoveAndResize, params Control[] controls)
        {
            if (controls != null)
                foreach (var item in controls)
                    AddControl(item, item, moveOrResize);
        }
        public void AddControl(MoveOrResize moveOrResize, params Control[] controls)
        {
            if (controls != null)
                for (int i = 0; i < controls.Length; i++)
                    AddControl(controls[i], controls[i], moveOrResize);
        }
        public void AddControl(Control control, MoveOrResize moveOrResize = MoveOrResize.MoveAndResize)
        { AddControl(control,control,moveOrResize); }
        public void AddControl(Control handler, Control control, MoveOrResize moveOrResize = MoveOrResize.MoveAndResize)
        {
            if (Statement.IsDesignTime || handler == null || control == null) return;
            _moving = false;
            _resizing = false;
            _moveIsInterNal = false;
            _cursorStartPoint = Point.Empty;
            MouseIsInLeftEdge = false;
            MouseIsInLeftEdge = false;
            MouseIsInRightEdge = false;
            MouseIsInTopEdge = false;
            MouseIsInBottomEdge = false;
           
            handler.MouseEnter += (sender, e) => { _currentContainerDock = control.Dock; };
            handler.MouseDown += (sender, e) => StartMovingOrResizing(handler, control, e, moveOrResize);
            handler.MouseUp += (sender, e) => StopDragOrResizing(handler, control, moveOrResize);
            handler.MouseLeave += (sender, e) => StopDragOrResizing(handler, control, moveOrResize);
            handler.MouseMove += (sender, e) => MoveControl(handler, control, e, moveOrResize);
        }
        public void RemoveControl(Control control,  MoveOrResize moveOrResize = MoveOrResize.MoveAndResize)
        { AddControl(control,control,moveOrResize); }
        public void RemoveControl(Control handler, Control control,  MoveOrResize moveOrResize = MoveOrResize.MoveAndResize)
        {
            if (handler == null || control == null) return;
            _moving = false;
            _resizing = false;
            _moveIsInterNal = false;
            _cursorStartPoint = Point.Empty;
            MouseIsInLeftEdge = false;
            MouseIsInLeftEdge = false;
            MouseIsInRightEdge = false;
            MouseIsInTopEdge = false;
            MouseIsInBottomEdge = false;

            handler.MouseEnter -= (sender, e) => { _currentContainerDock = control.Dock; };
            handler.MouseDown -= (sender, e) => StartMovingOrResizing(handler, control, e, moveOrResize);
            handler.MouseUp -= (sender, e) => StopDragOrResizing(handler, control, moveOrResize);
            handler.MouseLeave -= (sender, e) => StopDragOrResizing(handler, control, moveOrResize);
            handler.MouseMove -= (sender, e) => MoveControl(handler, control, e, moveOrResize);
        }
        #region Private Region

        private bool _draging;
        private bool _moving;
        private Point _cursorStartPoint;
        private bool _moveIsInterNal;
        private bool _resizing;
        private Size _currentContainerStartSize;
        private DockStyle _currentContainerDock;
        private bool MouseIsInLeftEdge;
        private bool MouseIsInRightEdge;
        private bool MouseIsInTopEdge;
        private bool MouseIsInBottomEdge;
        public bool OnlyInParentArea { get; set; } = true;

        private void UpdateMouseEdgeProperties(Control handler, Control container, Point mouseLocationInControl, MoveOrResize WorkType)
        {
            if (WorkType == MoveOrResize.Move)
                return;
            bool free = container.Dock == DockStyle.None;
            bool resizable = container.Dock != DockStyle.Fill;
            MouseIsInLeftEdge = !free && resizable && container.Dock != DockStyle.Right? false :  Math.Abs(mouseLocationInControl.X) <= 4;
            MouseIsInRightEdge = !free && resizable && container.Dock != DockStyle.Left ? false : Math.Abs(mouseLocationInControl.X - container.Width) <= 4;
            MouseIsInTopEdge = !free && resizable && container.Dock != DockStyle.Bottom ? false : Math.Abs(mouseLocationInControl.Y) <= 4;
            MouseIsInBottomEdge = !free && resizable && container.Dock != DockStyle.Top ? false : Math.Abs(mouseLocationInControl.Y - container.Height) <= 4;
        }
        private void UpdateMouseCursor(Control handler, Control container, MoveOrResize WorkType)
        {
            if (WorkType == MoveOrResize.Move)
                return;
            else if (MouseIsInLeftEdge)
                if (MouseIsInTopEdge)
                    container.Cursor = Cursors.SizeNWSE;
                else if (MouseIsInBottomEdge)
                    container.Cursor = Cursors.SizeNESW;
                else
                    container.Cursor = Cursors.SizeWE;
            else if (MouseIsInRightEdge)
                if (MouseIsInTopEdge)
                    container.Cursor = Cursors.SizeNESW;
                else if (MouseIsInBottomEdge)
                    container.Cursor = Cursors.SizeNWSE;
                else
                    container.Cursor = Cursors.SizeWE;
            else if (MouseIsInTopEdge || MouseIsInBottomEdge)
                container.Cursor = Cursors.SizeNS;
            else container.Cursor = Cursors.Default;
        }

        private string GetSizeAndPositionOfControlsToString(Control container)
        {
            CultureInfo cultureInfo = new CultureInfo("en");
            string info = string.Empty;
            foreach (Control control in ControlService.GetAllControls(container))
            {
                info += control.Name + ":" + control.Left.ToString(cultureInfo) + "," + control.Top.ToString(cultureInfo) + "," +
                        control.Width.ToString(cultureInfo) + "," + control.Height.ToString(cultureInfo) + "*";
            }
            return info;
        }
        private void SetSizeAndPositionOfControlsFromString(Control container, string controlsInfoStr)
        {
            string[] controlsInfo = controlsInfoStr.Split(new[] { "*" }, StringSplitOptions.RemoveEmptyEntries);
            Dictionary<string, string> controlsInfoDictionary = new Dictionary<string, string>();
            foreach (string controlInfo in controlsInfo)
            {
                string[] info = controlInfo.Split(new[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                controlsInfoDictionary.Add(info[0], info[1]);
            }
            foreach (Control control in ControlService.GetAllControls(container))
            {
                string propertiesStr;
                controlsInfoDictionary.TryGetValue(control.Name, out propertiesStr);
                string[] properties = propertiesStr.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                if (properties.Length == 4)
                {
                    control.Left = int.Parse(properties[0]);
                    control.Top = int.Parse(properties[1]);
                    control.Width = int.Parse(properties[2]);
                    control.Height = int.Parse(properties[3]);
                }
            }
        }

        //Target
        private void StartMovingOrResizing(Control handler, Control container, MouseEventArgs e, MoveOrResize WorkType)
        {
            ControlService.SetControlThreadSafe(handler, new Action<object[]>((oa) =>
            {
                if (_moving || _resizing)
                    return;
                if (WorkType != MoveOrResize.Move &&
                    (MouseIsInRightEdge || MouseIsInLeftEdge || MouseIsInTopEdge || MouseIsInBottomEdge))
                {
                    _resizing = true;
                    _currentContainerStartSize = container.Size;
                }
                else if (WorkType != MoveOrResize.Resize)
                {
                    _moving = true;
                    handler.Cursor = Cursors.SizeAll;
                }
                else handler.Cursor = Cursors.Default;
                _currentContainerDock = container.Dock;
                _cursorStartPoint = new Point(e.X, e.Y);
                handler.Capture = true;
                _draging = true;
            }), new object[] { });
        }
        private void MoveControl(Control handler, Control container, MouseEventArgs e, MoveOrResize WorkType)
        {
            ControlService.SetControlThreadSafe(container, new Action<object[]>((oa) =>
            {
                if ((!_resizing && !_moving) || !_draging)
                {
                    _resizing = _moving = _draging;
                    UpdateMouseEdgeProperties(handler,container, new Point(e.X, e.Y), WorkType);
                    UpdateMouseCursor(handler, container, WorkType);
                }
                if (_resizing)
                {
                    if (MouseIsInLeftEdge)
                    {
                        if (MouseIsInTopEdge)
                        {
                            container.Width -= (e.X - _cursorStartPoint.X);
                            container.Left += (e.X - _cursorStartPoint.X);
                            container.Height -= (e.Y - _cursorStartPoint.Y);
                            container.Top += (e.Y - _cursorStartPoint.Y);
                        }
                        else if (MouseIsInBottomEdge)
                        {
                            container.Width -= (e.X - _cursorStartPoint.X);
                            container.Left += (e.X - _cursorStartPoint.X);
                            container.Height = (e.Y - _cursorStartPoint.Y) + _currentContainerStartSize.Height;
                        }
                        else
                        {
                            container.Width -= (e.X - _cursorStartPoint.X);
                            container.Left += (e.X - _cursorStartPoint.X);
                        }
                    }
                    else if (MouseIsInRightEdge)
                    {
                        if (MouseIsInTopEdge)
                        {
                            container.Width = (e.X - _cursorStartPoint.X) + _currentContainerStartSize.Width;
                            container.Height -= (e.Y - _cursorStartPoint.Y);
                            container.Top += (e.Y - _cursorStartPoint.Y);

                        }
                        else if (MouseIsInBottomEdge)
                        {
                            container.Width = (e.X - _cursorStartPoint.X) + _currentContainerStartSize.Width;
                            container.Height = (e.Y - _cursorStartPoint.Y) + _currentContainerStartSize.Height;
                        }
                        else
                        {
                            container.Width = (e.X - _cursorStartPoint.X) + _currentContainerStartSize.Width;
                        }
                    }
                    else if (MouseIsInTopEdge)
                    {
                        container.Height -= (e.Y - _cursorStartPoint.Y);
                        container.Top += (e.Y - _cursorStartPoint.Y);
                    }
                    else if (MouseIsInBottomEdge)
                    {
                        container.Height = (e.Y - _cursorStartPoint.Y) + _currentContainerStartSize.Height;
                    }
                    else
                    {
                        StopDragOrResizing(handler, container, WorkType);
                    }
                }
                else if (_moving)
                {
                    _moveIsInterNal = !_moveIsInterNal;
                    if (!_moveIsInterNal)
                    {
                        int x = (e.X - _cursorStartPoint.X) + container.Left;
                        int y = (e.Y - _cursorStartPoint.Y) + container.Top;
                        if (OnlyInParentArea)
                        {
                            if (x < 0) x = 0;
                            else if (container.Parent != null && x > container.Parent.Width - container.Width) x = container.Parent.Width - container.Width;
                            if (y < 0) y = 0;
                            else if (container.Parent != null && y > container.Parent.Height - container.Height) y = container.Parent.Height - container.Height;
                        }
                        container.Location = new Point(x, y);
                    }
                }
            }), new object[] { });
        }
        private void StopDragOrResizing( Control handler, Control container, MoveOrResize WorkType)
        {
            ControlService.SetControlThreadSafe(container, new Action<object[]>((oa) =>
            {
                container.Dock = _currentContainerDock;
                _draging = false;
                _resizing = false;
                _moving = false;
                handler.Capture = false;
                MouseIsInLeftEdge = false;
                MouseIsInRightEdge = false;
                MouseIsInTopEdge = false;
                MouseIsInBottomEdge = false;
                UpdateMouseCursor(handler, container, WorkType);
            }), new object[] { });
        }

        #endregion
    }
}
