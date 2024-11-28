using MiMFa.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MiMFa.General;
using System.Text.RegularExpressions;

namespace MiMFa.Service.Dialog.MessageDialog.FormDialog
{
    public partial class ModernDialog : Form, IDialog
    {
        public int CurrentID = -1;
        public static SmartDictionary<int,SmartKeyValue<DialogResult,long>> Histories = new SmartDictionary<int, SmartKeyValue<DialogResult, long>>();
        public static Regex IDRegex = new Regex(@"\'[\s\S]*\'|\" +"\""+ @"[\s\S]*\" + "\"");
        public Exclusive.Animate.FormMove FMove => new Exclusive.Animate.FormMove(this); 
        public ModernDialog()
        {
            InitializeComponent();
            if (Default.HasTemplator && Default.Templator.Palette.Font != null) Font = Default.Templator.Palette.Font;
            FMove.AddToControl(tlp_Main,tableLayoutPanel1,p_Content, l_Subject,p_TR, MSGContent, pb_Icon);
        }

        public ModernDialog(string text = "", string caption = "", MessageBoxButtons buttuns = MessageBoxButtons.OK, MessageMode icon = MessageMode.Message, MessageBoxDefaultButton dbtn = MessageBoxDefaultButton.Button1, MessageBoxOptions rtlReading = MessageBoxOptions.DefaultDesktopOnly, string defaultValue = "", params string[] options) : this()
        {
            Set(text, caption, buttuns, icon, dbtn, rtlReading, defaultValue, options);
        }
        public DialogResult ShowDialog(string text , string caption = "", MessageBoxButtons buttuns = MessageBoxButtons.OK, MessageMode icon = MessageMode.Message, MessageBoxDefaultButton dbtn = MessageBoxDefaultButton.Button1, MessageBoxOptions rtlReading = MessageBoxOptions.DefaultDesktopOnly, string defaultValue = "", params string[] options)
        {
            Set(text, caption, buttuns, icon, dbtn, rtlReading, defaultValue, options);
            CurrentID = string.Join("'", caption, icon, IDRegex.Replace(text, "'")).GetHashCode();
            p_TR.Hide();
            try
            {
                if (Histories.ContainsKey(CurrentID))
                    if (Histories[CurrentID].Value % 100 != 0)
                    {
                        Histories[CurrentID].Value++;
                        return Histories[CurrentID].Key;
                    }
                    else Histories[CurrentID].Value++;
            }
            catch { }
            try { return base.ShowDialog(); } catch { try { base.Close(); } catch { } }
            return DialogResult.Abort;
        }
        public string GetDialog(string text , string caption = "", MessageBoxButtons buttuns = MessageBoxButtons.OK, MessageMode icon = MessageMode.Message, MessageBoxDefaultButton dbtn = MessageBoxDefaultButton.Button1, MessageBoxOptions rtlReading = MessageBoxOptions.DefaultDesktopOnly, string defaultValue = "", params string[] options)
        {
            Set(text, caption, buttuns, icon, dbtn, rtlReading, defaultValue, options);
            p_TR.Show();
            cb_AgainShow.Checked = true;
            cb_AgainShow.Hide();
            TextResult.Focus();
            try { base.ShowDialog(); } catch { try { base.Close(); } catch { } }
            return DialogResult != DialogResult.OK?null: TextResult.Text;
        }

