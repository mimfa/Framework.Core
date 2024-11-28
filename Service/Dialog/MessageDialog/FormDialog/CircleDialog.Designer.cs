namespace MiMFa.Service.Dialog.MessageDialog.FormDialog
{
    partial class CircleDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tlp_Main = new System.Windows.Forms.TableLayoutPanel();
            this.MSGContent = new System.Windows.Forms.Label();
            this.p_Button = new System.Windows.Forms.Panel();
            this.btn_Cancel = new System.Windows.Forms.Button();
            this.Btn_No = new System.Windows.Forms.Button();
            this.btn_Yes = new System.Windows.Forms.Button();
            this.p_TR = new System.Windows.Forms.Panel();
            this.TextResult = new System.Windows.Forms.TextBox();
            this.btn_Ok = new System.Windows.Forms.Button();
            this.p_Icon = new System.Windows.Forms.Panel();
            this.pb_Icon = new System.Windows.Forms.PictureBox();
            this.l_Subject = new System.Windows.Forms.Label();
            this.tlp_Main.SuspendLayout();
            this.p_Button.SuspendLayout();
            this.p_TR.SuspendLayout();
            this.p_Icon.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pb_Icon)).BeginInit();
            this.SuspendLayout();
            // 
            // tlp_Main
            // 
            this.tlp_Main.BackColor = System.Drawing.Color.Transparent;
            this.tlp_Main.BackgroundImage = global::MiMFa.Properties.Resources.Frame_Circle_Color;
            this.tlp_Main.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.tlp_Main.ColumnCount = 3;
            this.tlp_Main.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlp_Main.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_Main.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlp_Main.Controls.Add(this.MSGContent, 1, 1);
            this.tlp_Main.Controls.Add(this.p_Button, 1, 2);
            this.tlp_Main.Controls.Add(this.p_Icon, 1, 0);
            this.tlp_Main.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_Main.Location = new System.Drawing.Point(0, 0);
            this.tlp_Main.Name = "tlp_Main";
            this.tlp_Main.RowCount = 3;
            this.tlp_Main.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlp_Main.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_Main.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlp_Main.Size = new System.Drawing.Size(284, 283);
            this.tlp_Main.TabIndex = 1;
            // 
            // MSGContent
            // 
            this.MSGContent.AutoSize = true;
            this.MSGContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MSGContent.Location = new System.Drawing.Point(30, 94);
            this.MSGContent.Margin = new System.Windows.Forms.Padding(10);
            this.MSGContent.Name = "MSGContent";
            this.MSGContent.Size = new System.Drawing.Size(224, 86);
            this.MSGContent.TabIndex = 0;
            this.MSGContent.Text = "Content of Message";
            // 
            // p_Button
            // 
            this.p_Button.Controls.Add(this.btn_Cancel);
            this.p_Button.Controls.Add(this.Btn_No);
            this.p_Button.Controls.Add(this.btn_Yes);
            this.p_Button.Controls.Add(this.p_TR);
            this.p_Button.Controls.Add(this.btn_Ok);
            this.p_Button.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.p_Button.Location = new System.Drawing.Point(23, 193);
            this.p_Button.Name = "p_Button";
            this.p_Button.Size = new System.Drawing.Size(238, 87);
            this.p_Button.TabIndex = 1;
            // 
            // btn_Cancel
            // 
            this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btn_Cancel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_Cancel.FlatAppearance.BorderSize = 0;
            this.btn_Cancel.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
            this.btn_Cancel.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btn_Cancel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btn_Cancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Cancel.Location = new System.Drawing.Point(75, 0);
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.Padding = new System.Windows.Forms.Padding(5);
            this.btn_Cancel.Size = new System.Drawing.Size(88, 32);
            this.btn_Cancel.TabIndex = 1;
            this.btn_Cancel.Text = "Cancel";
            this.btn_Cancel.UseMnemonic = false;
            this.btn_Cancel.Click += new System.EventHandler(this.btn_Close_Click);
            // 
            // Btn_No
            // 
            this.Btn_No.DialogResult = System.Windows.Forms.DialogResult.No;
            this.Btn_No.Dock = System.Windows.Forms.DockStyle.Right;
            this.Btn_No.FlatAppearance.BorderSize = 0;
            this.Btn_No.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
            this.Btn_No.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.Btn_No.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.Btn_No.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Btn_No.Location = new System.Drawing.Point(163, 0);
            this.Btn_No.Name = "Btn_No";
            this.Btn_No.Padding = new System.Windows.Forms.Padding(5);
            this.Btn_No.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Btn_No.Size = new System.Drawing.Size(75, 32);
            this.Btn_No.TabIndex = 2;
            this.Btn_No.Text = "No";
            this.Btn_No.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_No.UseMnemonic = false;
            this.Btn_No.Click += new System.EventHandler(this.Btn_No_Click);
            // 
            // btn_Yes
            // 
            this.btn_Yes.DialogResult = System.Windows.Forms.DialogResult.Yes;
            this.btn_Yes.Dock = System.Windows.Forms.DockStyle.Left;
            this.btn_Yes.FlatAppearance.BorderSize = 0;
            this.btn_Yes.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
            this.btn_Yes.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btn_Yes.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btn_Yes.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Yes.Location = new System.Drawing.Point(0, 0);
            this.btn_Yes.Name = "btn_Yes";
            this.btn_Yes.Padding = new System.Windows.Forms.Padding(5);
            this.btn_Yes.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btn_Yes.Size = new System.Drawing.Size(75, 32);
            this.btn_Yes.TabIndex = 0;
            this.btn_Yes.Text = "Yes";
            this.btn_Yes.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btn_Yes.UseMnemonic = false;
            this.btn_Yes.Click += new System.EventHandler(this.btn_Yes_Click);
            // 
            // p_TR
            // 
            this.p_TR.AutoSize = true;
            this.p_TR.Controls.Add(this.TextResult);
            this.p_TR.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.p_TR.Location = new System.Drawing.Point(0, 32);
            this.p_TR.Name = "p_TR";
            this.p_TR.Padding = new System.Windows.Forms.Padding(25, 1, 25, 1);
            this.p_TR.Size = new System.Drawing.Size(238, 24);
            this.p_TR.TabIndex = 4;
            this.p_TR.Visible = false;
            // 
            // TextResult
            // 
            this.TextResult.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.TextResult.Location = new System.Drawing.Point(25, 1);
            this.TextResult.Name = "TextResult";
            this.TextResult.Size = new System.Drawing.Size(188, 22);
            this.TextResult.TabIndex = 6;
            this.TextResult.TextChanged += new System.EventHandler(this.TextResult_TextChanged);
            this.TextResult.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TextResult_KeyUp);
            // 
            // btn_Ok
            // 
            this.btn_Ok.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btn_Ok.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btn_Ok.FlatAppearance.BorderSize = 0;
            this.btn_Ok.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
            this.btn_Ok.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btn_Ok.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btn_Ok.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Ok.Location = new System.Drawing.Point(0, 56);
            this.btn_Ok.Name = "btn_Ok";
            this.btn_Ok.Padding = new System.Windows.Forms.Padding(5);
            this.btn_Ok.Size = new System.Drawing.Size(238, 31);
            this.btn_Ok.TabIndex = 3;
            this.btn_Ok.Text = "Ok";
            this.btn_Ok.UseMnemonic = false;
            this.btn_Ok.Click += new System.EventHandler(this.btn_Ok_Click);
            // 
            // p_Icon
            // 
            this.p_Icon.AutoSize = true;
            this.p_Icon.Controls.Add(this.pb_Icon);
            this.p_Icon.Controls.Add(this.l_Subject);
            this.p_Icon.Dock = System.Windows.Forms.DockStyle.Top;
            this.p_Icon.Location = new System.Drawing.Point(23, 3);
            this.p_Icon.Name = "p_Icon";
            this.p_Icon.Padding = new System.Windows.Forms.Padding(0, 30, 0, 0);
            this.p_Icon.Size = new System.Drawing.Size(238, 78);
            this.p_Icon.TabIndex = 2;
            // 
            // pb_Icon
            // 
            this.pb_Icon.BackgroundImage = global::MiMFa.Properties.Resources.Shadow_Circle;
            this.pb_Icon.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pb_Icon.Dock = System.Windows.Forms.DockStyle.Top;
            this.pb_Icon.Image = global::MiMFa.Properties.Resources.star_white;
            this.pb_Icon.Location = new System.Drawing.Point(0, 30);
            this.pb_Icon.Name = "pb_Icon";
            this.pb_Icon.Size = new System.Drawing.Size(238, 22);
            this.pb_Icon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pb_Icon.TabIndex = 0;
            this.pb_Icon.TabStop = false;
            // 
            // l_Subject
            // 
            this.l_Subject.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.l_Subject.Location = new System.Drawing.Point(0, 52);
            this.l_Subject.Name = "l_Subject";
            this.l_Subject.Padding = new System.Windows.Forms.Padding(5);
            this.l_Subject.Size = new System.Drawing.Size(238, 26);
            this.l_Subject.TabIndex = 1;
            this.l_Subject.Text = "Subject";
            this.l_Subject.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // CircleDialog
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.OrangeRed;
            this.BackgroundImage = global::MiMFa.Properties.Resources.Frame_Circle;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.CancelButton = this.btn_Cancel;
            this.ClientSize = new System.Drawing.Size(284, 283);
            this.Controls.Add(this.tlp_Main);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "CircleDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Circle";
            this.TopMost = true;
            this.TransparencyKey = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(54)))), ((int)(((byte)(54)))));
            this.tlp_Main.ResumeLayout(false);
            this.tlp_Main.PerformLayout();
            this.p_Button.ResumeLayout(false);
            this.p_Button.PerformLayout();
            this.p_TR.ResumeLayout(false);
            this.p_TR.PerformLayout();
            this.p_Icon.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pb_Icon)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TableLayoutPanel tlp_Main;
        private System.Windows.Forms.Label MSGContent;
        private System.Windows.Forms.Panel p_Button;
        private System.Windows.Forms.Button btn_Cancel;
        private System.Windows.Forms.Button Btn_No;
        private System.Windows.Forms.Button btn_Yes;
        private System.Windows.Forms.Button btn_Ok;
        private System.Windows.Forms.Panel p_Icon;
        private System.Windows.Forms.PictureBox pb_Icon;
        private System.Windows.Forms.Label l_Subject;
        private System.Windows.Forms.Panel p_TR;
        public System.Windows.Forms.Control TextResult;
    }
}