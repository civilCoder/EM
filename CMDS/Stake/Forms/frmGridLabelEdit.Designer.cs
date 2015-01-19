namespace Stake.Forms
{
    partial class frmGridLabelEdit
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
            this.Label1 = new System.Windows.Forms.Label();
            this.lblGridLabel = new System.Windows.Forms.Label();
            this.Label3 = new System.Windows.Forms.Label();
            this.txtGridLabelEdit = new System.Windows.Forms.TextBox();
            this.cmdDone = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // Label1
            // 
            this.Label1.BackColor = System.Drawing.SystemColors.Control;
            this.Label1.Cursor = System.Windows.Forms.Cursors.Default;
            this.Label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Label1.Location = new System.Drawing.Point(9, 19);
            this.Label1.Name = "Label1";
            this.Label1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Label1.Size = new System.Drawing.Size(112, 16);
            this.Label1.TabIndex = 5;
            this.Label1.Text = "CURRENT LABEL:";
            // 
            // lblGridLabel
            // 
            this.lblGridLabel.BackColor = System.Drawing.SystemColors.Control;
            this.lblGridLabel.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblGridLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblGridLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblGridLabel.Location = new System.Drawing.Point(129, 19);
            this.lblGridLabel.Name = "lblGridLabel";
            this.lblGridLabel.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblGridLabel.Size = new System.Drawing.Size(32, 16);
            this.lblGridLabel.TabIndex = 6;
            this.lblGridLabel.Text = "A";
            this.lblGridLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // Label3
            // 
            this.Label3.BackColor = System.Drawing.SystemColors.Control;
            this.Label3.Cursor = System.Windows.Forms.Cursors.Default;
            this.Label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label3.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Label3.Location = new System.Drawing.Point(169, 19);
            this.Label3.Name = "Label3";
            this.Label3.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Label3.Size = new System.Drawing.Size(88, 16);
            this.Label3.TabIndex = 7;
            this.Label3.Text = "CHANGE TO:";
            // 
            // txtGridLabelEdit
            // 
            this.txtGridLabelEdit.AcceptsReturn = true;
            this.txtGridLabelEdit.BackColor = System.Drawing.SystemColors.Window;
            this.txtGridLabelEdit.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtGridLabelEdit.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtGridLabelEdit.ForeColor = System.Drawing.SystemColors.WindowText;
            this.txtGridLabelEdit.Location = new System.Drawing.Point(257, 15);
            this.txtGridLabelEdit.MaxLength = 0;
            this.txtGridLabelEdit.Name = "txtGridLabelEdit";
            this.txtGridLabelEdit.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.txtGridLabelEdit.Size = new System.Drawing.Size(56, 20);
            this.txtGridLabelEdit.TabIndex = 8;
            // 
            // cmdDone
            // 
            this.cmdDone.BackColor = System.Drawing.SystemColors.Control;
            this.cmdDone.Cursor = System.Windows.Forms.Cursors.Default;
            this.cmdDone.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdDone.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cmdDone.Location = new System.Drawing.Point(329, 11);
            this.cmdDone.Name = "cmdDone";
            this.cmdDone.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.cmdDone.Size = new System.Drawing.Size(88, 32);
            this.cmdDone.TabIndex = 9;
            this.cmdDone.Text = "DONE";
            this.cmdDone.UseVisualStyleBackColor = false;
            // 
            // frmGridLabelEdit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(425, 53);
            this.Controls.Add(this.Label1);
            this.Controls.Add(this.lblGridLabel);
            this.Controls.Add(this.Label3);
            this.Controls.Add(this.txtGridLabelEdit);
            this.Controls.Add(this.cmdDone);
            this.Name = "frmGridLabelEdit";
            this.Text = "STAKE - BUILDING GRID LABEL EDIT";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmGridLabelEdit_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Label Label1;
        public System.Windows.Forms.Label lblGridLabel;
        public System.Windows.Forms.Label Label3;
        public System.Windows.Forms.TextBox txtGridLabelEdit;
        public System.Windows.Forms.Button cmdDone;
    }
}