using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MiMFa.Service;
using System.Drawing;
using MiMFa.Model;
using System.Data;
using MiMFa.Exclusive.View;
using System.Reflection;
using System.Threading;
using System.ComponentModel;
using System.Threading.Tasks;
using MiMFa.General;
using System.IO;
using System.Windows.Forms.DataVisualization.Charting;
using System.Text.RegularExpressions;

namespace MiMFa.Service
{
    public enum TableScope
    {
        Table,
        Column,
        Row,
        Cell
    }
    public static class ControlService
    {
        #region Threading
        private delegate void SetControlPropertyThreadSafeDelegate(System.Windows.Forms.Control control, string propertyName, object propertyValue);
        public static void SetControlDebugSafe(Action action, params Control[] controls)
        {
            if (System.Diagnostics.Debugger.IsAttached)
                SetControlThreadSafe(action, controls);
            else action();
        }
        public static void SetControlThreadSafe(Action action, params Control[] controls) => SetControlThreadSafe(controls,action);
        public static void SetControlDebugSafe(Control[] controls, Action action)
        {
            if (System.Diagnostics.Debugger.IsAttached)
                SetControlThreadSafe(controls, action);
            else
                action();
        }
        public static void SetControlThreadSafe(Control[] controls, Action action)
        {
            bool b = false;
            foreach (var control in controls)
                if(control != null)
                b = control.InvokeRequired || b;
            if(b) try { controls.First(v=>v!=null).Invoke(new Action<Action, Control[]>(SetControlThreadSafe), action, controls); } catch { }
            else action();
        }
        public static void SetControlDebugSafe(Control control, Action action)
        {
            if (System.Diagnostics.Debugger.IsAttached)
                SetControlThreadSafe(control, action);
            else
                action();
        }
        public static void SetControlThreadSafe(Control control, Action  action)
        {
            if (control == null) return;
            if (control.InvokeRequired)
                try { control.Invoke(new Action<Control, Action>(SetControlThreadSafe), control, action); } catch { }
            else action();
        }
        public static void SetControlDebugSafe(Control control, Action<object[]> action, object[] args = null)
        {
            if (System.Diagnostics.Debugger.IsAttached)
                SetControlThreadSafe(control, action, args);
            else
                action(args);
        }
        public static void SetControlThreadSafe(Control control, Action<object[]> action, object[] args = null)
        {
            if (control == null) return;
            if (control.InvokeRequired)
                try { control.Invoke(new Action<Control, Action<object[]>, object[]>(SetControlThreadSafe), control, action, args); } catch { }
            else action(args);
        }
        public static T SetControlThreadSafe<T>(Control control, Func<object[],T> action, object[] args = null)
        {
            if (control == null) return default(T);
            if (control.InvokeRequired)
                try { return (T)control.Invoke(new Func<Control, Func<object[],T>, object[],T>(SetControlThreadSafe), control, action, args); } 
                catch { return default(T); }
            else return action(args);
        }

        public static void StartThreadInControl(ThreadingMethodMode method, System.Windows.Forms.Control control, Action<object[]> action, object[] args=null)
        {
            StartThread(method,
                (targs) => SetControlThreadSafe(control, action, targs)
                , args);
        }
        public static dynamic StartThread(ThreadingMethodMode method, Action<object[]> action, object[] args = null)
        {
            switch (method)
            {
                case ThreadingMethodMode.Null:
                    action(args);
                    break;
                case ThreadingMethodMode.SingleThread:
                    Thread thread = new Thread(() =>
                    {
                        action(args);
                    });
                    thread.SetApartmentState(ApartmentState.STA);
                    thread.IsBackground = false;
                    thread.Start();
                    return thread;
                case ThreadingMethodMode.MultiThread:
                    Thread mthread = new Thread(() =>
                    {
                        action(args);
                    });
                    mthread.SetApartmentState(ApartmentState.MTA);
                    mthread.Start();
                    return mthread;
                case ThreadingMethodMode.SingleTask:
                    Task.Run(() =>
                    {
                        action(args);
                    });
                    break;
                case ThreadingMethodMode.MultiTask:
                    Task.Run(() =>
                    {
                        action(args);
                    });
                    break;
                case ThreadingMethodMode.BackgroundWorker:
                    BackgroundWorker BV = new BackgroundWorker();
                    BV.WorkerReportsProgress = true;
                    BV.DoWork += (s, e) =>
                    {
                        action(args);
                    };
                    BV.RunWorkerAsync();
                    return BV;
                default:
                    Thread sthread = new Thread(() =>
                    {
                        action(args);
                    });
                    sthread.SetApartmentState(ApartmentState.STA);
                    sthread.IsBackground = true;
                    sthread.Start();
                    return sthread;
            }
            return null;
        }
        [System.Security.Permissions.SecurityPermissionAttribute(System.Security.Permissions.SecurityAction.Demand, ControlThread = true)]
        public static void KillThread(Thread th)
        {
            if (th != null)
            {
                th.Interrupt();
                th.Abort();
                th = null;
                //th.Join();
                //th.Suspend();
            }
        }
#endregion

        public static void SetSafeControlLocation(System.Windows.Forms.Control control, System.Windows.Forms.Control parent)
        {
            control.Location = GetSafeControlLocation(control, parent);
        }
        public static Point GetSafeControlLocation(System.Windows.Forms.Control control, System.Windows.Forms.Control parent)
        {
            return GetSafeControlLocation(control, parent.PointToClient(System.Windows.Forms.Cursor.Position), parent);
        }
        public static Point GetSafeControlLocation(System.Windows.Forms.Control control,Point location, System.Windows.Forms.Control parent)
        {
            int nx = location.X;
            int ny = location.Y;
            int x = location.X;
            int y = location.Y;
            int xsize = control.Size.Width;
            int ysize = control.Size.Height;
            int xmax = parent.Size.Width;
            int ymax = parent.Size.Height;

            if (x + xsize < xmax) nx = x;
            else if (x-xsize > 0) nx = x - xsize;
            else nx = x-xsize/2;
            if (y + ysize < ymax) ny = y;
            else if (y - ysize > 0) ny = y - ysize;
            else ny = y - ysize / 2;

            return new Point(nx, ny);
        }
        public static System.Windows.Forms.Control FindFocusedControl(System.Windows.Forms.Control mainControl)
        {
            ContainerControl container = mainControl as ContainerControl;
            while (container != null)
            {
                mainControl = container.ActiveControl;
                container = mainControl as ContainerControl;
            }
            return mainControl;
        }
        public static List<T> FindControlByType<T>(System.Windows.Forms.Control mainControl, bool getAllChild = false) where T : System.Windows.Forms.Control
        {
            List<T> lt = new List<T>();
            for (int i = 0; i < mainControl.Controls.Count; i++)
            {
                if (mainControl.Controls[i] is T) lt.Add((T)mainControl.Controls[i]);
                if (getAllChild) lt.AddRange(FindControlByType<T>(mainControl.Controls[i], getAllChild));
            }
            return lt;
        }
        public static System.Windows.Forms.Control GetControlsByName(System.Windows.Forms.Control parent, string name)
        {
            for (int i = 0; i < parent.Controls.Count; i++)
                if (parent.Controls[i].Name == name) return parent.Controls[i];
            return null;
        }
        public static bool IsLastObject(object obj)
        {
            if (obj == null) return true;
            Type type = obj.GetType();
            return 
                type.IsArray ||
                type.IsEnum ||
                type.IsPointer ||
                type.IsAbstract ||
                type.IsSealed;
        }
        public static IEnumerable<object> GetAllObjects(object obj, int maxNests = 1)
        {
            if (obj == null || maxNests < 0) yield break;
            Type type = obj.GetType();
            foreach (var item in type.GetFields())
            {
                var nobj = item.GetValue(obj);
                if (nobj == null) continue;
                yield return nobj;
                if (IsLastObject(nobj)) continue;
                foreach (var ni in GetAllObjects(nobj, maxNests-1))
                    yield return ni;
            }
            foreach (var item in type.GetProperties(BindingFlags.Instance))
            {
                var nobj = item.GetValue(obj);
                if (nobj == null) continue;
                yield return nobj;
                if (IsLastObject(nobj)) continue;
                foreach (var ni in GetAllObjects(nobj, maxNests - 1))
                    yield return ni;
            }
        }
        public static IEnumerable<Control> GetAllControls(object obj, int maxNests = 1)
        {
            if (obj == null || maxNests < 1) yield break;
            Type type = obj.GetType();
            List<Control> list = new List<Control>();
            if (obj is Control)
                foreach (Control item in ((Control)obj).Controls)
                {
                    list.Add(item);
                    yield return item;
                    foreach (var ni in GetAllControls(item, maxNests - 1))
                        yield return ni;
                }
            foreach (var item in type.GetFields())
            {
                var nobj = item.GetValue(obj);
                if (nobj == null) continue;
                if (nobj is Control && !list.Contains((Control)nobj)) yield return (Control)nobj;
                if (IsLastObject(nobj)) continue;
                foreach (var ni in GetAllControls(nobj, maxNests - 1))
                    yield return ni;
            }
        }
        public static IEnumerable<Control> GetFinalControls(object obj, int maxNests = 10)
        {
            if (obj == null || maxNests < 1) yield break;
            Type type = obj.GetType();
            List<Control> list = new List<Control>();
            if (obj is Control)
                if (((Control)obj).HasChildren)
                    foreach (Control item in ((Control)obj).Controls)
                    {
                        if (item.HasChildren)
                            foreach (var ni in GetFinalControls(item, maxNests - 1))
                            {
                                if (!list.Exists(v => v.Name == ni.Name))
                                {
                                    list.Add(ni);
                                    yield return ni;
                                }
                            }
                        else if (!list.Exists(v => v.Name == item.Name))
                        {
                            list.Add(item);
                            yield return item;
                        }
                    }
                else if (!list.Exists(v => v.Name == ((Control)obj).Name))
                {
                    list.Add(((Control)obj)); yield return ((Control)obj);
                }
            foreach (var item in type.GetFields())
            {
                var nobj = item.GetValue(obj);
                if (nobj == null) continue;
                if (nobj is Control)
                    if (((Control)nobj).HasChildren)
                        foreach (var ni in GetFinalControls(nobj, maxNests - 1))
                        {
                            if (!list.Exists(v => v.Name == ni.Name))
                            {
                                list.Add(ni); yield return ni;
                            }
                        }
                    else if (!list.Exists(v => v.Name == ((Control)nobj).Name))
                    {
                        list.Add((Control)nobj); yield return (Control)nobj;
                    }
                    else if (IsLastObject(nobj)) continue;
                    else foreach (var nim in GetFinalControls(nobj, maxNests - 1))
                            if (!list.Exists(v => v.Name == nim.Name))
                            {
                                list.Add(nim); yield return nim;
                            }
            }
        }
        public static IEnumerable<Component> GetAllComponents(object control, int maxNests = 10)
        {
            foreach (var item in GetAllObjects(control, maxNests))
                if (item is Component) yield return (Component)item;
        }
        public static IEnumerable<ToolStrip> GetAllToolStrips(object control, int maxNests = 10)
        {
            foreach (var item in GetAllObjects(control, maxNests))
                if (item is ToolStrip) yield return (ToolStrip)item;
        }
        public static void ChangeChildrenVisibility(Control gb_Type_Options, bool visible = false, bool subsets = false)
        {
            if(visible)
                foreach (Control item in gb_Type_Options.Controls)
            {
                item.Show();
                if (subsets) ChangeChildrenVisibility(item, visible, subsets);
            }
            else 
                foreach (Control item in gb_Type_Options.Controls)
            {
                item.Hide();
                if (subsets) ChangeChildrenVisibility(item, visible, subsets);
            }
        }

