namespace Stake.Forms
{
    partial class frmBldgNames
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
            this.cmdDone = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // cmdDone
            // 
            this.cmdDone.BackColor = System.Drawing.SystemColors.Control;
            this.cmdDone.Cursor = System.Windows.Forms.Cursors.Default;
            this.cmdDone.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdDone.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cmdDone.Location = new System.Drawing.Point(47, 167);
            this.cmdDone.Name = "cmdDone";
            this.cmdDone.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.cmdDone.Size = new System.Drawing.Size(144, 40);
            this.cmdDone.TabIndex = 1;
            this.cmdDone.Text = "DONE";
            this.cmdDone.UseVisualStyleBackColor = false;
            this.cmdDone.Click += new System.EventHandler(this.cmdDone_Click);
            // 
            // frmBldgNames
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(237, 219);
            this.Controls.Add(this.cmdDone);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmBldgNames";
            this.Text = "STAKE - BUILDING NAMES";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmBldgNames_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.Button cmdDone;
    }
}