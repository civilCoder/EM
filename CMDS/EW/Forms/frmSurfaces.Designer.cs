namespace EW.Forms
{
    partial class frmSurfaces
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
            this.lblBASE = new System.Windows.Forms.Label();
            this.lblCOMP = new System.Windows.Forms.Label();
            this.lboxBASE = new System.Windows.Forms.ListBox();
            this.lboxCOMP = new System.Windows.Forms.ListBox();
            this.cmdOK = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblBASE
            // 
            this.lblBASE.BackColor = System.Drawing.SystemColors.Control;
            this.lblBASE.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblBASE.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBASE.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblBASE.Location = new System.Drawing.Point(12, 15);
            this.lblBASE.Name = "lblBASE";
            this.lblBASE.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblBASE.Size = new System.Drawing.Size(120, 16);
            this.lblBASE.TabIndex = 5;
            this.lblBASE.Text = "BASE SURFACE";
            // 
            // lblCOMP
            // 
            this.lblCOMP.BackColor = System.Drawing.SystemColors.Control;
            this.lblCOMP.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblCOMP.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCOMP.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblCOMP.Location = new System.Drawing.Point(12, 87);
            this.lblCOMP.Name = "lblCOMP";
            this.lblCOMP.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblCOMP.Size = new System.Drawing.Size(152, 16);
            this.lblCOMP.TabIndex = 6;
            this.lblCOMP.Text = "COMPARISON SURFACE";
            // 
            // lboxBASE
            // 
            this.lboxBASE.BackColor = System.Drawing.SystemColors.Window;
            this.lboxBASE.Cursor = System.Windows.Forms.Cursors.Default;
            this.lboxBASE.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lboxBASE.ForeColor = System.Drawing.SystemColors.WindowText;
            this.lboxBASE.Location = new System.Drawing.Point(204, 15);
            this.lboxBASE.Name = "lboxBASE";
            this.lboxBASE.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lboxBASE.Size = new System.Drawing.Size(136, 17);
            this.lboxBASE.TabIndex = 7;
            // 
            // lboxCOMP
            // 
            this.lboxCOMP.BackColor = System.Drawing.SystemColors.Window;
            this.lboxCOMP.Cursor = System.Windows.Forms.Cursors.Default;
            this.lboxCOMP.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lboxCOMP.ForeColor = System.Drawing.SystemColors.WindowText;
            this.lboxCOMP.Location = new System.Drawing.Point(204, 87);
            this.lboxCOMP.Name = "lboxCOMP";
            this.lboxCOMP.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lboxCOMP.Size = new System.Drawing.Size(136, 17);
            this.lboxCOMP.TabIndex = 8;
            // 
            // cmdOK
            // 
            this.cmdOK.BackColor = System.Drawing.SystemColors.Control;
            this.cmdOK.Cursor = System.Windows.Forms.Cursors.Default;
            this.cmdOK.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdOK.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cmdOK.Location = new System.Drawing.Point(220, 134);
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.cmdOK.Size = new System.Drawing.Size(120, 32);
            this.cmdOK.TabIndex = 9;
            this.cmdOK.Text = "OK";
            this.cmdOK.UseVisualStyleBackColor = false;
            this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
            // 
            // frmSurfaces
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(352, 178);
            this.Controls.Add(this.lblBASE);
            this.Controls.Add(this.lblCOMP);
            this.Controls.Add(this.lboxBASE);
            this.Controls.Add(this.lboxCOMP);
            this.Controls.Add(this.cmdOK);
            this.Name = "frmSurfaces";
            this.Text = "frmSurfaces";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmSurfaces_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.Label lblBASE;
        public System.Windows.Forms.Label lblCOMP;
        public System.Windows.Forms.ListBox lboxBASE;
        public System.Windows.Forms.ListBox lboxCOMP;
        public System.Windows.Forms.Button cmdOK;
    }
}