        public static bool RemoveNextControls(Control control)
        {
            if (control == null || control.Parent == null) return false;
            for (int i = 0; i < control.Parent.Controls.Count; i++)
                if (control.Parent.Controls[i] == control)
                {
                    for (int j = control.Parent.Controls.Count - 1; j > i; j--)
                        control.Parent.Controls.RemoveAt(j);
                    return true;
                }
            return false;
        }
        public static bool RemovePreviousControls(Control control)
        {
            if (control == null || control.Parent == null) return false;
            for (int j = 0; j < control.Parent.Controls.Count - 1;)
                if (control.Parent.Controls[j] == control)
                    return j>0;
                else control.Parent.Controls.RemoveAt(j);
            return false;
        }
        public static void SetControlLocation(System.Windows.Forms.Control control, Point point, bool forceInFrame = false)
        {
            control.SuspendLayout();
            if (forceInFrame && control.Parent !=null)
            {
                int x = point.X + control.Margin.Left;
                int y = point.Y + control.Margin.Top;
                if (x + control.Width >= control.Parent.Width)
                    x -= control.Width + control.Margin.Left + control.Margin.Right;
                if (y + control.Height >= control.Parent.Height)
                    y -= control.Height + control.Margin.Top + control.Margin.Bottom;
                control.Location = new Point(x,y);
            }
            else control.Location = new Point(point.X + control.Margin.Left, point.Y + +control.Margin.Top);
            control.ResumeLayout(true);
        }
        public static void ControlInDesignHorizontal(System.Windows.Forms.Control control, bool changeFirstcontrolLocation)
        {
            control.SuspendLayout();
            if (changeFirstcontrolLocation && control.Controls.Count > 0)
                if (control.RightToLeft == RightToLeft.Yes)
                    SetControlLocation(control.Controls[control.Controls.Count - 1], new Point(
                        control.Location.X + control.Width - control.Padding.Left,
                       control.Location.Y + control.Padding.Top
                        ));
                else SetControlLocation(control.Controls[0], new Point(
                            0,
                          0
                            ));
            if (control.RightToLeft == RightToLeft.Yes)
                for (int i = control.Controls.Count - 1; i > 0; i--)
                    if ((control.Controls[i].Location.X +
                     control.Controls[i].Width + +control.Controls[i].Margin.Right +
                   control.Controls[i - 1].Width + control.Controls[i - 1].Margin.Left + control.Controls[i - 1].Margin.Right)
                   < 0)
                        SetControlLocation(control.Controls[i - 1], new Point(
                             control.Controls[i].Location.X - control.Controls[i].Margin.Left,
                            control.Controls[i].Location.Y + control.Controls[i].Height + control.Controls[i].Margin.Bottom
                             ));
                    else SetControlLocation(control.Controls[i - 1], new Point(
                        control.Controls[i].Location.X + control.Controls[i].Width + control.Controls[i].Margin.Right,
                       control.Controls[i].Location.Y - control.Controls[i].Margin.Top
                        ));
            else for (int i = 0; i < control.Controls.Count - 1; i++)
                    if ((control.Controls[i].Location.X +
                         control.Controls[i].Width + +control.Controls[i].Margin.Right +
                       control.Controls[i + 1].Width + control.Controls[i + 1].Margin.Left + control.Controls[i + 1].Margin.Right)
                       > control.Width)
                        SetControlLocation(control.Controls[i + 1], new Point(
                        control.Controls[i].Location.X - control.Controls[i].Margin.Left,
                       control.Controls[i].Location.Y + control.Controls[i].Height + control.Controls[i].Margin.Bottom
                        ));
                    else SetControlLocation(control.Controls[i + 1], new Point(
                        control.Controls[i].Location.X + control.Controls[i].Width + control.Controls[i].Margin.Right,
                       control.Controls[i].Location.Y - control.Controls[i].Margin.Top
                        ));
            control.ResumeLayout(true);
        }
        public static void ClearEventHandlers( object obj,params string[] eventNames)
        {
            Type t = obj.GetType();
            foreach (var eventName in eventNames)
                try
                {
                    EventInfo ei = GetEvent(t, eventName);
                    if (ei != null)
                        ei.GetRemoveMethod(true).Invoke(obj, new object[] { null });
                }
                catch { }
        }
        public static EventInfo GetEvent( Type type, string eventName)
        {
            EventInfo ei = null;
            while (type != null)
            {
                ei = type.GetEvent(eventName);
                if (ei != null)
                    break;
                type = type.BaseType;
            }
            return ei;
        }

        #region Panel

        #endregion

        #region TextBox
        public static void ToggleSelectSeries(Series dp)
        {
            if (dp.BorderWidth < 4) SelectSeries(dp);
            else DeselectSeries(dp);
        }
        public static void SelectSeries(Series dp)
        {
            dp.BorderWidth *= 4;
        }
        public static void DeselectSeries(Series dp)
        {
            dp.BorderWidth /= 4;
        }
        public static void ToggleShowSeries(Series dp)
        {
            if (dp.Color == Color.Transparent) ShowSeries(dp);
            else if (dp.Color == Color.Empty) HideSeries(dp);
        }
        public static void HideSeries(Series dp)
        {
            dp.Color = Color.Transparent;
        }
        public static void ShowSeries(Series dp)
        {
            dp.Color = Color.Empty;
        }
        public static bool RemoveSeries(Chart chart, Series series)
        {
            return chart.Series.Remove(series);
        }
        public static bool RemoveSeries(Chart chart, int series)
        {
            if (chart.Series.Count > series)
            {
                chart.Series.RemoveAt(series);
                return true;
            }
            return false;
        }
        public static bool RemoveSeries(Chart chart, string series)
        {
            if (chart.Series.Any(v=>v.Name== series))
                return chart.Series.Remove(chart.Series[series]);
            return false;
        }

        #endregion

