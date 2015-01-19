namespace EW.Forms
{
    partial class frmGridSpacing
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
            this.cmdOK = new System.Windows.Forms.Button();
            this.Label2 = new System.Windows.Forms.Label();
            this.txtGridSpacing = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // cmdOK
            // 
            this.cmdOK.BackColor = System.Drawing.SystemColors.Control;
            this.cmdOK.Cursor = System.Windows.Forms.Cursors.Default;
            this.cmdOK.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdOK.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cmdOK.Location = new System.Drawing.Point(52, 41);
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.cmdOK.Size = new System.Drawing.Size(112, 32);
            this.cmdOK.TabIndex = 3;
            this.cmdOK.Text = "OK";
            this.cmdOK.UseVisualStyleBackColor = false;
            // 
            // Label2
            // 
            this.Label2.BackColor = System.Drawing.SystemColors.Control;
            this.Label2.Cursor = System.Windows.Forms.Cursors.Default;
            this.Label2.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Label2.Location = new System.Drawing.Point(12, 9);
            this.Label2.Name = "Label2";
            this.Label2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Label2.Size = new System.Drawing.Size(144, 24);
            this.Label2.TabIndex = 4;
            this.Label2.Text = "Point Grid Spacing";
            // 
            // txtGridSpacing
            // 
            this.txtGridSpacing.AcceptsReturn = true;
            this.txtGridSpacing.BackColor = System.Drawing.SystemColors.Window;
            this.txtGridSpacing.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtGridSpacing.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtGridSpacing.ForeColor = System.Drawing.SystemColors.WindowText;
            this.txtGridSpacing.Location = new System.Drawing.Point(164, 9);
            this.txtGridSpacing.MaxLength = 0;
            this.txtGridSpacing.Name = "txtGridSpacing";
            this.txtGridSpacing.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.txtGridSpacing.Size = new System.Drawing.Size(40, 23);
            this.txtGridSpacing.TabIndex = 5;
            this.txtGridSpacing.Text = "10";
            // 
            // frmGridSpacing
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(215, 82);
            this.Controls.Add(this.cmdOK);
            this.Controls.Add(this.Label2);
            this.Controls.Add(this.txtGridSpacing);
            this.Name = "frmGridSpacing";
            this.Text = "frmGridSpacing";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmGridSpacing_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Button cmdOK;
        public System.Windows.Forms.Label Label2;
        public System.Windows.Forms.TextBox txtGridSpacing;
    }
}