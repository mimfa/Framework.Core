namespace MiMFa.Service.Dialog.MessageDialog.FormDialog
{
    partial class ModernDialog
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
            this.p_Content = new System.Windows.Forms.Panel();
            this.MSGContent = new System.Windows.Forms.Label();
            this.p_TR = new System.Windows.Forms.Panel();
            this.TextResult = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.l_Subject = new System.Windows.Forms.Label();
            this.pb_Icon = new System.Windows.Forms.PictureBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.cb_AgainShow = new System.Windows.Forms.CheckBox();
            this.btn_Ok = new System.Windows.Forms.Button();
            this.btn_Yes = new System.Windows.Forms.Button();
            this.Btn_No = new System.Windows.Forms.Button();
            this.btn_Cancel = new System.Windows.Forms.Button();
            this.tlp_Main.SuspendLayout();
            this.p_Content.SuspendLayout();
            this.p_TR.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pb_Icon)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tlp_Main
            // 
            this.tlp_Main.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.tlp_Main.ColumnCount = 4;
            this.tlp_Main.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlp_Main.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_Main.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlp_Main.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlp_Main.Controls.Add(this.p_Content, 1, 1);
            this.tlp_Main.Controls.Add(this.tableLayoutPanel1, 2, 1);
            this.tlp_Main.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_Main.Location = new System.Drawing.Point(0, 0);
            this.tlp_Main.Name = "tlp_Main";
            this.tlp_Main.RowCount = 3;
            this.tlp_Main.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlp_Main.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_Main.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlp_Main.Size = new System.Drawing.Size(567, 191);
            this.tlp_Main.TabIndex = 1;
            // 
            // p_Content
            // 
            this.p_Content.Controls.Add(this.MSGContent);
            this.p_Content.Controls.Add(this.p_TR);
            this.p_Content.Controls.Add(this.tableLayoutPanel2);
            this.p_Content.Dock = System.Windows.Forms.DockStyle.Fill;
            this.p_Content.Location = new System.Drawing.Point(23, 23);
            this.p_Content.Name = "p_Content";
            this.p_Content.Size = new System.Drawing.Size(382, 145);
            this.p_Content.TabIndex = 2;
            // 
            // MSGContent
            // 
            this.MSGContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MSGContent.Location = new System.Drawing.Point(0, 24);
            this.MSGContent.Margin = new System.Windows.Forms.Padding(3);
            this.MSGContent.Name = "MSGContent";
            this.MSGContent.Padding = new System.Windows.Forms.Padding(0, 10, 0, 10);
            this.MSGContent.Size = new System.Drawing.Size(382, 94);
            this.MSGContent.TabIndex = 0;
            this.MSGContent.Text = "Content of Message";
            // 
            // p_TR
            // 
            this.p_TR.AutoSize = true;
            this.p_TR.BackColor = System.Drawing.SystemColors.Window;
            this.p_TR.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.p_TR.Controls.Add(this.TextResult);
            this.p_TR.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.p_TR.Location = new System.Drawing.Point(0, 118);
            this.p_TR.Name = "p_TR";
            this.p_TR.Padding = new System.Windows.Forms.Padding(5);
            this.p_TR.Size = new System.Drawing.Size(382, 27);
            this.p_TR.TabIndex = 4;
            this.p_TR.Visible = false;
            // 
            // TextResult
            // 
            this.TextResult.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.TextResult.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.TextResult.Location = new System.Drawing.Point(5, 5);
            this.TextResult.Name = "TextResult";
            this.TextResult.Size = new System.Drawing.Size(370, 15);
            this.TextResult.TabIndex = 6;
            this.TextResult.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.TextResult.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TextResult_KeyUp);
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.AutoSize = true;
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.l_Subject, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.pb_Icon, 0, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.Size = new System.Drawing.Size(382, 24);
            this.tableLayoutPanel2.TabIndex = 2;
            // 
            // l_Subject
            // 
            this.l_Subject.AutoSize = true;
            this.l_Subject.Dock = System.Windows.Forms.DockStyle.Top;
            this.l_Subject.Location = new System.Drawing.Point(24, 0);
            this.l_Subject.Name = "l_Subject";
            this.l_Subject.Padding = new System.Windows.Forms.Padding(5);
            this.l_Subject.Size = new System.Drawing.Size(355, 24);
            this.l_Subject.TabIndex = 1;
            this.l_Subject.Text = "Subject";
            this.l_Subject.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pb_Icon
            // 
            this.pb_Icon.BackgroundImage = global::MiMFa.Properties.Resources.Shadow_Circle;
            this.pb_Icon.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pb_Icon.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pb_Icon.Image = global::MiMFa.Properties.Resources.star_white;
            this.pb_Icon.Location = new System.Drawing.Point(0, 0);
            this.pb_Icon.Margin = new System.Windows.Forms.Padding(0);
            this.pb_Icon.Name = "pb_Icon";
            this.pb_Icon.Size = new System.Drawing.Size(21, 24);
            this.pb_Icon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pb_Icon.TabIndex = 0;
            this.pb_Icon.TabStop = false;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.cb_AgainShow, 0, 6);
            this.tableLayoutPanel1.Controls.Add(this.btn_Ok, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.btn_Yes, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.Btn_No, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.btn_Cancel, 0, 5);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(428, 23);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(20, 3, 3, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 7;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(116, 145);
            this.tableLayoutPanel1.TabIndex = 3;
            // 
            // cb_AgainShow
            // 
            this.cb_AgainShow.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cb_AgainShow.AutoSize = true;
            this.cb_AgainShow.Checked = true;
            this.cb_AgainShow.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cb_AgainShow.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_AgainShow.Location = new System.Drawing.Point(22, 124);
            this.cb_AgainShow.Name = "cb_AgainShow";
            this.cb_AgainShow.Size = new System.Drawing.Size(91, 18);
            this.cb_AgainShow.TabIndex = 7;
            this.cb_AgainShow.Text = "Show Again";
            this.cb_AgainShow.UseVisualStyleBackColor = true;
            // 
            // btn_Ok
            // 
            this.btn_Ok.AutoSize = true;
            this.btn_Ok.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btn_Ok.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btn_Ok.FlatAppearance.BorderSize = 0;
            this.btn_Ok.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Ok.Location = new System.Drawing.Point(3, -40);
            this.btn_Ok.Name = "btn_Ok";
            this.btn_Ok.Padding = new System.Windows.Forms.Padding(5, 3, 5, 3);
            this.btn_Ok.Size = new System.Drawing.Size(110, 36);
            this.btn_Ok.TabIndex = 3;
            this.btn_Ok.Text = "Ok";
            this.btn_Ok.UseMnemonic = false;
            this.btn_Ok.Click += new System.EventHandler(this.btn_Ok_Click);
            // 
            // btn_Yes
            // 
            this.btn_Yes.AutoSize = true;
            this.btn_Yes.DialogResult = System.Windows.Forms.DialogResult.Yes;
            this.btn_Yes.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btn_Yes.FlatAppearance.BorderSize = 0;
            this.btn_Yes.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Yes.Location = new System.Drawing.Point(3, 2);
            this.btn_Yes.Name = "btn_Yes";
            this.btn_Yes.Padding = new System.Windows.Forms.Padding(5, 3, 5, 3);
            this.btn_Yes.Size = new System.Drawing.Size(110, 36);
            this.btn_Yes.TabIndex = 0;
            this.btn_Yes.Text = "Yes";
            this.btn_Yes.UseMnemonic = false;
            this.btn_Yes.Click += new System.EventHandler(this.btn_Yes_Click);
            // 
            // Btn_No
            // 
            this.Btn_No.AutoSize = true;
            this.Btn_No.DialogResult = System.Windows.Forms.DialogResult.No;
            this.Btn_No.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.Btn_No.FlatAppearance.BorderSize = 0;
            this.Btn_No.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Btn_No.Location = new System.Drawing.Point(3, 44);
            this.Btn_No.Name = "Btn_No";
            this.Btn_No.Padding = new System.Windows.Forms.Padding(5, 3, 5, 3);
            this.Btn_No.Size = new System.Drawing.Size(110, 34);
            this.Btn_No.TabIndex = 2;
            this.Btn_No.Text = "No";
            this.Btn_No.UseMnemonic = false;
            this.Btn_No.Click += new System.EventHandler(this.Btn_No_Click);
            // 
            // btn_Cancel
            // 
            this.btn_Cancel.AutoSize = true;
            this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btn_Cancel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btn_Cancel.FlatAppearance.BorderSize = 0;
            this.btn_Cancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Cancel.Location = new System.Drawing.Point(3, 84);
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.Padding = new System.Windows.Forms.Padding(5, 3, 5, 3);
            this.btn_Cancel.Size = new System.Drawing.Size(110, 34);
            this.btn_Cancel.TabIndex = 1;
            this.btn_Cancel.Text = "Cancel";
            this.btn_Cancel.UseMnemonic = false;
            this.btn_Cancel.Click += new System.EventHandler(this.btn_Close_Click);
            // 
            // ModernDialog
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.OrangeRed;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.CancelButton = this.btn_Cancel;
            this.ClientSize = new System.Drawing.Size(567, 210);
            this.Controls.Add(this.tlp_Main);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "ModernDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Modern";
            this.TopMost = true;
            this.TransparencyKey = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(54)))), ((int)(((byte)(54)))));
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ModernDialog_FormClosing);
            this.tlp_Main.ResumeLayout(false);
            this.p_Content.ResumeLayout(false);
            this.p_Content.PerformLayout();
            this.p_TR.ResumeLayout(false);
            this.p_TR.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pb_Icon)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TableLayoutPanel tlp_Main;
        private System.Windows.Forms.Label MSGContent;
        private System.Windows.Forms.Button btn_Cancel;
        private System.Windows.Forms.Button Btn_No;
        private System.Windows.Forms.Button btn_Yes;
        private System.Windows.Forms.Button btn_Ok;
        private System.Windows.Forms.Panel p_Content;
        private System.Windows.Forms.PictureBox pb_Icon;
        private System.Windows.Forms.Label l_Subject;
        private System.Windows.Forms.Panel p_TR;
        public System.Windows.Forms.TextBox TextResult;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.CheckBox cb_AgainShow;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
    }
}