        #region RichTextBox
        public static RichTextBox RichTextBoxAppendWithStyle(ref RichTextBox rtb, string text, Color foreColor, HorizontalAlignment ha)
        {
            bool sc = !rtb.Focused || rtb.GetPositionFromCharIndex(rtb.GetFirstCharIndexOfCurrentLine()).Y >= rtb.PreferredSize.Height- rtb.Height-100;
            var st = rtb.TextLength;
            rtb.SelectionStart = st;
            rtb.SelectionColor = foreColor;
            rtb.SelectedText = text;
            rtb.SelectionStart = st + 1;
            rtb.SelectionLength = text.Length;
            rtb.SelectionAlignment = ha;
            rtb.SelectionStart = rtb.TextLength;
            if (sc) try { rtb.ScrollToCaret(); } catch { }
            return rtb;
        }
        public static RichTextBox RichTextBoxAppendWithStyle(ref RichTextBox rtb, string text, Color foreColor)
        {
            bool sc = !rtb.Focused || rtb.AutoScrollOffset.Y >= rtb.PreferredSize.Height - rtb.Height - 100;
            rtb.SelectionStart = rtb.TextLength + 1;
            rtb.SelectionLength = text.Length;
            rtb.SelectionColor = foreColor;
            rtb.SelectedText = text;
            if (sc) rtb.ScrollToCaret();
            return rtb;
        }
        public static RichTextBox RichTextBoxChangeWordColor(ref RichTextBox rtb, string startWord, string endWord, Color foreColor)
        {
            return RichTextBoxChangePositionColor(ref rtb,foreColor, StringService.WordsIndecesBetween(rtb.Text, startWord, endWord, true).ToArray());
        }
        public static RichTextBox RichTextBoxChangeWordColor(ref RichTextBox rtb, string word, Color foreColor)
        {
            return RichTextBoxChangeWordColor(ref rtb,word,"",foreColor);
        }
        public static RichTextBox RichTextBoxAppendWithStyle(ref RichTextBox rtb, string text, Color foreColor, Color backColor, HorizontalAlignment ha)
        {
            bool sc = !rtb.Focused || rtb.AutoScrollOffset.Y >= rtb.PreferredSize.Height - rtb.Height - 100;
            var st = rtb.TextLength;
            rtb.SelectionStart = st;
            rtb.SelectionLength = text.Length;
            rtb.SelectionColor = foreColor;
            rtb.SelectionBackColor = backColor;
            rtb.SelectedText = text;
            rtb.SelectionStart = st + 1;
            rtb.SelectionLength = text.Length;
            rtb.SelectionAlignment = ha;
            rtb.SelectionStart = rtb.TextLength;
            if(sc) rtb.ScrollToCaret();
            return rtb;
        }
        public static RichTextBox RichTextBoxAppendWithStyle(ref RichTextBox rtb, string text, Color foreColor, Color backColor)
        {
            int st = rtb.TextLength;
            rtb.SelectionStart = st+1;
            rtb.SelectionLength = text.Length;
            rtb.SelectionColor = foreColor;
            rtb.SelectionBackColor = backColor;
            rtb.SelectedText = text;
            rtb.ScrollToCaret();
            return rtb;
        }
        public static RichTextBox RichTextBoxChangeWordColor(ref RichTextBox rtb, string startWord, string endWord, Color foreColor, Color backColor)
        {
            return RichTextBoxChangePositionColor(ref rtb,foreColor,backColor, StringService.WordsIndecesBetween(rtb.Text, startWord, endWord, true).ToArray());
        }
        public static RichTextBox RichTextBoxChangeWordColor(ref RichTextBox rtb, string word, Color foreColor, Color backColor)
        {
            return RichTextBoxChangeWordColor(ref rtb, word, "", foreColor, backColor);
        }
        public static RichTextBox RichTextBoxChangePositionColor(ref RichTextBox rtb, Color foreColor, params Point[] points)
        {
            rtb.SuspendLayout();
            Point scroll = rtb.AutoScrollOffset;
            int slct = rtb.SelectionIndent;
            int ss = rtb.SelectionStart;
            foreach (var item in points)
            {
                rtb.SelectionStart = item.X;
                rtb.SelectionLength = item.Y - item.X;
                rtb.SelectionColor = foreColor;
            }
            rtb.SelectionStart = ss;
            rtb.SelectionIndent = slct;
            rtb.AutoScrollOffset = scroll;
            rtb.ResumeLayout(true);
            return rtb;
        }
        public static RichTextBox RichTextBoxChangePositionColor(ref RichTextBox rtb, Color foreColor, Color backColor, params Point[] points)
        {
            rtb.SuspendLayout();
            Point scroll = rtb.AutoScrollOffset;
            int slct = rtb.SelectionIndent;
            int ss = rtb.SelectionStart;
            foreach (var item in points)
            {
                rtb.SelectionStart = item.X;
                rtb.SelectionLength = item.Y - item.X;
                rtb.SelectionColor = foreColor;
                rtb.SelectionBackColor = backColor;
            }
            rtb.SelectionStart = ss;
            rtb.SelectionIndent = slct;
            rtb.AutoScrollOffset = scroll;
            rtb.ResumeLayout(true);
            return rtb;
        }
        public static RichTextBox RichTextBoxDequeueLinesToFile(RichTextBox rtb, string path, int num = -1)
        {
            if (rtb.TextLength > 0)
            try
            {
                if (!File.Exists(path)) File.CreateText(path);
                if (num < 1)
                {
                    File.AppendAllLines(path, rtb.Lines);
                    rtb.Text = "";
                }
                else if(rtb.Lines.Length > num)
                {
                    var l = rtb.Lines.Length - num;
                    File.AppendAllLines(path, rtb.Lines.Take(l));
                    rtb.Lines = rtb.Lines.Skip(num).ToArray();
                    //var h = Regex.Match(rtb.Rtf, "(^[\\{\\\\]([\\s\\S](?!^.*[^\\\\\\{]))+\\n?)+", RegexOptions.Multiline).Value;
                    //rtb.Rtf = string.Join(Environment.NewLine,new string[] {h}.Concat( rtb.Rtf.Replace(h, "").Split(new string[] { Environment.NewLine, "\n" }, StringSplitOptions.None).Skip(l+1)));
                }
            }
            catch (Exception ex) { }
            return rtb;
        }
        public static RichTextBox RichTextBoxDequeueLines(RichTextBox rtb, int num = -1)
        {
            if (rtb.TextLength > 0)
                try
                {
                    if (num < 1)
                        rtb.Text = "";
                    else
                    {
                        while (rtb.Lines.Length <= num) num--;
                        rtb.Lines = rtb.Lines.Skip(num).ToArray();
                        //var h = Regex.Match(rtb.Rtf, "(^[\\{\\\\]([\\s\\S](?!^.*[^\\\\\\{]))+\\n?)+", RegexOptions.Multiline).Value;
                        //rtb.Rtf = h + string.Join(Environment.NewLine, rtb.Rtf.Replace(h, "").Split(new string[] { Environment.NewLine, "\n" }, StringSplitOptions.None).Skip(num));
                    }
                }
                catch (Exception ex) { }
            return rtb;
        }
        #endregion

