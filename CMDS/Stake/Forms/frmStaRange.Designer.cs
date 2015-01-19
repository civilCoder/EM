namespace Stake.Forms
{
    partial class frmStaRange
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
            this.lblStaBeg = new System.Windows.Forms.Label();
            this.tbxStaStart = new System.Windows.Forms.TextBox();
            this.lblStaEnd = new System.Windows.Forms.Label();
            this.tbxStaEnd = new System.Windows.Forms.TextBox();
            this.cmdDone = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblStaBeg
            // 
            this.lblStaBeg.BackColor = System.Drawing.SystemColors.Control;
            this.lblStaBeg.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblStaBeg.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStaBeg.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblStaBeg.Location = new System.Drawing.Point(12, 9);
            this.lblStaBeg.Name = "lblStaBeg";
            this.lblStaBeg.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblStaBeg.Size = new System.Drawing.Size(112, 16);
            this.lblStaBeg.TabIndex = 5;
            this.lblStaBeg.Text = "STATION START";
            // 
            // tbxStaStart
            // 
            this.tbxStaStart.AcceptsReturn = true;
            this.tbxStaStart.BackColor = System.Drawing.SystemColors.Window;
            this.tbxStaStart.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.tbxStaStart.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbxStaStart.ForeColor = System.Drawing.SystemColors.WindowText;
            this.tbxStaStart.Location = new System.Drawing.Point(121, 6);
            this.tbxStaStart.MaxLength = 0;
            this.tbxStaStart.Name = "tbxStaStart";
            this.tbxStaStart.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.tbxStaStart.Size = new System.Drawing.Size(88, 20);
            this.tbxStaStart.TabIndex = 6;
            // 
            // lblStaEnd
            // 
            this.lblStaEnd.BackColor = System.Drawing.SystemColors.Control;
            this.lblStaEnd.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblStaEnd.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStaEnd.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblStaEnd.Location = new System.Drawing.Point(12, 49);
            this.lblStaEnd.Name = "lblStaEnd";
            this.lblStaEnd.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblStaEnd.Size = new System.Drawing.Size(112, 16);
            this.lblStaEnd.TabIndex = 7;
            this.lblStaEnd.Text = "STATION END";
            // 
            // tbxStaEnd
            // 
            this.tbxStaEnd.AcceptsReturn = true;
            this.tbxStaEnd.BackColor = System.Drawing.SystemColors.Window;
            this.tbxStaEnd.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.tbxStaEnd.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbxStaEnd.ForeColor = System.Drawing.SystemColors.WindowText;
            this.tbxStaEnd.Location = new System.Drawing.Point(121, 45);
            this.tbxStaEnd.MaxLength = 0;
            this.tbxStaEnd.Name = "tbxStaEnd";
            this.tbxStaEnd.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.tbxStaEnd.Size = new System.Drawing.Size(88, 20);
            this.tbxStaEnd.TabIndex = 8;
            // 
            // cmdDone
            // 
            this.cmdDone.BackColor = System.Drawing.SystemColors.Control;
            this.cmdDone.Cursor = System.Windows.Forms.Cursors.Default;
            this.cmdDone.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdDone.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cmdDone.Location = new System.Drawing.Point(224, 33);
            this.cmdDone.Name = "cmdDone";
            this.cmdDone.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.cmdDone.Size = new System.Drawing.Size(96, 32);
            this.cmdDone.TabIndex = 9;
            this.cmdDone.Text = "CONTINUE";
            this.cmdDone.UseVisualStyleBackColor = false;
            this.cmdDone.Click += new System.EventHandler(this.cmdDone_Click);
            // 
            // frmStaRange
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(330, 79);
            this.Controls.Add(this.lblStaBeg);
            this.Controls.Add(this.tbxStaStart);
            this.Controls.Add(this.lblStaEnd);
            this.Controls.Add(this.tbxStaEnd);
            this.Controls.Add(this.cmdDone);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmStaRange";
            this.Text = "STAKE - STATION RANGE";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmStaRange_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Label lblStaBeg;
        public System.Windows.Forms.TextBox tbxStaStart;
        public System.Windows.Forms.Label lblStaEnd;
        public System.Windows.Forms.TextBox tbxStaEnd;
        public System.Windows.Forms.Button cmdDone;
    }
}