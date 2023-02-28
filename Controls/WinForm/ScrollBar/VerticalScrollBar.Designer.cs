namespace MiMFa.Controls.WinForm.ScrollBar
{
    partial class VerticalScrollBar
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.NextButton = new System.Windows.Forms.Button();
            this.BackButton = new System.Windows.Forms.Button();
            this.p_Slipper = new System.Windows.Forms.Panel();
            this.Slipper = new System.Windows.Forms.Button();
            this.Tracer = new System.Windows.Forms.Timer(this.components);
            this.tableLayoutPanel1.SuspendLayout();
            this.p_Slipper.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.NextButton, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.BackButton, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.p_Slipper, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(7, 354);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // NextButton
            // 
            this.NextButton.AutoSize = true;
            this.NextButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.NextButton.FlatAppearance.BorderSize = 0;
            this.NextButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.NextButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NextButton.Location = new System.Drawing.Point(0, 329);
            this.NextButton.Margin = new System.Windows.Forms.Padding(0);
            this.NextButton.Name = "NextButton";
            this.NextButton.Size = new System.Drawing.Size(7, 25);
            this.NextButton.TabIndex = 0;
            this.NextButton.Text = "▼";
            this.NextButton.UseMnemonic = false;
            this.NextButton.UseVisualStyleBackColor = false;
            this.NextButton.Click += new System.EventHandler(this.NextButton_Click);
            this.NextButton.MouseDown += new System.Windows.Forms.MouseEventHandler(this.NextButton_MouseDown);
            this.NextButton.MouseEnter += new System.EventHandler(this.VerticalScrollBar_Enter);
            this.NextButton.MouseUp += new System.Windows.Forms.MouseEventHandler(this.NextButton_MouseUp);
            // 
            // BackButton
            // 
            this.BackButton.AutoSize = true;
            this.BackButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.BackButton.FlatAppearance.BorderSize = 0;
            this.BackButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BackButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BackButton.Location = new System.Drawing.Point(0, 0);
            this.BackButton.Margin = new System.Windows.Forms.Padding(0);
            this.BackButton.Name = "BackButton";
            this.BackButton.Size = new System.Drawing.Size(7, 25);
            this.BackButton.TabIndex = 1;
            this.BackButton.Text = "▲";
            this.BackButton.UseMnemonic = false;
            this.BackButton.UseVisualStyleBackColor = false;
            this.BackButton.Click += new System.EventHandler(this.BackButton_Click);
            this.BackButton.MouseDown += new System.Windows.Forms.MouseEventHandler(this.BackButton_MouseDown);
            this.BackButton.MouseEnter += new System.EventHandler(this.VerticalScrollBar_Enter);
            this.BackButton.MouseUp += new System.Windows.Forms.MouseEventHandler(this.BackButton_MouseUp);
            // 
            // p_Slipper
            // 
            this.p_Slipper.Controls.Add(this.Slipper);
            this.p_Slipper.Dock = System.Windows.Forms.DockStyle.Fill;
            this.p_Slipper.Location = new System.Drawing.Point(0, 25);
            this.p_Slipper.Margin = new System.Windows.Forms.Padding(0);
            this.p_Slipper.Name = "p_Slipper";
            this.p_Slipper.Padding = new System.Windows.Forms.Padding(1);
            this.p_Slipper.Size = new System.Drawing.Size(7, 304);
            this.p_Slipper.TabIndex = 2;
            this.p_Slipper.MouseClick += new System.Windows.Forms.MouseEventHandler(this.p_Slipper_MouseClick);
            this.p_Slipper.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.p_Slipper_MouseDoubleClick);
            this.p_Slipper.MouseDown += new System.Windows.Forms.MouseEventHandler(this.p_Slipper_MouseDown);
            this.p_Slipper.MouseEnter += new System.EventHandler(this.VerticalScrollBar_Enter);
            this.p_Slipper.MouseUp += new System.Windows.Forms.MouseEventHandler(this.p_Slipper_MouseUp);
            // 
            // Slipper
            // 
            this.Slipper.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.Slipper.BackgroundImage = global::MiMFa.Properties.Resources.Hover;
            this.Slipper.FlatAppearance.BorderSize = 0;
            this.Slipper.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Slipper.Location = new System.Drawing.Point(1, 74);
            this.Slipper.Margin = new System.Windows.Forms.Padding(0);
            this.Slipper.Name = "Slipper";
            this.Slipper.Size = new System.Drawing.Size(5, 139);
            this.Slipper.TabIndex = 0;
            this.Slipper.TabStop = false;
            this.Slipper.UseVisualStyleBackColor = true;
            this.Slipper.LocationChanged += new System.EventHandler(this.Slipper_LocationChanged);
            this.Slipper.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Slipper_MouseDown);
            this.Slipper.MouseEnter += new System.EventHandler(this.VerticalScrollBar_Enter);
            this.Slipper.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Slipper_MouseMove);
            this.Slipper.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Slipper_MouseUp);
            // 
            // Tracer
            // 
            this.Tracer.Interval = 50;
            this.Tracer.Tick += new System.EventHandler(this.Tracer_Tick);
            // 
            // VerticalScrollBar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "VerticalScrollBar";
            this.Size = new System.Drawing.Size(7, 354);
            this.Load += new System.EventHandler(this.VerticalScrollBar_Load);
            this.SizeChanged += new System.EventHandler(this.VerticalScrollBar_SizeChanged);
            this.Enter += new System.EventHandler(this.VerticalScrollBar_Enter);
            this.Leave += new System.EventHandler(this.VerticalScrollBar_Leave);
            this.MouseEnter += new System.EventHandler(this.VerticalScrollBar_Enter);
            this.MouseLeave += new System.EventHandler(this.VerticalScrollBar_Leave);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.p_Slipper.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        public System.Windows.Forms.Button NextButton;
        private System.Windows.Forms.Button BackButton;
        private System.Windows.Forms.Panel p_Slipper;
        private System.Windows.Forms.Button Slipper;
        private System.Windows.Forms.Timer Tracer;
    }
}