        #region Form
        public static void ShowFormIntoPanel(ref Panel panel, Form form)
        {
            panel.SuspendLayout();
            form.SuspendLayout();
            form.FormBorderStyle = FormBorderStyle.None;
            form.TopLevel = false;
            form.WindowState = FormWindowState.Normal;
            form.Size = panel.Size;
            form.Dock = DockStyle.Fill;
            panel.Controls.Add(form);
            form.BringToFront();
            form.ResumeLayout();
            panel.ResumeLayout();
            form.PerformLayout();
            panel.PerformLayout();
            form.Show();
        }
        public static void ShowFormIntoForm(ref Form main, Form form)
        {
            main.SuspendLayout();
            form.SuspendLayout();
            form.FormBorderStyle = FormBorderStyle.None;
            form.TopLevel = false;
            form.WindowState = FormWindowState.Normal;
            form.Size = main.Size;
            form.Dock = DockStyle.Fill;
            main.Controls.Add(form);
            form.BringToFront();
            form.ResumeLayout();
            main.ResumeLayout();
            form.PerformLayout();
            main.PerformLayout();
            form.Show();
        }
        public static void ShowFormIntoPanel(Panel panel, Form form)
        {
            ControlService.SetControlThreadSafe(panel,
           new Action<object[]>((ll) =>
           {
               panel.SuspendLayout();
               form.SuspendLayout();
               form.FormBorderStyle = FormBorderStyle.None;
               form.WindowState = FormWindowState.Normal;
               form.TopLevel = false;
               form.Size = panel.Size;
               form.Dock = DockStyle.Fill;
               panel.Controls.Add(form);
               form.BringToFront();
               form.ResumeLayout();
               panel.ResumeLayout();
               form.PerformLayout();
               panel.PerformLayout();
               form.Show();
           })
          , new object[] { });
        }
        public static void ShowFormIntoForm(Form main, Form form)
        {
            main.SuspendLayout();
            form.SuspendLayout();
            form.FormBorderStyle = FormBorderStyle.None;
            form.WindowState = FormWindowState.Normal;
            form.TopLevel = false;
            form.Size = main.Size;
            form.Dock = DockStyle.Fill;
            main.Controls.Add(form);
            form.BringToFront();
            form.ResumeLayout();
            main.ResumeLayout();
            form.PerformLayout();
            main.PerformLayout();
            form.Show();
        }
#endregion

#region IO
        public static string SaveAddress(string name, params string[] extentions)
        {
           var sfd = new SaveFileDialog();
            if (extentions != null && extentions.Length > 0)
                sfd.Filter = "All Supported Formats (*."+CollectionService.GetAllItems(extentions,", *.") + ") |*."
                    + CollectionService.GetAllItems(extentions, "; *.");
            sfd.RestoreDirectory =true;
            sfd.CheckFileExists = false;
            sfd.CheckPathExists = true;
            sfd.AddExtension = true;
            if (sfd.ShowDialog() == DialogResult.OK) return sfd.FileName;
            return null;
        }
        public static string OpenAddress(string name, params string[] extentions)
        {
            var ofd = new OpenFileDialog();
            if (extentions != null && extentions.Length > 0)
                ofd.Filter = "All Supported Formats (*." + CollectionService.GetAllItems(extentions, ", *.") + ") |*."
                    + CollectionService.GetAllItems(extentions, "; *.");
            ofd.RestoreDirectory = true;
            ofd.CheckFileExists = true;
            ofd.CheckPathExists = true;
            if (ofd.ShowDialog() == DialogResult.OK) return ofd.FileName;
            return null;
        }

#endregion

#region DataGridView
        public static void DataGridViewToProfessional(DataGridView dgv, bool createEvent = true, bool resetCMS = false, TextBox searchBox = null, ComboBox columnNamesBox = null, bool collapsedColumns = true, bool onlyVisibled = false, bool selectionItems = true, bool editItems = false, bool exportItems = true, bool rowsNumber = false)
        {
            if(createEvent) dgv.CellMouseDown += DataGridViewSelectWithMouseDown;
            if (resetCMS) dgv.ContextMenuStrip = null;
            if (searchBox != null)
            {
                if (columnNamesBox == null)
                    TextBoxConvertToDataGridViewSearchBox(searchBox, dgv, createEvent);
                else TextBoxAndComboBoxConvertToDataGridViewSearchBox(searchBox, columnNamesBox, dgv, createEvent);
            }
            if (collapsedColumns) DataGridViewCollapsedCollumnsCMS(dgv, onlyVisibled);
            if (selectionItems) DataGridViewSelectionRowsCMS(dgv);
            if (editItems) DataGridViewEditCMS(dgv);
            if (exportItems) DataGridViewExportCMS(dgv);
            if (rowsNumber) DataGridViewAutoSetRowsNumber(dgv);
        }
        public static void DataGridViewToProfessional(DataGridView dgv, TextBox searchBox, bool collapsedColumns = true, bool onlyVisibled = false, bool selectionItems = true, bool editItems = false, bool exportItems = true, bool createEvent = true, bool resetCMS = false, bool rowsNumber = false)
        {
            DataGridViewToProfessional( dgv,createEvent, resetCMS,  searchBox,null,  collapsedColumns,  onlyVisibled , selectionItems ,editItems , exportItems,rowsNumber);
        }
        public static void DataGridViewNormalization(ref DataGridView dgv)
        {
            for (int i = 0; i < dgv.Columns.Count; i++)
                try {
                    dgv.Columns[i].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    dgv.Columns[i].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                    dgv.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                } catch { }
        }
        public static void DataGridViewAutoSetRowsNumber(DataGridView dgv)
        {
            dgv.RowHeadersVisible = true;
            dgv.AutoResizeRowHeadersWidth(DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders);
            dgv.RowEnter += (s, e) =>
            {
                if (dgv.Visible)
                    dgv.Rows[e.RowIndex].HeaderCell.Value = (e.RowIndex + 1).ToString();
            };
        }
        public static void DataGridViewSelectWithMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;
            if (e.RowIndex>-1 && e.Button == MouseButtons.Right && !dgv.Rows[e.RowIndex].Selected)
                try
                {
                    dgv.ClearSelection();
                    dgv.Rows[e.RowIndex].Selected = true;
                    dgv.Focus();
                }
                catch { }
        }
        public static void DataGridViewTranslate(ref DataGridView dgv, ToStringBase displayer = null)
        {
            dgv.SuspendLayout();
            if (dgv.DataSource == null) return;
            DataTable dt = DataTableTranslate((DataTable)dgv.DataSource);
            dgv.DataSource = dt;
            DataGridViewHeaderTranslate(ref dgv, displayer);
            dgv.Update();
            dgv.ResumeLayout(true);
        }
        public static void DataGridViewHeaderTranslate(ref DataGridView dgv, ToStringBase displayer = null)
        {
            if (displayer == null)
            {
                displayer = new ToText();
                if (displayer.PointerSign.Contains("<") || displayer.PointerSign.Contains(">")) displayer.PointerSign = ": ";
                displayer.AllowBitmap = false;
            }
            displayer.Translate = true;
            if (displayer.PointerSign.Contains("<") || displayer.PointerSign.Contains(">")) displayer.PointerSign = ": ";
            for (int i = 0; i < dgv.Columns.Count; i++)
                try { dgv.Columns[i].HeaderText = displayer.Done(dgv.Columns[i].HeaderText); } catch { }
        }

