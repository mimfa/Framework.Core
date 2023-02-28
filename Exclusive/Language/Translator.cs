using MiMFa.Exclusive.ProgramingTechnology.CommandLanguage;
using MiMFa.General;
using MiMFa.Service;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace MiMFa.Exclusive.Language
{
    public class Translator
    {
        public virtual string SignTranslate { get; set; } = "";
        public virtual bool InternalParameterTranslate
        {
            get { return SignTranslate.Contains("¶"); }
            set
            {
                if (value) SignTranslate = "¶" + SignTranslate;
                else SignTranslate.Replace("¶", "");
            }
        }
        public virtual bool TryTranslate
        {
            get { return SignTranslate.Contains("§"); }
            set
            {
                if (value) SignTranslate = "§" + SignTranslate;
                else SignTranslate.Replace("§", "");
            }
        }
        public virtual bool NoTranslate
        {
            get { return SignTranslate.Contains("▬"); }
            set
            {
                if (value) SignTranslate = "▬" + SignTranslate;
                else SignTranslate.Replace("▬", "");
            }
        }
        public virtual bool FullTranslate
        {
            get { return SignTranslate.Contains("↨"); }
            set
            {
                if (value) SignTranslate = "↨" + SignTranslate;
                else SignTranslate.Replace("↨", "");
            }
        }

        public virtual dynamic TryDone(dynamic obj)
        {
            if (obj == null) return obj;
            Type t = obj.GetType();
            string tn = t.Name.ToLower();
            try
            {
                if ((t is IDictionary || tn.StartsWith("dictionary") || tn.StartsWith("mimfa_dictionary"))) return Done(ConvertService.ToDictionary(obj));
                if (tn.StartsWith("keyvaluepair")) return Done(ConvertService.ToKeyValuePair(obj));
                if (t is IList || tn.StartsWith("list") || tn.StartsWith("mimfa_list")) return Done(ConvertService.ToList(obj));
                if (tn.EndsWith("[]")) return Done(ConvertService.ToArray(obj));
                if (t is ICollection) return Done(ConvertService.ToArray(obj));
                if (tn.StartsWith("datatable")) return Done((DataTable)obj);
                if (tn.StartsWith("datarow")) return Done((DataRow)obj);
                if (obj is Enum) return Done(obj.ToString());
                if (obj is EventPack) return Done((EventPack)obj);
                if (obj is String) return Done(obj.ToString());

                if (obj is TextBox) return Done((TextBox)obj);
                if (obj is RichTextBox) return Done((RichTextBox)obj);
                if (obj is TabPage) return Done((TabPage)obj);
                if (obj is TabControl) return Done((TabControl)obj);
                if (obj is ToolStripDropDownButton) return Done((ToolStripDropDownButton)obj);
                if (obj is ToolStripDropDown) return Done((ToolStripDropDown)obj);
                if (obj is ToolStripComboBox) return Done((ToolStripComboBox)obj);
                if (obj is ToolStripButton) return Done((ToolStripButton)obj);
                if (obj is ToolStripItem) return Done((ToolStripItem)obj);
                if (obj is ContextMenu) return Done((ContextMenu)obj);
                if (obj is ContextMenuStrip) return Done((ContextMenuStrip)obj);
                if (obj is ToolStrip) return Done((ToolStrip)obj);
                if (obj is MenuStrip) return Done((MenuStrip)obj);
                if (obj is Menu) return Done((Menu)obj);
                if (obj is ComboBox) return Done((ComboBox)obj);
                if (obj is DataGridView) return Done((DataGridView)obj);
                if (obj is Chart) return Done((Chart)obj);
                if (obj is Control) return Done((Control)obj);
            }
            catch { }
            return obj;
        }

        public virtual string Done(string arg)
        {
            if (arg == null) return arg;
            return Reader.GetText(SignTranslate + arg);
        }

        #region windows forms control
        public virtual Chart Done(Chart arg)
        {
            if (arg == null) return arg;
            for (int i = 0; i < arg.Titles.Count; i++)
            {
                arg.Titles[i].Text = Done(arg.Titles[i].Text);
            }
            for (int i = 0; i < arg.Series.Count; i++)
            {
                arg.Series[i].Label = Done(arg.Series[i].Label);
                for (int j = 0; j < arg.Series[i].Points.Count; j++)
                {
                    arg.Series[i].Points[j].Label = Done(arg.Series[i].Points[j].Label);
                    arg.Series[i].Points[j].LabelToolTip = Done(arg.Series[i].Points[j].LabelToolTip);
                    arg.Series[i].Points[j].LegendText = Done(arg.Series[i].Points[j].LegendText);
                    arg.Series[i].Points[j].AxisLabel = Done(arg.Series[i].Points[j].AxisLabel);
                }
            }
            for (int i = 0; i < arg.Legends.Count; i++)
                arg.Legends[i].Title = Done(arg.Legends[i].Title);
            return arg;
        }
        public virtual TextBox Done(TextBox arg)
        {
            if (arg == null) return arg;
            for (int i = 0; i < arg.Lines.Length; i++)
                arg.Lines[i] = Done(arg.Lines[i]);
            return arg;
        }
        public virtual RichTextBox Done(RichTextBox arg)
        {
            if (arg == null) return arg;
            string s = "";
            for (int i = 0; i < arg.Lines.Length; i++)
                arg.Lines[i] = s = Done(arg.Lines[i]);
            return arg;
        }
        public virtual TabPage Done(TabPage arg)
        {
            if (arg == null) return arg;
            arg.Text = Done(arg.Text);
            return arg;
        }
        public virtual TabControl Done(TabControl arg)
        {
            if (arg == null) return arg;
            arg.Text = Done(arg.Text);
            for (int i = 0; i < arg.TabPages.Count; i++)
                arg.TabPages[i] = Done(arg.TabPages[i]);
            return arg;
        }
        public virtual ToolStripMenuItem Done(ToolStripMenuItem arg)
        {
            if (arg == null) return arg;
            arg.Text = Done(arg.Text);
            for (int i = 0; i < arg.DropDownItems.Count; i++)
                arg.DropDownItems[i].Text = Done(arg.DropDownItems[i].Text);
            return arg;
        }
        public virtual ToolStripItem Done(ToolStripItem arg)
        {
            if (arg == null) return arg;
            arg.Text = Done(arg.Text);
            return arg;
        }
        public virtual ContextMenu Done(ContextMenu arg)
        {
            if (arg == null) return arg;
            for (int i = 0; i < arg.MenuItems.Count; i++)
               arg.MenuItems[i].Text = Done(arg.MenuItems[i].Text);
            return arg;
        }
        public virtual ContextMenuStrip Done(ContextMenuStrip arg)
        {
            if (arg == null) return arg;
            arg.Text = Done(arg.Text);
            for (int i = 0; i < arg.Items.Count; i++)
               arg.Items[i].Text = Done(arg.Items[i].Text);
            return arg;
        }
        public virtual ToolStrip Done(ToolStrip arg)
        {
            if (arg == null) return arg;
            arg.Text = Done(arg.Text);
            for (int i = 0; i < arg.Items.Count; i++)
               arg.Items[i].Text = Done(arg.Items[i].Text);
            return arg;
        }
        public virtual ToolStripDropDown Done(ToolStripDropDown arg)
        {
            if (arg == null) return arg;
            arg.Text = Done(arg.Text);
            for (int i = 0; i < arg.Items.Count; i++)
               arg.Items[i].Text = TryDone(arg.Items[i]);
            return arg;
        }
        public virtual ToolStripDropDownButton Done(ToolStripDropDownButton arg)
        {
            if (arg == null) return arg;
            arg.Text = Done(arg.Text);
            return arg;
        }
        public virtual ToolStripComboBox Done(ToolStripComboBox arg)
        {
            if (arg == null) return arg;
            arg.Text = Done(arg.Text);
            for (int i = 0; i < arg.Items.Count; i++)
                arg.Items[i] = TryDone(arg.Items[i]);
            return arg;
        }
        public virtual ToolStripButton Done(ToolStripButton arg)
        {
            if (arg == null) return arg;
            arg.Text = Done(arg.Text);
            return arg;
        }
        public virtual MenuStrip Done(MenuStrip arg)
        {
            if (arg == null) return arg;
            arg.Text = Done(arg.Text);
            for (int i = 0; i < arg.Items.Count; i++)
               arg.Items[i].Text = Done(arg.Items[i].Text);
            return arg;
        }
        public virtual Menu Done(Menu arg)
        {
            if (arg == null) return arg;
            for (int i = 0; i < arg.MenuItems.Count; i++)
              arg.MenuItems[i].Text = Done(arg.MenuItems[i].Text);
            return arg;
        }
        public virtual ComboBox Done(ComboBox arg)
        {
            if (arg == null) return arg;
            arg.Text = Done(arg.Text);
            for (int i = 0; i < arg.Items.Count; i++)
                arg.Items[i] = TryDone(arg.Items[i]);
            return arg;
        }
        public virtual DataGridView Done(DataGridView arg)
        {
            if (arg == null) return arg;
            arg.Text = Done(arg.Text);
            for (int i = 0; i < arg.Columns.Count; i++)
                arg.Columns[i].HeaderText = Done(arg.Columns[i].HeaderText);
            for (int i = 0; i < arg.Rows.Count; i++)
                for (int j = 0; j < arg.Rows[i].Cells.Count; j++)
                    arg.Rows[i].Cells[j].Value = TryDone(arg.Rows[i].Cells[j].Value);
            return arg;
        }
        public virtual Control Done(Control arg)
        {
            if (arg == null) return arg;
            arg.Text = Done(arg.Text);
            return arg;
        }
        #endregion

        #region types
        public virtual T[] Done<T>(T[] arg)
        {
            if (arg == null) return arg;
            for (int i = 0; i < arg.Length; i++)
                arg[i] = TryDone(arg[i]);
            return arg;
        }
        public virtual IList<T> Done<T>(IList<T> arg)
        {
            if (arg == null) return arg;
            for (int i = 0; i < arg.Count; i++)
                arg[i] = TryDone(arg[i]);
            return arg;
        }
        public virtual IDictionary<T, F> Done<T, F>(IDictionary<T, F> arg)
        {
            if (arg == null) return arg;
            foreach (var item in arg.Keys)
                arg[item]= TryDone(arg[item]);
            return arg;
        }
        public virtual KeyValuePair<T, F> Done<T, F>( KeyValuePair<T, F> arg)
        {
            return new KeyValuePair<T, F>(TryDone(arg.Key), TryDone(arg.Value));
        }
        public virtual DataTable Done(DataTable arg)
        {
            if (arg == null) return arg;
            for (int i = 0; i < arg.Columns.Count; i++)
                arg.Columns[i].ColumnName = Done(arg.Columns[i].ColumnName);
            for (int i = 0; i < arg.Rows.Count; i++)
                for (int j = 0; j < arg.Rows[i].ItemArray.Length; j++)
                    arg.Rows[i][j] = TryDone(arg.Rows[i].ItemArray[j]);
            return arg;
        }
        public virtual DataRow Done(DataRow arg)
        {
            if (arg == null) return arg;
            for (int i = 0; i < arg.ItemArray.Length; i++)
                arg[i] = TryDone(arg.ItemArray[i]);
            return arg;
        }
        public virtual EventPack Done(EventPack arg)
        {
            if (arg == null) return arg;
            arg.Target = Done(arg.Target);
            return arg;
        }
        #endregion
    }
}
