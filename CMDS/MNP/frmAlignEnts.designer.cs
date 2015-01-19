namespace MNP
{
    partial class frmAlignEnts
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
            this.cmdUpdate = new System.Windows.Forms.Button();
            this.cmdAddCurve = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // cmdUpdate
            // 
            this.cmdUpdate.BackColor = System.Drawing.SystemColors.Control;
            this.cmdUpdate.Cursor = System.Windows.Forms.Cursors.Default;
            this.cmdUpdate.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdUpdate.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cmdUpdate.Location = new System.Drawing.Point(473, 221);
            this.cmdUpdate.Name = "cmdUpdate";
            this.cmdUpdate.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.cmdUpdate.Size = new System.Drawing.Size(112, 40);
            this.cmdUpdate.TabIndex = 8;
            this.cmdUpdate.Text = "Update";
            this.cmdUpdate.UseVisualStyleBackColor = false;
            this.cmdUpdate.Click += new System.EventHandler(this.cmdUpdate_Click);
            // 
            // cmdAddCurve
            // 
            this.cmdAddCurve.BackColor = System.Drawing.SystemColors.Control;
            this.cmdAddCurve.Cursor = System.Windows.Forms.Cursors.Default;
            this.cmdAddCurve.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdAddCurve.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cmdAddCurve.Location = new System.Drawing.Point(2, 221);
            this.cmdAddCurve.Name = "cmdAddCurve";
            this.cmdAddCurve.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.cmdAddCurve.Size = new System.Drawing.Size(112, 40);
            this.cmdAddCurve.TabIndex = 10;
            this.cmdAddCurve.Text = "Add Curve";
            this.cmdAddCurve.UseVisualStyleBackColor = false;
            this.cmdAddCurve.Click += new System.EventHandler(this.cmdAddCurve_Click);
            // 
            // frmAlignEnts
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(587, 272);
            this.Controls.Add(this.cmdAddCurve);
            this.Controls.Add(this.cmdUpdate);
            this.Name = "frmAlignEnts";
            this.Text = "frmAlignEnts";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmAlignEnts_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.Button cmdUpdate;
        public System.Windows.Forms.Button cmdAddCurve;
    }
}