        public static void DataGridViewFilter(DataGridView dgv, string search, string collName = null)
        {
            bool noCol = true;
            try { noCol = dgv.Columns[collName].Index < 0; } catch { }
            ControlService.StartThread(ThreadingMethodMode.Default,
                    new Action<object[]>((o) =>
                    {
                        ControlService.SetControlThreadSafe(dgv, new Action<object[]>((oa) =>
                        {
                            search = (search + "").ToLower();
                            if (string.IsNullOrEmpty(search))
                                for (int r = dgv.Rows.Count - 1; r >= 0; r--)
                                {
                                    dgv.Rows[r].DefaultCellStyle.BackColor = dgv.DefaultCellStyle.BackColor;
                                    dgv.Rows[r].DefaultCellStyle.ForeColor = dgv.DefaultCellStyle.ForeColor;
                                    dgv.Rows[r].Visible = true;
                                }
                            else if (noCol)
                            {
                                for (int r = dgv.Rows.Count - 1; r >= 0; r--)
                                    for (int c = 0; c < dgv.Columns.Count; c++)
                                        if (dgv.Rows[r].Cells[c].ValueType.IsAssignableFrom(typeof(string)))
                                            if ((dgv.Rows[r].Cells[c].Value + "").ToLower().Contains(search))
                                            {
                                                dgv.Rows[r].DefaultCellStyle.BackColor = dgv.DefaultCellStyle.BackColor;
                                                dgv.Rows[r].DefaultCellStyle.ForeColor = dgv.DefaultCellStyle.ForeColor;
                                                dgv.Rows[r].Visible = true;
                                                break;
                                            }
                                            else
                                            {
                                                if (c == dgv.Columns.Count - 1)
                                                    dgv.Rows[r].Visible = false;
                                            }
                                        else
                                        {
                                            if (c == dgv.Columns.Count - 1)
                                                dgv.Rows[r].Visible = false;
                                        }
                            }
                            else for (int r1 = dgv.Rows.Count - 1; r1 >= 0; r1--)
                                    if (dgv.Rows[r1].Cells[collName].ValueType.IsAssignableFrom(typeof(string)))
                                    {
                                        if ((dgv.Rows[r1].Cells[collName].Value + "").ToLower().Contains(search))
                                        {
                                            dgv.Rows[r1].DefaultCellStyle.BackColor = dgv.DefaultCellStyle.BackColor;
                                            dgv.Rows[r1].DefaultCellStyle.ForeColor = dgv.DefaultCellStyle.ForeColor;
                                            dgv.Rows[r1].Visible = true;
                                        }
                                        else dgv.Rows[r1].Visible = false;
                                    }
                                    else dgv.Rows[r1].Visible = false;
                        }), new object[] { });
                    }), new object[] { });
        }
        public static void DataGridViewSearch(DataGridView dgv, string search, string collName = null)
        {
            bool noCol = true;
            try { noCol = dgv.Columns[collName].Index < 0; } catch { }
            ControlService.StartThread(ThreadingMethodMode.Default,
             new Action<object[]>((o) =>
             {
                 ControlService.SetControlThreadSafe(dgv, new Action<object[]>((oa) =>
                 {
                     search = (search + "").ToLower();
                     if (string.IsNullOrEmpty(search))
                         for (int r = dgv.Rows.Count - 1; r >= 0; r--)
                         {
                             dgv.Rows[r].DefaultCellStyle.BackColor = dgv.DefaultCellStyle.BackColor;
                             dgv.Rows[r].DefaultCellStyle.ForeColor = dgv.DefaultCellStyle.ForeColor;
                             dgv.Rows[r].Visible = true;
                         }
                     else if (noCol)
                     {
                         for (int r = dgv.Rows.Count - 1; r >= 0; r--)
                             for (int c = 0; c < dgv.Columns.Count; c++)
                                 if (dgv.Rows[r].Cells[c].ValueType.IsAssignableFrom(typeof(string)))
                                 {
                                     if ((dgv.Rows[r].Cells[c].Value + "").ToLower().Contains(search))
                                     {
                                         DataGridViewRowHilight(dgv,r);
                                         break;
                                     }
                                     else if (c == dgv.Columns.Count - 1)
                                     {
                                         dgv.Rows[r].DefaultCellStyle.BackColor = dgv.DefaultCellStyle.BackColor;
                                         dgv.Rows[r].DefaultCellStyle.ForeColor = dgv.DefaultCellStyle.ForeColor;
                                     }
                                 }
                                 else if (c == dgv.Columns.Count - 1)
                                 {
                                     dgv.Rows[r].DefaultCellStyle.BackColor = dgv.DefaultCellStyle.BackColor;
                                     dgv.Rows[r].DefaultCellStyle.ForeColor = dgv.DefaultCellStyle.ForeColor;
                                 }
                     }
                     else for (int r1 = dgv.Rows.Count - 1; r1 >= 0; r1--)
                             if (dgv.Rows[r1].Cells[collName].ValueType.IsAssignableFrom(typeof(string)))
                             {
                                 if ((dgv.Rows[r1].Cells[collName].Value + "").ToLower().Contains(search))
                                     DataGridViewRowHilight(dgv, r1);
                                 else
                                 {
                                     dgv.Rows[r1].DefaultCellStyle.BackColor = dgv.DefaultCellStyle.BackColor;
                                     dgv.Rows[r1].DefaultCellStyle.ForeColor = dgv.DefaultCellStyle.ForeColor;
                                 }
                             }
                             else
                             {
                                 dgv.Rows[r1].DefaultCellStyle.BackColor = dgv.DefaultCellStyle.BackColor;
                                 dgv.Rows[r1].DefaultCellStyle.ForeColor = dgv.DefaultCellStyle.ForeColor;
                             }
                 }), new object[] { });
             }), new object[] { });
        }
        public static void DataGridViewSearch(DataGridView dgv, Func<string,bool> searchFunc, string collName = null)
        {
            bool noCol = true;
            try { noCol = dgv.Columns[collName].Index < 0; } catch { }
            ControlService.StartThread(ThreadingMethodMode.Default,
             new Action<object[]>((o) =>
             {
                 ControlService.SetControlThreadSafe(dgv, new Action<object[]>((oa) =>
                 {
                     if (noCol)
                     {
                         for (int r = dgv.Rows.Count - 1; r >= 0; r--)
                             for (int c = 0; c < dgv.Columns.Count; c++)
                                 if (dgv.Rows[r].Cells[c].ValueType.IsAssignableFrom(typeof(string)))
                                 {
                                     if (searchFunc(dgv.Rows[r].Cells[c].Value + ""))
                                     {
                                         DataGridViewRowHilight(dgv,r);
                                         break;
                                     }
                                     else if (c == dgv.Columns.Count - 1)
                                     {
                                         dgv.Rows[r].DefaultCellStyle.BackColor = dgv.DefaultCellStyle.BackColor;
                                         dgv.Rows[r].DefaultCellStyle.ForeColor = dgv.DefaultCellStyle.ForeColor;
                                     }
                                 }
                                 else if (c == dgv.Columns.Count - 1)
                                 {
                                     dgv.Rows[r].DefaultCellStyle.BackColor = dgv.DefaultCellStyle.BackColor;
                                     dgv.Rows[r].DefaultCellStyle.ForeColor = dgv.DefaultCellStyle.ForeColor;
                                 }
                     }
                     else for (int r1 = dgv.Rows.Count - 1; r1 >= 0; r1--)
                             if (dgv.Rows[r1].Cells[collName].ValueType.IsAssignableFrom(typeof(string)))
                             {
                                 if (searchFunc(dgv.Rows[r1].Cells[collName].Value + ""))
                                     DataGridViewRowHilight(dgv, r1);
                                 else
                                 {
                                     dgv.Rows[r1].DefaultCellStyle.BackColor = dgv.DefaultCellStyle.BackColor;
                                     dgv.Rows[r1].DefaultCellStyle.ForeColor = dgv.DefaultCellStyle.ForeColor;
                                 }
                             }
                             else
                             {
                                 dgv.Rows[r1].DefaultCellStyle.BackColor = dgv.DefaultCellStyle.BackColor;
                                 dgv.Rows[r1].DefaultCellStyle.ForeColor = dgv.DefaultCellStyle.ForeColor;
                             }
                 }), new object[] { });
             }), new object[] { });
        }
        public static void DataGridViewSearch(DataGridView dgv, string search, DataGridViewCell dgvc,bool inCell = false)
        {
            ControlService.StartThread(ThreadingMethodMode.Default,
         new Action<object[]>((o) =>
         {
             ControlService.SetControlThreadSafe(dgv, new Action<object[]>((oa) =>
             {
                 search = (search + "").ToLower();
                 for (int r = dgv.Rows.Count - 1; r >= 0; r--)
                 {
                     dgv.Rows[r].DefaultCellStyle.BackColor = dgv.DefaultCellStyle.BackColor;
                     dgv.Rows[r].DefaultCellStyle.ForeColor = dgv.DefaultCellStyle.ForeColor;
                     dgv.Rows[r].Visible = true;
                 }
                 if (inCell)
                 {
                 }
                 else
                 {
                     int r = dgvc.RowIndex;
                     for (int c = 0; c < dgv.Columns.Count; c++)
                         if (dgv.Rows[r].Cells[c].ValueType.IsAssignableFrom(typeof(string)))
                         {
                             if ((dgv.Rows[r].Cells[c].Value + "").ToLower().Contains(search))
                             {
                                 DataGridViewRowHilight(dgv, r);
                                 break;
                             }
                             else
                             {
                                 dgv.Rows[r].DefaultCellStyle.BackColor = dgv.DefaultCellStyle.BackColor;
                                 dgv.Rows[r].DefaultCellStyle.ForeColor = dgv.DefaultCellStyle.ForeColor;
                             }
                         }
                         else
                         {
                             dgv.Rows[r].DefaultCellStyle.BackColor = dgv.DefaultCellStyle.BackColor;
                             dgv.Rows[r].DefaultCellStyle.ForeColor = dgv.DefaultCellStyle.ForeColor;
                         }
                 }
             }), new object[] { });
         }), new object[] { });
        }
        public static void DataGridViewSearch(DataGridView dgv, Func<string, bool> searchFunc, DataGridViewCell dgvc,bool inCell = false)
        {
            ControlService.StartThread(ThreadingMethodMode.Default,
         new Action<object[]>((o) =>
         {
             ControlService.SetControlThreadSafe(dgv, new Action<object[]>((oa) =>
             {
                 if (inCell)
                 {
                 }
                 else
                 {
                     int r = dgvc.RowIndex;
                     for (int c = 0; c < dgv.Columns.Count; c++)
                         if (dgv.Rows[r].Cells[c].ValueType.IsAssignableFrom(typeof(string)))
                         {
                             if (searchFunc(dgv.Rows[r].Cells[c].Value + ""))
                             {
                                 DataGridViewRowHilight(dgv, r);
                                 break;
                             }
                             else
                             {
                                 dgv.Rows[r].DefaultCellStyle.BackColor = dgv.DefaultCellStyle.BackColor;
                                 dgv.Rows[r].DefaultCellStyle.ForeColor = dgv.DefaultCellStyle.ForeColor;
                             }
                         }
                         else
                         {
                             dgv.Rows[r].DefaultCellStyle.BackColor = dgv.DefaultCellStyle.BackColor;
                             dgv.Rows[r].DefaultCellStyle.ForeColor = dgv.DefaultCellStyle.ForeColor;
                         }
                 }
             }), new object[] { });
         }), new object[] { });
        }
        public static void DataGridViewFind(DataGridView dgv, string search, DataGridViewCell dgvc, TableScope scope = TableScope.Table)
        {
            switch (scope)
            {
                case TableScope.Column:
                    DataGridViewSearch(dgv, search,dgvc.OwningColumn.Name);
                    break;
                case TableScope.Row:
                    DataGridViewSearch(dgv, search,dgvc,false);
                    break;
                case TableScope.Cell:
                    DataGridViewSearch(dgv, search,dgvc,true);
                    break;
                default:
                    DataGridViewSearch(dgv, search);
                    break;
            }
        }
        public static void DataGridViewFind(DataGridView dgv, Func<string, bool> searchFunc, DataGridViewCell dgvc, TableScope scope = TableScope.Table)
        {
            switch (scope)
            {
                case TableScope.Column:
                    DataGridViewSearch(dgv, searchFunc, dgvc.OwningColumn.Name);
                    break;
                case TableScope.Row:
                    DataGridViewSearch(dgv, searchFunc, dgvc,false);
                    break;
                case TableScope.Cell:
                    DataGridViewSearch(dgv, searchFunc, dgvc,true);
                    break;
                default:
                    DataGridViewSearch(dgv, searchFunc);
                    break;
            }
        }
        public static void DataGridViewFind(DataGridView dgv, string startFrom, string endTo, DataGridViewCell dgvc, TableScope scope = TableScope.Table)
        {
            Func<string, bool> searchFunc = (str) => 
            {
                if (str.StartsWith(startFrom))
                    for (int i = startFrom.Length; i < str.Length; i++)
                        if (str.Substring(i).StartsWith(endTo)) return true;
                return false;
            };
            DataGridViewFind(dgv, searchFunc, dgvc, scope);
        }
        public static DataGridViewCell DataGridViewFindNext(DataGridView dgv, string search, DataGridViewCell dgvc, TableScope scope = TableScope.Table)
        {
            search = (search + "").ToLower();
            if (string.IsNullOrEmpty(search)) return dgvc;
            int c = dgvc.ColumnIndex;
            int r = dgvc.RowIndex;
            switch (scope)
            {
                case TableScope.Column:
                    r++;
                    for (; r < dgv.Rows.Count; r++)
                        if (dgv.Rows[r].Cells[c].ValueType.IsAssignableFrom(typeof(string)))
                            if ((dgv.Rows[r].Cells[c].Value + "").ToLower().Contains(search))
                                return DataGridViewCellCurrent(dgv, r, c);
                    break;
                case TableScope.Row:
                    c++;
                    for (; c < dgv.Columns.Count; c++)
                        if (dgv.Rows[r].Cells[c].ValueType.IsAssignableFrom(typeof(string)))
                            if ((dgv.Rows[r].Cells[c].Value + "").ToLower().Contains(search))
                                return DataGridViewCellCurrent(dgv, r, c);
                    break;
                case TableScope.Cell:
                    break;
                default:
                    c++;
                    for (; r < dgv.Rows.Count; r++)
                    {
                        for (; c < dgv.Columns.Count; c++)
                            if (dgv.Rows[r].Cells[c].ValueType.IsAssignableFrom(typeof(string)))
                                if ((dgv.Rows[r].Cells[c].Value + "").ToLower().Contains(search))
                                    return DataGridViewCellCurrent(dgv, r, c);
                        c = 0;
                    }
                    break;
            }
            return dgvc;
        }
        public static DataGridViewCell DataGridViewFindBack(DataGridView dgv, string search, DataGridViewCell dgvc, TableScope scope = TableScope.Table)
        {
            search = (search + "").ToLower();
            if (string.IsNullOrEmpty(search)) return dgvc;
            int c = dgvc.ColumnIndex;
            int r = dgvc.RowIndex;
            switch (scope)
            {
                case TableScope.Column:
                    r--;
                    for (; r > -1; r--)
                        if (dgv.Rows[r].Cells[c].ValueType.IsAssignableFrom(typeof(string)))
                            if ((dgv.Rows[r].Cells[c].Value + "").ToLower().Contains(search))
                                return DataGridViewCellCurrent(dgv, r, c);
                    break;
                case TableScope.Row:
                    c--;
                    for (; c > -1; c--)
                        if (dgv.Rows[r].Cells[c].ValueType.IsAssignableFrom(typeof(string)))
                            if ((dgv.Rows[r].Cells[c].Value + "").ToLower().Contains(search))
                                return DataGridViewCellCurrent(dgv, r, c);
                    break;
                case TableScope.Cell:
                    break;
                default:
                    c--;
                    for (; r > -1; r--)
                    {
                        for (; c > -1; c--)
                            if (dgv.Rows[r].Cells[c].ValueType.IsAssignableFrom(typeof(string)))
                                if ((dgv.Rows[r].Cells[c].Value + "").ToLower().Contains(search))
                                    return DataGridViewCellCurrent(dgv, r, c);
                        c = dgv.Columns.Count - 1;
                    }
                    break;
            }
            return dgvc;
        }
        public static void DataGridViewReplace(DataGridView dgv, Func<object, object> replaceFunc, DataGridViewCell dgvc, TableScope scope = TableScope.Table)
        {
            switch (scope)
            {
                case TableScope.Column:
                    ControlService.StartThread(ThreadingMethodMode.Default,
                     new Action<object[]>((o) =>
                     {
                         ControlService.SetControlThreadSafe(dgv, new Action<object[]>((oa) =>
                         {
                             int c = dgvc.ColumnIndex;
                             for (int r = dgv.Rows.Count - 1; r >= 0; r--)
                                 if (dgv.Rows[r].Cells[c].ValueType.IsAssignableFrom(typeof(string)))
                                     dgv.Rows[r].Cells[c].Value = replaceFunc(dgv.Rows[r].Cells[c].Value);
                         }), new object[] { });
                     }), new object[] { });
                    break;
                case TableScope.Row:
                    ControlService.StartThread(ThreadingMethodMode.Default,
                     new Action<object[]>((o) =>
                     {
                         ControlService.SetControlThreadSafe(dgv, new Action<object[]>((oa) =>
                         {
                             int r = dgvc.RowIndex;
                             for (int c = 0; c < dgv.Columns.Count; c++)
                                 if (dgv.Rows[r].Cells[c].ValueType.IsAssignableFrom(typeof(string)))
                                         dgv.Rows[r].Cells[c].Value = replaceFunc(dgv.Rows[r].Cells[c].Value);
                         }), new object[] { });
                     }), new object[] { });
                    break;
                case TableScope.Cell:
                    ControlService.StartThread(ThreadingMethodMode.Default,
                     new Action<object[]>((o) =>
                     {
                         ControlService.SetControlThreadSafe(dgv, new Action<object[]>((oa) =>
                         {
                             if (dgvc.ValueType.IsAssignableFrom(typeof(string)))
                                     dgvc.Value = replaceFunc(dgvc.Value);
                         }), new object[] { });
                     }), new object[] { });
                    break;
                default:
                    ControlService.StartThread(ThreadingMethodMode.Default,
                     new Action<object[]>((o) =>
                     {
                         ControlService.SetControlThreadSafe(dgv, new Action<object[]>((oa) =>
                         {
                             for (int r = dgv.Rows.Count - 1; r >= 0; r--)
                                 for (int c = 0; c < dgv.Columns.Count; c++)
                                     if (dgv.Rows[r].Cells[c].ValueType.IsAssignableFrom(typeof(string)))
                                             dgv.Rows[r].Cells[c].Value = replaceFunc(dgv.Rows[r].Cells[c].Value);
                         }), new object[] { });
                     }), new object[] { });
                    break;
            }
        }
        public static void DataGridViewReplace(DataGridView dgv, string thisText, string withText, DataGridViewCell dgvc, TableScope scope = TableScope.Table)
        {
            thisText = (thisText + "").ToLower();
            if (string.IsNullOrEmpty(thisText)) return;
            switch (scope)
            {
                case TableScope.Column:
                    ControlService.StartThread(ThreadingMethodMode.Default,
                     new Action<object[]>((o) =>
                     {
                         ControlService.SetControlThreadSafe(dgv, new Action<object[]>((oa) =>
                         {
                             string text = "";
                             int c = dgvc.ColumnIndex;
                             for (int r = dgv.Rows.Count - 1; r >= 0; r--)
                                 if (dgv.Rows[r].Cells[c].ValueType.IsAssignableFrom(typeof(string)))
                                     if ((text = dgv.Rows[r].Cells[c].Value + "").ToLower().Contains(thisText))
                                         dgv.Rows[r].Cells[c].Value = text.Replace(thisText, withText);
                         }), new object[] { });
                     }), new object[] { });
                    break;
                case TableScope.Row:
                    ControlService.StartThread(ThreadingMethodMode.Default,
                     new Action<object[]>((o) =>
                     {
                         ControlService.SetControlThreadSafe(dgv, new Action<object[]>((oa) =>
                         {
                             string text = "";
                             int r = dgvc.RowIndex;
                             for (int c = 0; c < dgv.Columns.Count; c++)
                                 if (dgv.Rows[r].Cells[c].ValueType.IsAssignableFrom(typeof(string)))
                                     if ((text = dgv.Rows[r].Cells[c].Value + "").ToLower().Contains(thisText))
                                         dgv.Rows[r].Cells[c].Value = text.Replace(thisText, withText);
                         }), new object[] { });
                     }), new object[] { });
                    break;
                case TableScope.Cell:
                    ControlService.StartThread(ThreadingMethodMode.Default,
                     new Action<object[]>((o) =>
                     {
                         ControlService.SetControlThreadSafe(dgv, new Action<object[]>((oa) =>
                         {
                             string text = "";
                             if (dgvc.ValueType.IsAssignableFrom(typeof(string)))
                                 if ((text = dgvc.Value + "").ToLower().Contains(thisText))
                                     dgvc.Value = text.Replace(thisText, withText);
                         }), new object[] { });
                     }), new object[] { });
                    break;
                default:
                    ControlService.StartThread(ThreadingMethodMode.Default,
                     new Action<object[]>((o) =>
                     {
                         ControlService.SetControlThreadSafe(dgv, new Action<object[]>((oa) =>
                         {
                             string text = "";
                             for (int r = dgv.Rows.Count - 1; r >= 0; r--)
                                 for (int c = 0; c < dgv.Columns.Count; c++)
                                     if (dgv.Rows[r].Cells[c].ValueType.IsAssignableFrom(typeof(string)))
                                         if ((text = dgv.Rows[r].Cells[c].Value + "").ToLower().Contains(thisText))
                                             dgv.Rows[r].Cells[c].Value = text.Replace(thisText,withText);
                         }), new object[] { });
                     }), new object[] { });
                    break;
            }
        }
        public static DataGridViewCell DataGridViewReplaceNext(DataGridView dgv, string thisText, string withText, DataGridViewCell dgvc, TableScope scope = TableScope.Table)
        {
            thisText = (thisText + "").ToLower();
            if (string.IsNullOrEmpty(thisText)) return dgvc;
            string text = "";
            int c = dgvc.ColumnIndex;
            int r = dgvc.RowIndex;
            if ((text = dgv.Rows[r].Cells[c].Value + "").ToLower().Contains(thisText))
                dgv.Rows[r].Cells[c].Value = text.Replace(thisText, withText);
            return DataGridViewFindNext(dgv,thisText, dgvc,scope);
        }
        public static DataGridViewCell DataGridViewReplaceBack(DataGridView dgv, string thisText, string withText, DataGridViewCell dgvc, TableScope scope = TableScope.Table)
        {
            thisText = (thisText + "").ToLower();
            if (string.IsNullOrEmpty(thisText)) return dgvc;
            string text = "";
            int c = dgvc.ColumnIndex;
            int r = dgvc.RowIndex;
            if ((text = dgv.Rows[r].Cells[c].Value + "").ToLower().Contains(thisText))
                dgv.Rows[r].Cells[c].Value = text.Replace(thisText, withText);
            return DataGridViewFindBack(dgv, thisText, dgvc, scope);
        }

