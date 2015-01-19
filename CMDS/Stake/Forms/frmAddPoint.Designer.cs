namespace Stake.Forms
{
    partial class frmAddPoint
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
            this.lblActiveAlign = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cmdSelectPoint = new System.Windows.Forms.Button();
            this.lblDesc = new System.Windows.Forms.Label();
            this.cmdAddPoint = new System.Windows.Forms.Button();
            this.tbxDescription = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // lblActiveAlign
            // 
            this.lblActiveAlign.AutoSize = true;
            this.lblActiveAlign.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblActiveAlign.Location = new System.Drawing.Point(12, 9);
            this.lblActiveAlign.Name = "lblActiveAlign";
            this.lblActiveAlign.Size = new System.Drawing.Size(117, 13);
            this.lblActiveAlign.TabIndex = 3;
            this.lblActiveAlign.Text = "ACTIVE ALIGNMENT: ";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(135, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "NAME";
            // 
            // cmdSelectPoint
            // 
            this.cmdSelectPoint.BackColor = System.Drawing.SystemColors.Control;
            this.cmdSelectPoint.Cursor = System.Windows.Forms.Cursors.Default;
            this.cmdSelectPoint.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdSelectPoint.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cmdSelectPoint.Location = new System.Drawing.Point(15, 25);
            this.cmdSelectPoint.Name = "cmdSelectPoint";
            this.cmdSelectPoint.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.cmdSelectPoint.Size = new System.Drawing.Size(107, 32);
            this.cmdSelectPoint.TabIndex = 5;
            this.cmdSelectPoint.Text = "SELECT POINT";
            this.cmdSelectPoint.UseVisualStyleBackColor = false;
            // 
            // lblDesc
            // 
            this.lblDesc.BackColor = System.Drawing.SystemColors.Control;
            this.lblDesc.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblDesc.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDesc.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblDesc.Location = new System.Drawing.Point(130, 33);
            this.lblDesc.Name = "lblDesc";
            this.lblDesc.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblDesc.Size = new System.Drawing.Size(80, 24);
            this.lblDesc.TabIndex = 6;
            this.lblDesc.Text = "DESCRIPTION";
            this.lblDesc.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // cmdAddPoint
            // 
            this.cmdAddPoint.BackColor = System.Drawing.SystemColors.Control;
            this.cmdAddPoint.Cursor = System.Windows.Forms.Cursors.Default;
            this.cmdAddPoint.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdAddPoint.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cmdAddPoint.Location = new System.Drawing.Point(412, 63);
            this.cmdAddPoint.Name = "cmdAddPoint";
            this.cmdAddPoint.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.cmdAddPoint.Size = new System.Drawing.Size(107, 32);
            this.cmdAddPoint.TabIndex = 7;
            this.cmdAddPoint.Text = "ADD POINT";
            this.cmdAddPoint.UseVisualStyleBackColor = false;
            // 
            // tbxDescription
            // 
            this.tbxDescription.Location = new System.Drawing.Point(233, 30);
            this.tbxDescription.Name = "tbxDescription";
            this.tbxDescription.Size = new System.Drawing.Size(286, 20);
            this.tbxDescription.TabIndex = 8;
            // 
            // frmAddPoint
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(531, 108);
            this.Controls.Add(this.tbxDescription);
            this.Controls.Add(this.cmdAddPoint);
            this.Controls.Add(this.cmdSelectPoint);
            this.Controls.Add(this.lblDesc);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblActiveAlign);
            this.Name = "frmAddPoint";
            this.Text = "AddPoint";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmAddPoint_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblActiveAlign;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.Button cmdSelectPoint;
        public System.Windows.Forms.Label lblDesc;
        public System.Windows.Forms.Button cmdAddPoint;
        private System.Windows.Forms.TextBox tbxDescription;
    }
}