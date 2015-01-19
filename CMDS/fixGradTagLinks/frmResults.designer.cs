namespace fixGradeTagLinks
{
    partial class frmResults
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose (bool disposing)
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
        private void InitializeComponent ()
        {
            this.pBar1 = new System.Windows.Forms.ProgressBar();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnProceed = new System.Windows.Forms.Button();
            this.btnSAL = new System.Windows.Forms.Button();
            this.btnUSAL = new System.Windows.Forms.Button();
            this.btnTest = new System.Windows.Forms.Button();
            this.btnPass = new System.Windows.Forms.Button();
            this.lbxFix = new System.Windows.Forms.ListBox();
            this.lbxAdd = new System.Windows.Forms.ListBox();
            this.lblCount1 = new System.Windows.Forms.Label();
            this.lblCount2 = new System.Windows.Forms.Label();
            this.lblCountFix = new System.Windows.Forms.Label();
            this.lblCountAdd = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // pBar1
            // 
            this.pBar1.Location = new System.Drawing.Point(14, 69);
            this.pBar1.Name = "pBar1";
            this.pBar1.Size = new System.Drawing.Size(237, 36);
            this.pBar1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(14, 205);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(112, 55);
            this.label1.TabIndex = 3;
            this.label1.Text = "Tags with Errors: \r\nRE-TAG";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(144, 205);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(104, 55);
            this.label2.TabIndex = 4;
            this.label2.Text = "Newly Linked Blocks for Review";
            this.label2.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // btnProceed
            // 
            this.btnProceed.Location = new System.Drawing.Point(13, 12);
            this.btnProceed.Name = "btnProceed";
            this.btnProceed.Size = new System.Drawing.Size(238, 51);
            this.btnProceed.TabIndex = 5;
            this.btnProceed.Text = "PROCEED";
            this.btnProceed.UseVisualStyleBackColor = true;
            this.btnProceed.Click += new System.EventHandler(this.btnProceed_Click);
            // 
            // btnSAL
            // 
            this.btnSAL.Location = new System.Drawing.Point(14, 111);
            this.btnSAL.Name = "btnSAL";
            this.btnSAL.Size = new System.Drawing.Size(115, 41);
            this.btnSAL.TabIndex = 10;
            this.btnSAL.Text = "run SAL";
            this.btnSAL.UseVisualStyleBackColor = true;
            this.btnSAL.Click += new System.EventHandler(this.btnSAL_Click);
            // 
            // btnUSAL
            // 
            this.btnUSAL.Location = new System.Drawing.Point(135, 111);
            this.btnUSAL.Name = "btnUSAL";
            this.btnUSAL.Size = new System.Drawing.Size(115, 41);
            this.btnUSAL.TabIndex = 11;
            this.btnUSAL.Text = "run USAL";
            this.btnUSAL.UseVisualStyleBackColor = true;
            this.btnUSAL.Click += new System.EventHandler(this.btnUSAL_Click);
            // 
            // btnTest
            // 
            this.btnTest.Location = new System.Drawing.Point(14, 161);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(115, 41);
            this.btnTest.TabIndex = 12;
            this.btnTest.Text = "TEST UP/DOWN";
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // btnPass
            // 
            this.btnPass.Location = new System.Drawing.Point(135, 161);
            this.btnPass.Name = "btnPass";
            this.btnPass.Size = new System.Drawing.Size(116, 41);
            this.btnPass.TabIndex = 14;
            this.btnPass.Text = "PASS";
            this.btnPass.UseVisualStyleBackColor = true;
            this.btnPass.Click += new System.EventHandler(this.btnPass_Click);
            // 
            // lbxFix
            // 
            this.lbxFix.AllowDrop = true;
            this.lbxFix.FormattingEnabled = true;
            this.lbxFix.Location = new System.Drawing.Point(14, 290);
            this.lbxFix.Name = "lbxFix";
            this.lbxFix.Size = new System.Drawing.Size(112, 225);
            this.lbxFix.TabIndex = 17;
            // 
            // lbxAdd
            // 
            this.lbxAdd.AllowDrop = true;
            this.lbxAdd.FormattingEnabled = true;
            this.lbxAdd.Location = new System.Drawing.Point(139, 290);
            this.lbxAdd.Name = "lbxAdd";
            this.lbxAdd.Size = new System.Drawing.Size(112, 225);
            this.lbxAdd.TabIndex = 18;
            // 
            // lblCount1
            // 
            this.lblCount1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCount1.Location = new System.Drawing.Point(17, 259);
            this.lblCount1.Name = "lblCount1";
            this.lblCount1.Size = new System.Drawing.Size(52, 21);
            this.lblCount1.TabIndex = 19;
            this.lblCount1.Text = "Count =";
            this.lblCount1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblCount2
            // 
            this.lblCount2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCount2.Location = new System.Drawing.Point(140, 259);
            this.lblCount2.Name = "lblCount2";
            this.lblCount2.Size = new System.Drawing.Size(55, 21);
            this.lblCount2.TabIndex = 20;
            this.lblCount2.Text = "Count =";
            this.lblCount2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblCountFix
            // 
            this.lblCountFix.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCountFix.Location = new System.Drawing.Point(67, 259);
            this.lblCountFix.Name = "lblCountFix";
            this.lblCountFix.Size = new System.Drawing.Size(59, 21);
            this.lblCountFix.TabIndex = 21;
            this.lblCountFix.Text = "0";
            this.lblCountFix.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblCountAdd
            // 
            this.lblCountAdd.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCountAdd.Location = new System.Drawing.Point(193, 259);
            this.lblCountAdd.Name = "lblCountAdd";
            this.lblCountAdd.Size = new System.Drawing.Size(55, 21);
            this.lblCountAdd.TabIndex = 22;
            this.lblCountAdd.Text = "0";
            this.lblCountAdd.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // frmResults
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(266, 527);
            this.Controls.Add(this.lblCountAdd);
            this.Controls.Add(this.lblCountFix);
            this.Controls.Add(this.lblCount2);
            this.Controls.Add(this.lblCount1);
            this.Controls.Add(this.lbxAdd);
            this.Controls.Add(this.lbxFix);
            this.Controls.Add(this.btnPass);
            this.Controls.Add(this.btnTest);
            this.Controls.Add(this.btnUSAL);
            this.Controls.Add(this.btnSAL);
            this.Controls.Add(this.btnProceed);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pBar1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmResults";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "CHECK LINKED TAGS";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmResults_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        internal System.Windows.Forms.ProgressBar pBar1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnProceed;
        private System.Windows.Forms.Button btnSAL;
        private System.Windows.Forms.Button btnUSAL;
        private System.Windows.Forms.Button btnTest;
        private System.Windows.Forms.Button btnPass;
        internal System.Windows.Forms.ListBox lbxFix;
        internal System.Windows.Forms.ListBox lbxAdd;
        private System.Windows.Forms.Label lblCount1;
        private System.Windows.Forms.Label lblCount2;
        internal System.Windows.Forms.Label lblCountFix;
        internal System.Windows.Forms.Label lblCountAdd;
    }
}