        public static DataGridViewCell DataGridViewCellCurrent(DataGridView dgv,int rowIndex,int colIndex)
        {
            dgv.ClearSelection();
            dgv.CurrentCell = dgv.Rows[rowIndex].Cells[colIndex];
            dgv.BeginEdit(false);
            return dgv.CurrentCell;
        }
        public static DataGridViewCell DataGridViewCellHilight(DataGridView dgv,int rowIndex, int colIndex)
        {
            dgv.Rows[rowIndex].Cells[colIndex].Style.BackColor = Color.LightYellow;
            dgv.Rows[rowIndex].Cells[colIndex].Style.ForeColor = Color.DarkRed;
            return dgv.Rows[rowIndex].Cells[colIndex];
        }
        public static DataGridViewRow DataGridViewRowHilight(DataGridView dgv,int rowIndex)
        {
            dgv.Rows[rowIndex].DefaultCellStyle.BackColor = Color.LightYellow;
            dgv.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.DarkRed;
            return dgv.Rows[rowIndex];
        }
        public static void DataGridViewDataSource(DataGridView dgv, DataTable dt)
        {
            dgv.SuspendLayout();
            dgv.DataSource = null;
            dgv.Columns.Clear();
            dgv.AutoGenerateColumns = true;
            dgv.DataSource = dt;
            dgv.Update();
            dgv.ResumeLayout(true);
        }
        public static void DataGridViewDataSource(DataGridView dgv, DataSet ds)
        {
            dgv.SuspendLayout();
            dgv.Columns.Clear();
            dgv.AutoGenerateColumns = true;
            dgv.DataSource = ds;
            dgv.ResumeLayout(true);
        }
        public static void DataGridViewCollapsedCollumnsCMS(DataGridView dgv, bool onlyVisibled = false)
        {
            var tsm = new System.Windows.Forms.ToolStripMenuItem("Collumns", Properties.Resources.columns_black);
            if (onlyVisibled)
            {
                foreach (DataGridViewColumn item in dgv.Columns)
                    if (item.Visible) tsm.DropDownItems.Add(new ToolStripMenuItem(item.HeaderText, null, (o, e) => dgv.Columns[item.Name].Visible = ((ToolStripMenuItem)o).Checked = !((ToolStripMenuItem)o).Checked) { Checked = item.Visible, DoubleClickEnabled = true });
            }
            else foreach (DataGridViewColumn item in dgv.Columns)
                    tsm.DropDownItems.Add(new ToolStripMenuItem(item.HeaderText, null, (o, e) => dgv.Columns[item.Name].Visible = ((ToolStripMenuItem)o).Checked = !((ToolStripMenuItem)o).Checked) { Checked = item.Visible, DoubleClickEnabled = true });
            if (dgv.ContextMenuStrip == null) dgv.ContextMenuStrip = new ContextMenuStrip();
            else dgv.ContextMenuStrip.Items.Add(new ToolStripSeparator());
            dgv.ContextMenuStrip.Items.Add(tsm);
        }
        public static void DataGridViewEditCMS( DataGridView dgv)
        {
            //var tsm = new System.Windows.Forms.ToolStripMenuItem();
            //tsm.Text = "Selection";
            //if (onlyVisibled) foreach (DataGridViewColumn item in dgv.Columns)
            //        tsm.DropDownItems.Add(new ToolStripMenuItem(item.HeaderText, null, (o, e) => dgv.Columns[item.Name].Visible = ((ToolStripMenuItem)o).Checked = !((ToolStripMenuItem)o).Checked) { Checked = item.Visible, DoubleClickEnabled = true });
            //if (dgv.ContextMenuStrip == null) dgv.ContextMenuStrip = new ContextMenuStrip();
            //else dgv.ContextMenuStrip.Items.Add(new ToolStripSeparator());
            //dgv.ContextMenuStrip.Items.Add(tsm);
        }
        public static void DataGridViewSelectionRowsCMS(DataGridView dgv)
        {
            if (dgv.MultiSelect)
            {
                if (dgv.ContextMenuStrip == null) dgv.ContextMenuStrip = new ContextMenuStrip();
                dgv.ContextMenuStrip.Items.AddRange(new ToolStripMenuItem []{
                    new ToolStripMenuItem("Select All", Properties.Resources.Tick, (o, e) => dgv.SelectAll()),
                new ToolStripMenuItem("DeSelect All", Properties.Resources.Cancel, (o, e) => dgv.ClearSelection())});
            }
        }
        public static void DataGridViewExportCMS(DataGridView dgv)
        {
            var tsm = new System.Windows.Forms.ToolStripMenuItem("Export", Properties.Resources.Export);
            tsm.DropDownItems.Add(new ToolStripMenuItem("Microsoft Excel", Properties.Resources.Microsoft_Excel, (o, e) => DataGridViewToExcel( dgv)));
            if (dgv.ContextMenuStrip == null) dgv.ContextMenuStrip = new ContextMenuStrip();
            else dgv.ContextMenuStrip.Items.Add(new ToolStripSeparator());
            dgv.ContextMenuStrip.Items.Add(tsm);
        }
        public static void DataGridViewToExcel(DataGridView dgv, bool openAfter = true)
        {
            string addr = SaveAddress("My Export", "xlsx");
            if (!string.IsNullOrEmpty(addr)) ConvertService.ToExcelFile(dgv, addr, "MiMFa Export", openAfter);
        }
        public static void DataGridViewRowsOrder(DataGridView dgv, int number, params int[] rowIndices)
        {
            if (dgv.AllowUserToAddRows && dgv.Rows.Count < 2) return;
            var count = dgv.Rows.Count - (dgv.AllowUserToAddRows ? 1 : 0);
            if (rowIndices.Length == 0)
                foreach (DataGridViewRow item in dgv.SelectedRows)
                    rowIndices = rowIndices.Concat(new int[] { item.Index}).ToArray();
            dgv.ClearSelection();
            for (int i = 0; i < rowIndices.Length; i++)
            {
                if (rowIndices[i] < 0 || rowIndices[i] > count) continue;
                var row = dgv.Rows[rowIndices[i]];
                dgv.Rows.Remove(row);
                dgv.Rows.Insert(Math.Max(0, Math.Min(count-1, rowIndices[i] + number)), row);
                row.Selected = true;
            }
        }


