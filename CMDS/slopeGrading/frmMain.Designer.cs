namespace slopeGrading
{
    partial class frmMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose ( bool disposing )
        {
            if (disposing && (components != null))
            {
                components.Dispose ();
            }
            base.Dispose ( disposing );
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent ()
        {
            this.btnBuildSlope = new System.Windows.Forms.Button();
            this.gbxB1 = new System.Windows.Forms.GroupBox();
            this.tbxB1Width = new System.Windows.Forms.TextBox();
            this.tbxB1Slope = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cbxB1 = new System.Windows.Forms.CheckBox();
            this.gbxB2 = new System.Windows.Forms.GroupBox();
            this.tbxB2Slope = new System.Windows.Forms.TextBox();
            this.tbxB2Width = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.cbxB2 = new System.Windows.Forms.CheckBox();
            this.gbxSlope = new System.Windows.Forms.GroupBox();
            this.tbxSlope = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.gbxSurface = new System.Windows.Forms.GroupBox();
            this.lbxSurfaceDES = new System.Windows.Forms.ListBox();
            this.lbxSurfaceTAR = new System.Windows.Forms.ListBox();
            this.label7 = new System.Windows.Forms.Label();
            this.gbxInterval = new System.Windows.Forms.GroupBox();
            this.tbxInterval = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.gbxB1.SuspendLayout();
            this.gbxB2.SuspendLayout();
            this.gbxSlope.SuspendLayout();
            this.gbxSurface.SuspendLayout();
            this.gbxInterval.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnBuildSlope
            // 
            this.btnBuildSlope.Location = new System.Drawing.Point(91, 270);
            this.btnBuildSlope.Name = "btnBuildSlope";
            this.btnBuildSlope.Size = new System.Drawing.Size(96, 28);
            this.btnBuildSlope.TabIndex = 0;
            this.btnBuildSlope.Text = "BUILD SLOPE";
            this.btnBuildSlope.UseVisualStyleBackColor = true;
            this.btnBuildSlope.Click += new System.EventHandler(this.btnBuildSlope_Click);
            // 
            // gbxB1
            // 
            this.gbxB1.Controls.Add(this.tbxB1Width);
            this.gbxB1.Controls.Add(this.tbxB1Slope);
            this.gbxB1.Controls.Add(this.label2);
            this.gbxB1.Controls.Add(this.label1);
            this.gbxB1.Controls.Add(this.cbxB1);
            this.gbxB1.Location = new System.Drawing.Point(9, 106);
            this.gbxB1.Name = "gbxB1";
            this.gbxB1.Size = new System.Drawing.Size(189, 76);
            this.gbxB1.TabIndex = 1;
            this.gbxB1.TabStop = false;
            this.gbxB1.Text = "B1";
            // 
            // tbxB1Width
            // 
            this.tbxB1Width.Location = new System.Drawing.Point(82, 47);
            this.tbxB1Width.Name = "tbxB1Width";
            this.tbxB1Width.Size = new System.Drawing.Size(44, 20);
            this.tbxB1Width.TabIndex = 8;
            // 
            // tbxB1Slope
            // 
            this.tbxB1Slope.Location = new System.Drawing.Point(134, 47);
            this.tbxB1Slope.Name = "tbxB1Slope";
            this.tbxB1Slope.Size = new System.Drawing.Size(44, 20);
            this.tbxB1Slope.TabIndex = 7;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(130, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(48, 26);
            this.label2.TabIndex = 4;
            this.label2.Text = "BENCH SLOPE";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(81, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 27);
            this.label1.TabIndex = 2;
            this.label1.Text = "BENCH WIDTH";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // cbxB1
            // 
            this.cbxB1.AutoSize = true;
            this.cbxB1.Location = new System.Drawing.Point(6, 49);
            this.cbxB1.Name = "cbxB1";
            this.cbxB1.Size = new System.Drawing.Size(63, 17);
            this.cbxB1.TabIndex = 0;
            this.cbxB1.Text = "BENCH";
            this.cbxB1.UseVisualStyleBackColor = true;
            // 
            // gbxB2
            // 
            this.gbxB2.Controls.Add(this.tbxB2Slope);
            this.gbxB2.Controls.Add(this.tbxB2Width);
            this.gbxB2.Controls.Add(this.label3);
            this.gbxB2.Controls.Add(this.label4);
            this.gbxB2.Controls.Add(this.cbxB2);
            this.gbxB2.Location = new System.Drawing.Point(9, 188);
            this.gbxB2.Name = "gbxB2";
            this.gbxB2.Size = new System.Drawing.Size(189, 76);
            this.gbxB2.TabIndex = 2;
            this.gbxB2.TabStop = false;
            this.gbxB2.Text = "B2";
            // 
            // tbxB2Slope
            // 
            this.tbxB2Slope.Location = new System.Drawing.Point(134, 46);
            this.tbxB2Slope.Name = "tbxB2Slope";
            this.tbxB2Slope.Size = new System.Drawing.Size(44, 20);
            this.tbxB2Slope.TabIndex = 6;
            // 
            // tbxB2Width
            // 
            this.tbxB2Width.Location = new System.Drawing.Point(82, 46);
            this.tbxB2Width.Name = "tbxB2Width";
            this.tbxB2Width.Size = new System.Drawing.Size(44, 20);
            this.tbxB2Width.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(130, 16);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(48, 26);
            this.label3.TabIndex = 4;
            this.label3.Text = "BENCH SLOPE";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(81, 16);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(46, 27);
            this.label4.TabIndex = 2;
            this.label4.Text = "BENCH WIDTH";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // cbxB2
            // 
            this.cbxB2.AutoSize = true;
            this.cbxB2.Location = new System.Drawing.Point(6, 49);
            this.cbxB2.Name = "cbxB2";
            this.cbxB2.Size = new System.Drawing.Size(63, 17);
            this.cbxB2.TabIndex = 0;
            this.cbxB2.Text = "BENCH";
            this.cbxB2.UseVisualStyleBackColor = true;
            // 
            // gbxSlope
            // 
            this.gbxSlope.Controls.Add(this.tbxSlope);
            this.gbxSlope.Controls.Add(this.label5);
            this.gbxSlope.Location = new System.Drawing.Point(204, 106);
            this.gbxSlope.Name = "gbxSlope";
            this.gbxSlope.Size = new System.Drawing.Size(71, 76);
            this.gbxSlope.TabIndex = 3;
            this.gbxSlope.TabStop = false;
            // 
            // tbxSlope
            // 
            this.tbxSlope.Location = new System.Drawing.Point(9, 46);
            this.tbxSlope.Name = "tbxSlope";
            this.tbxSlope.Size = new System.Drawing.Size(44, 20);
            this.tbxSlope.TabIndex = 8;
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(6, 16);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 27);
            this.label5.TabIndex = 4;
            this.label5.Text = "SLOPE\r\n( ft/ft )";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(6, 16);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(101, 35);
            this.label6.TabIndex = 5;
            this.label6.Text = "SELECT TARGET SURFACE";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // gbxSurface
            // 
            this.gbxSurface.Controls.Add(this.lbxSurfaceDES);
            this.gbxSurface.Controls.Add(this.lbxSurfaceTAR);
            this.gbxSurface.Controls.Add(this.label7);
            this.gbxSurface.Controls.Add(this.label6);
            this.gbxSurface.Location = new System.Drawing.Point(9, 12);
            this.gbxSurface.Name = "gbxSurface";
            this.gbxSurface.Size = new System.Drawing.Size(266, 88);
            this.gbxSurface.TabIndex = 6;
            this.gbxSurface.TabStop = false;
            // 
            // lbxSurfaceDES
            // 
            this.lbxSurfaceDES.FormattingEnabled = true;
            this.lbxSurfaceDES.Location = new System.Drawing.Point(159, 50);
            this.lbxSurfaceDES.Name = "lbxSurfaceDES";
            this.lbxSurfaceDES.Size = new System.Drawing.Size(93, 17);
            this.lbxSurfaceDES.TabIndex = 8;
            // 
            // lbxSurfaceTAR
            // 
            this.lbxSurfaceTAR.FormattingEnabled = true;
            this.lbxSurfaceTAR.Location = new System.Drawing.Point(13, 50);
            this.lbxSurfaceTAR.Name = "lbxSurfaceTAR";
            this.lbxSurfaceTAR.Size = new System.Drawing.Size(93, 17);
            this.lbxSurfaceTAR.TabIndex = 7;
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(156, 16);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(104, 35);
            this.label7.TabIndex = 6;
            this.label7.Text = "SELECT DESIGN SURFACE";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // gbxInterval
            // 
            this.gbxInterval.Controls.Add(this.tbxInterval);
            this.gbxInterval.Controls.Add(this.label8);
            this.gbxInterval.Location = new System.Drawing.Point(204, 188);
            this.gbxInterval.Name = "gbxInterval";
            this.gbxInterval.Size = new System.Drawing.Size(70, 76);
            this.gbxInterval.TabIndex = 7;
            this.gbxInterval.TabStop = false;
            // 
            // tbxInterval
            // 
            this.tbxInterval.Location = new System.Drawing.Point(13, 45);
            this.tbxInterval.Name = "tbxInterval";
            this.tbxInterval.Size = new System.Drawing.Size(44, 20);
            this.tbxInterval.TabIndex = 9;
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(6, 16);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(60, 26);
            this.label8.TabIndex = 0;
            this.label8.Text = "INTERVAL (ft.)";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(279, 303);
            this.Controls.Add(this.gbxInterval);
            this.Controls.Add(this.gbxSurface);
            this.Controls.Add(this.gbxSlope);
            this.Controls.Add(this.gbxB2);
            this.Controls.Add(this.gbxB1);
            this.Controls.Add(this.btnBuildSlope);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Name = "frmMain";
            this.Text = " SLOPE GRADING";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.gbxB1.ResumeLayout(false);
            this.gbxB1.PerformLayout();
            this.gbxB2.ResumeLayout(false);
            this.gbxB2.PerformLayout();
            this.gbxSlope.ResumeLayout(false);
            this.gbxSlope.PerformLayout();
            this.gbxSurface.ResumeLayout(false);
            this.gbxInterval.ResumeLayout(false);
            this.gbxInterval.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnBuildSlope;
        private System.Windows.Forms.GroupBox gbxB1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox cbxB1;
        private System.Windows.Forms.GroupBox gbxB2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox cbxB2;
        private System.Windows.Forms.GroupBox gbxSlope;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox gbxSurface;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox tbxB2Width;
        private System.Windows.Forms.TextBox tbxB1Width;
        private System.Windows.Forms.TextBox tbxB1Slope;
        private System.Windows.Forms.TextBox tbxB2Slope;
        private System.Windows.Forms.TextBox tbxSlope;
        private System.Windows.Forms.ListBox lbxSurfaceDES;
        private System.Windows.Forms.ListBox lbxSurfaceTAR;
        private System.Windows.Forms.GroupBox gbxInterval;
        private System.Windows.Forms.TextBox tbxInterval;
        private System.Windows.Forms.Label label8;
    }
}