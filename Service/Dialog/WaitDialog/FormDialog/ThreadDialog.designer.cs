namespace MiMFa.Service.Dialog.WaitDialog.FormDialog
{
    partial class ThreadDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ThreadDialog));
            this.TextLabel = new System.Windows.Forms.Label();
            this.TitleLabel = new System.Windows.Forms.Label();
            this.ExitButton = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.WatingBar = new System.Windows.Forms.PictureBox();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.WatingBar)).BeginInit();
            this.SuspendLayout();
            // 
            // TextLabel
            // 
            this.TextLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TextLabel.Location = new System.Drawing.Point(0, 28);
            this.TextLabel.Name = "TextLabel";
            this.TextLabel.Padding = new System.Windows.Forms.Padding(5, 5, 5, 10);
            this.TextLabel.Size = new System.Drawing.Size(453, 60);
            this.TextLabel.TabIndex = 2;
            this.TextLabel.Text = "There a process is running!";
            this.TextLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // TitleLabel
            // 
            this.TitleLabel.AutoSize = true;
            this.TitleLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.TitleLabel.Location = new System.Drawing.Point(0, 0);
            this.TitleLabel.Name = "TitleLabel";
            this.TitleLabel.Padding = new System.Windows.Forms.Padding(5, 10, 5, 5);
            this.TitleLabel.Size = new System.Drawing.Size(133, 28);
            this.TitleLabel.TabIndex = 1;
            this.TitleLabel.Text = "Please Wait a Moment...";
            // 
            // ExitButton
            // 
            this.ExitButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ExitButton.AutoSize = true;
            this.ExitButton.BackColor = System.Drawing.Color.Transparent;
            this.ExitButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.ExitButton.FlatAppearance.BorderSize = 0;
            this.ExitButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ExitButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ExitButton.Location = new System.Drawing.Point(424, 1);
            this.ExitButton.Name = "ExitButton";
            this.ExitButton.Size = new System.Drawing.Size(28, 24);
            this.ExitButton.TabIndex = 1;
            this.ExitButton.TabStop = false;
            this.ExitButton.Text = "×";
            this.ExitButton.UseVisualStyleBackColor = false;
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.ExitButton);
            this.panel1.Controls.Add(this.TextLabel);
            this.panel1.Controls.Add(this.WatingBar);
            this.panel1.Controls.Add(this.TitleLabel);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(455, 91);
            this.panel1.TabIndex = 2;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // WatingBar
            // 
            this.WatingBar.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.WatingBar.Image = global::MiMFa.Properties.Resources.Progress_Bar_Crystal;
            this.WatingBar.Location = new System.Drawing.Point(0, 88);
            this.WatingBar.Name = "WatingBar";
            this.WatingBar.Size = new System.Drawing.Size(453, 1);
            this.WatingBar.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.WatingBar.TabIndex = 0;
            this.WatingBar.TabStop = false;
            // 
            // ThreadDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DarkViolet;
            this.ClientSize = new System.Drawing.Size(455, 91);
            this.Controls.Add(this.panel1);
            this.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ThreadDialog";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Please Wait...";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.WaitDialog_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ThreadDialog_FormClosed);
            this.Shown += new System.EventHandler(this.ThreadDialog_Shown);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.WatingBar)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        public System.Windows.Forms.Label TextLabel;
        public System.Windows.Forms.Label TitleLabel;
        private System.Windows.Forms.Button ExitButton;
        private System.Windows.Forms.Panel panel1;
        public System.Windows.Forms.PictureBox WatingBar;
    }
}