        public static void TextBoxConvertToDataGridViewSearchBox( TextBox tb,  DataGridView dgv, bool createEvent = true)
        {
            if(!createEvent) return;
            EventHandler eh = (o, e) =>
            {
                ControlService.DataGridViewSearch(dgv, tb.Text);
            };
            KeyPressEventHandler kpeh = (o, e) =>
            {
                StartThreadInControl(ThreadingMethodMode.Default, dgv,
                new Action<object[]>((arg) =>
                {
                    if ((Keys)e.KeyChar == Keys.Enter)
                        ControlService.DataGridViewFilter(dgv, tb.Text);
                    else if ((Keys)e.KeyChar == Keys.Escape)
                        ControlService.DataGridViewFilter(dgv, null);
                    else eh(tb,EventArgs.Empty);
                }), new object[] { });
            };
             tb.KeyPress += kpeh;
        }
        public static void TextBoxAndComboBoxConvertToDataGridViewSearchBox(TextBox tb, ComboBox cb, DataGridView dgv, bool createEvent = true)
        {
            cb.Items.Clear();
            cb.DisplayMember = "Value";
            cb.Items.Add(new KeyValuePair<string, string>("", "*"));
            foreach (DataGridViewColumn item in dgv.Columns)
                if (item.Visible) cb.Items.Add(new KeyValuePair<string, string>(item.Name, item.HeaderText));
            if (!createEvent) return;
            EventHandler eh = (o, e) =>
                          {
                              string col = null;
                              if (cb.SelectedIndex > 0)
                                  col = ((KeyValuePair<string, string>)cb.SelectedItem).Key;
                              ControlService.DataGridViewSearch(dgv, tb.Text, col);
                          };
            KeyPressEventHandler kpeh = (o, e) =>
            {
                string col = null;
                if (cb.SelectedIndex > 1)
                    col = ((KeyValuePair<string, string>)cb.SelectedItem).Key;
                StartThreadInControl(ThreadingMethodMode.Default, dgv,
                new Action<object[]>((arg) =>
                {
                    if ((Keys)e.KeyChar == Keys.Enter)
                        ControlService.DataGridViewFilter(dgv, tb.Text, col);
                    else if ((Keys)e.KeyChar == Keys.Escape)
                        ControlService.DataGridViewFilter(dgv, null);
                    else ControlService.DataGridViewSearch(dgv, tb.Text, col);
                }), new object[] { });
            };
            tb.KeyPress += kpeh;
            cb.SelectedIndexChanged += eh;
        }
        public static SortedDictionary<int, DataGridViewRow> DataGridViewRemoveRows(DataGridView dgv, DataGridViewSelectedRowCollection dgvrc)
        {
            DataGridViewRow[] dgvr = new DataGridViewRow[dgvrc.Count];
            for (int i = 0; i < dgvrc.Count; i++)
                dgvr[i] = dgvrc[i];
            return DataGridViewRemoveRows(dgv, dgvr);
        }
        public static SortedDictionary<int, DataGridViewRow> DataGridViewRemoveRows(DataGridView dgv, DataGridViewRowCollection dgvrc)
        {
            DataGridViewRow[] dgvr = new DataGridViewRow[dgvrc.Count];
            for (int i = 0; i < dgvrc.Count; i++)
                dgvr[i] = dgvrc[i];
            return DataGridViewRemoveRows(dgv, dgvr);
        }
        public static SortedDictionary<int, DataGridViewRow> DataGridViewRemoveRows(DataGridView dgv, params DataGridViewRow[] dgvrc)
        {
            SortedDictionary<int, DataGridViewRow> rows = new SortedDictionary<int, DataGridViewRow>();
            foreach (DataGridViewRow item in dgvrc)
                try
                {
                    rows.Add(item.Index, item);
                }
                catch { }
            List<int> inar = rows.Keys.ToList();
            inar.Reverse();
            foreach (var item in inar)
                try
                {
                    dgv.Rows.Remove(rows[item]);
                }
                catch { }
            return rows;
        }
#endregion

#region DataTable
        public static DataTable DataTableTranslate( DataTable dt, ToStringBase displayer = null)
        {
            if (displayer == null)
            {
                displayer = new ToText();
                displayer.AllowBitmap = false;
            }
            displayer.Translate = true;
            if (displayer.PointerSign.Contains("<") || displayer.PointerSign.Contains(">")) displayer.PointerSign = ": ";
            DataTable newdt = new DataTable();
            for (int j = 0; j < dt.Columns.Count; j++)
                if (dt.Columns[j].DataType == typeof(object))
                    newdt.Columns.Add(dt.Columns[j].ColumnName, typeof(object));
                else if (dt.Columns[j].DataType.IsAssignableFrom(typeof(Bitmap)))
                    newdt.Columns.Add(dt.Columns[j].ColumnName, typeof(Bitmap));
                else
                    newdt.Columns.Add( dt.Columns[j].ColumnName, typeof(object));
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                object[] oa = new object[dt.Columns.Count];
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    if ((dt.Rows[i].ItemArray[j] is Bitmap)) oa[j] = dt.Rows[i].ItemArray[j];
                    else
                        try
                        {
                            oa[j] = displayer.TryDone(dt.Rows[i].ItemArray[j]).ToString();
                        }
                        catch { }
                }
                newdt.Rows.Add(oa);
            }
            return newdt;
         }
        public static DataTable DataTableHeaderTranslate( DataTable dt, ToStringBase displayer = null)
        {
            if (displayer == null)
            {
                displayer = new ToText();
                if (displayer.PointerSign.Contains("<") || displayer.PointerSign.Contains(">")) displayer.PointerSign = ": ";
                displayer.AllowBitmap = false;
            }
            displayer.Translate = true;
            if (displayer.PointerSign.Contains("<") || displayer.PointerSign.Contains(">")) displayer.PointerSign = ": ";
            for (int i = 0; i < dt.Columns.Count; i++)
                try { dt.Columns[i].ColumnName = displayer.Done(dt.Columns[i].ColumnName); } catch { }
            return dt;
        }
        public static void DataTableToExcel(DataTable dt, bool openAfter = true)
        {
            string addr = SaveAddress("My Export","xlsx");
            if (!string.IsNullOrEmpty(addr)) ConvertService.ToExcelFile(dt, addr, "MiMFa Export", openAfter);
        }
