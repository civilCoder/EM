namespace Stake.Forms
{
    partial class frmPoints
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
            this.lbxPoints = new System.Windows.Forms.CheckedListBox();
            this.cmdCreateCutsheet = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lbxPoints
            // 
            this.lbxPoints.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbxPoints.FormattingEnabled = true;
            this.lbxPoints.Location = new System.Drawing.Point(9, 12);
            this.lbxPoints.MultiColumn = true;
            this.lbxPoints.Name = "lbxPoints";
            this.lbxPoints.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.lbxPoints.Size = new System.Drawing.Size(196, 79);
            this.lbxPoints.TabIndex = 4;
            // 
            // cmdCreateCutsheet
            // 
            this.cmdCreateCutsheet.BackColor = System.Drawing.SystemColors.Control;
            this.cmdCreateCutsheet.Cursor = System.Windows.Forms.Cursors.Default;
            this.cmdCreateCutsheet.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdCreateCutsheet.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cmdCreateCutsheet.Location = new System.Drawing.Point(9, 106);
            this.cmdCreateCutsheet.Name = "cmdCreateCutsheet";
            this.cmdCreateCutsheet.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.cmdCreateCutsheet.Size = new System.Drawing.Size(128, 40);
            this.cmdCreateCutsheet.TabIndex = 3;
            this.cmdCreateCutsheet.Text = "CREATE CUTSHEET";
            this.cmdCreateCutsheet.UseVisualStyleBackColor = false;
            this.cmdCreateCutsheet.Click += new System.EventHandler(this.cmdCreateCutsheet_Click);
            // 
            // frmPoints
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(217, 158);
            this.Controls.Add(this.lbxPoints);
            this.Controls.Add(this.cmdCreateCutsheet);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmPoints";
            this.Text = "STAKE - POINT LIST";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmPoints_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        internal System.Windows.Forms.CheckedListBox lbxPoints;
        public System.Windows.Forms.Button cmdCreateCutsheet;
    }
}