        public void Set(string text = "", string caption = "", MessageBoxButtons buttuns = MessageBoxButtons.OK, MessageMode icon = MessageMode.Message, MessageBoxDefaultButton dbtn = MessageBoxDefaultButton.Button1, MessageBoxOptions rtlReading = MessageBoxOptions.DefaultDesktopOnly, string defaultValue = "", params string[] options)
        {
            ControlService.SetControlThreadSafe(this, a =>
             {
                 btn_Cancel.Text = Default.Translate("Cancel");
                 Btn_No.Text = Default.Translate("No");
                 btn_Yes.Text = Default.Translate("Yes");
                 btn_Ok.Text = Default.Translate("Ok");
                 cb_AgainShow.Text = Default.Translate("Show Again");
                 MSGContent.Text = text;
                 l_Subject.Text = caption;
                 if (options.Length > 0)
                 {
                     Font f = TextResult.Font;
                     Color fc = TextResult.ForeColor;
                     Color bc = TextResult.BackColor;
                     int w = TextResult.Width;
                     TextResult = new ComboBox();
                     ((ComboBox)TextResult).Items.AddRange(options);
                     ((ComboBox)TextResult).DropDownStyle = ComboBoxStyle.DropDownList;
                     ((ComboBox)TextResult).FlatStyle = FlatStyle.Flat;
                     ((ComboBox)TextResult).Font = f;
                     ((ComboBox)TextResult).ForeColor = fc;
                     ((ComboBox)TextResult).BackColor = bc;
                     ((ComboBox)TextResult).Width = w;
                 }
                 TextResult.Text = defaultValue;
                 switch (buttuns)
                 {
                     case MessageBoxButtons.OK:
                         btn_Cancel.Visible =
                             Btn_No.Visible =
                             btn_Yes.Visible = false;
                         btn_Ok.Show();
                         cb_AgainShow.Show();
                         btn_Ok.Focus();
                         break;
                     case MessageBoxButtons.OKCancel:
                         Btn_No.Visible =
                         btn_Yes.Visible = false;
                         btn_Cancel.Visible =
                         btn_Ok.Visible = true;
                         cb_AgainShow.Show();
                         btn_Ok.Focus();
                         break;
                     case MessageBoxButtons.AbortRetryIgnore:
                         btn_Ok.Visible =
                         false;
                         btn_Yes.Visible =
                         Btn_No.Visible =
                     btn_Cancel.Visible =
                     true;
                         cb_AgainShow.Show();
                         btn_Yes.Focus();
                         break;
                     case MessageBoxButtons.YesNoCancel:
                         btn_Ok.Visible =
                         false;
                         Btn_No.Visible =
                             btn_Yes.Visible =
                             btn_Cancel.Visible =
                             true;
                         cb_AgainShow.Checked = true;
                         cb_AgainShow.Show();
                         btn_Yes.Focus();
                         break;
                     case MessageBoxButtons.YesNo:
                         btn_Cancel.Visible =
                             btn_Ok.Visible =
                             false;
                         Btn_No.Visible =
                             btn_Yes.Visible =
                             true;
                         cb_AgainShow.Checked = true;
                         cb_AgainShow.Show();
                         btn_Yes.Focus();
                         break;
                     case MessageBoxButtons.RetryCancel:
                         Btn_No.Visible =
                         btn_Ok.Visible =
                           false;
                         btn_Cancel.Visible =
                             btn_Yes.Visible =
                             true;
                         cb_AgainShow.Show();
                         btn_Yes.Focus();
                         break;
                     default:
                         break;
                 }
                 switch (icon)
                 {
                     case MessageMode.Success:
                         pb_Icon.Image = Properties.Resources.check_white;
                         BackColor = Color.SeaGreen;
                         break;
                     case MessageMode.Question:
                         pb_Icon.Image = Properties.Resources.Question_White;
                         cb_AgainShow.Checked = true;
                         cb_AgainShow.Visible = true;
                         BackColor = Color.DarkOrchid;
                         break;
                     case MessageMode.Error:
                         pb_Icon.Image = Properties.Resources.forbidden_white;
                         BackColor = Color.Crimson;
                         break;
                     case MessageMode.Warning:
                         pb_Icon.Image = Properties.Resources.alert_white;
                         BackColor = Color.OrangeRed;
                         break;
                     case MessageMode.Message:
                         pb_Icon.Image = Properties.Resources.info_white;
                         BackColor = Color.RoyalBlue;
                         break;
                     default:
                         pb_Icon.Image = null;
                         BackColor = Color.Gray;
                         break;
                 }
                 if (rtlReading == MessageBoxOptions.RightAlign || rtlReading == MessageBoxOptions.RtlReading)
                     this.RightToLeft = RightToLeft.Yes;
                 else
                     this.RightToLeft = RightToLeft.No;
             }
            );
        }

        private void btn_Close_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel; 
            Close();
        }

        private void btn_Yes_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Yes; Close();
        }
        private void Btn_No_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.No; Close();
        }
        private void btn_Ok_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK; Close();
        }

        private void ModernDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (!cb_AgainShow.Checked) Histories.AddOrSet(CurrentID, new SmartKeyValue<DialogResult, long>(DialogResult,1));
            }
            catch { }
        }

        private void TextResult_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.Enter:
                    if(btn_Yes.Focused) btn_Yes_Click(btn_Yes,e);
                    else if(btn_Ok.Focused) btn_Ok_Click(btn_Ok, e);
                    else if(Btn_No.Focused) Btn_No_Click(Btn_No, e);
                    else if(btn_Cancel.Focused) Close();
                    else if (btn_Yes.Visible) btn_Yes_Click(btn_Yes, e);
                    else if (btn_Ok.Visible) btn_Ok_Click(btn_Ok, e);
                    else if (Btn_No.Visible) Btn_No_Click(Btn_No, e);
                    else if (btn_Cancel.Visible) Close();
                    break;
                case Keys.Escape:
                    if(TextResult.Visible) TextResult.Text = "";
                    else Close();
                    break;
                default:
                    break;
            }
        }
    }
}