#endregion

#region WebBrowser
        public static void WebBrowserDocumentText(ref WebBrowser wb,string html)
        {
            wb.DocumentText=html;
        }
        public static void WebBrowserDocument(ref WebBrowser wb,string html)
        {
            if (wb.Document == null) WebBrowserDocumentText(ref wb,html);
            else
            {
                wb.Navigate("about:blank");
                wb.Document.OpenNew(true);
                wb.Document.Write(html);
                wb.Refresh();
            }
        }
        public static void WebBrowserDocument(ref WebBrowser wb,HtmlDocument html)
        {
            WebBrowserDocument(ref wb, html.All);
        }
        public static void WebBrowserDocument(ref WebBrowser wb, HtmlElementCollection html)
        {
            string ht = "";
            foreach (HtmlElement item in html)
                ht += item.OuterHtml;
            WebBrowserDocument(ref wb, ht);
        }
        public static void WebBrowserAppend(ref WebBrowser wb, string htmlpart)
        {
            if (wb.Document == null) WebBrowserDocumentText(ref wb, htmlpart);
            else
            {
                //wb.Document.Write(htmlpart);
                wb.DocumentText += htmlpart;
                //wb.Refresh();
            }
        }
        public static void WebBrowserNavigate(ref WebBrowser wb, string html, string path)
        {
            File.WriteAllText(path, html);
            wb.Navigate(path);
        }
        public static void WebBrowserNavigate(ref WebBrowser wb, string url)
        {
            WebBrowserNavigate(ref wb,new Uri( url));
        }
        public static void WebBrowserNavigate(ref WebBrowser wb, Uri uri)
        {
            wb.Refresh(WebBrowserRefreshOption.Completely);
            wb.Navigate(uri);
        }

        public static Control GetParentControl(Control control) => GetParentControl<Control>(control);
        public static T GetParentControl<T>(Control control) where T : Control
        {
            return (T)GetParentObject<T>(control);
        }
        public static object GetParentObject<T>(Control control)
        {
            if (control.Parent == null) return default(T);
            if (control.Parent is T) return control.Parent;
            return GetParentObject<T>(control.Parent);
        }
#endregion
    }
}
