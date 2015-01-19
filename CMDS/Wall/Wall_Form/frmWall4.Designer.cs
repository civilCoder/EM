namespace Wall.Wall_Form
{
    partial class frmWall4
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
            this.gbx_Design = new System.Windows.Forms.GroupBox();
            this.Label31 = new System.Windows.Forms.Label();
            this.Label30 = new System.Windows.Forms.Label();
            this.Label29 = new System.Windows.Forms.Label();
            this.tbx_X0 = new System.Windows.Forms.TextBox();
            this.Label17 = new System.Windows.Forms.Label();
            this.Label18 = new System.Windows.Forms.Label();
            this.Label16 = new System.Windows.Forms.Label();
            this.Label15 = new System.Windows.Forms.Label();
            this.tbx_S0 = new System.Windows.Forms.TextBox();
            this.Label14 = new System.Windows.Forms.Label();
            this.Label1 = new System.Windows.Forms.Label();
            this.lblSlope1 = new System.Windows.Forms.Label();
            this.tbx_S2 = new System.Windows.Forms.TextBox();
            this.tbx_S1 = new System.Windows.Forms.TextBox();
            this.tbx_B2 = new System.Windows.Forms.TextBox();
            this.Label6 = new System.Windows.Forms.Label();
            this.tbx_SG = new System.Windows.Forms.TextBox();
            this.Label5 = new System.Windows.Forms.Label();
            this.tbx_B1 = new System.Windows.Forms.TextBox();
            this.Label4 = new System.Windows.Forms.Label();
            this.tbx_CF = new System.Windows.Forms.TextBox();
            this.Label3 = new System.Windows.Forms.Label();
            this.btnSelectBoundaryRef = new System.Windows.Forms.Button();
            this.btn_UpdateCurb = new System.Windows.Forms.Button();
            this.btnSelectBrklineDes = new System.Windows.Forms.Button();
            this.btn_LocateCurb = new System.Windows.Forms.Button();
            this.StatusStrip1 = new System.Windows.Forms.StatusStrip();
            this.ToolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.gbx_Design.SuspendLayout();
            this.StatusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbx_Design
            // 
            this.gbx_Design.Controls.Add(this.Label31);
            this.gbx_Design.Controls.Add(this.Label30);
            this.gbx_Design.Controls.Add(this.Label29);
            this.gbx_Design.Controls.Add(this.tbx_X0);
            this.gbx_Design.Controls.Add(this.Label17);
            this.gbx_Design.Controls.Add(this.Label18);
            this.gbx_Design.Controls.Add(this.Label16);
            this.gbx_Design.Controls.Add(this.Label15);
            this.gbx_Design.Controls.Add(this.tbx_S0);
            this.gbx_Design.Controls.Add(this.Label14);
            this.gbx_Design.Controls.Add(this.Label1);
            this.gbx_Design.Controls.Add(this.lblSlope1);
            this.gbx_Design.Controls.Add(this.tbx_S2);
            this.gbx_Design.Controls.Add(this.tbx_S1);
            this.gbx_Design.Controls.Add(this.tbx_B2);
            this.gbx_Design.Controls.Add(this.Label6);
            this.gbx_Design.Controls.Add(this.tbx_SG);
            this.gbx_Design.Controls.Add(this.Label5);
            this.gbx_Design.Controls.Add(this.tbx_B1);
            this.gbx_Design.Controls.Add(this.Label4);
            this.gbx_Design.Controls.Add(this.tbx_CF);
            this.gbx_Design.Controls.Add(this.Label3);
            this.gbx_Design.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))));
            this.gbx_Design.Location = new System.Drawing.Point(216, 12);
            this.gbx_Design.Name = "gbx_Design";
            this.gbx_Design.Size = new System.Drawing.Size(348, 274);
            this.gbx_Design.TabIndex = 58;
            this.gbx_Design.TabStop = false;
            this.gbx_Design.Text = "DESIGN PARAMETERS";
            // 
            // Label31
            // 
            this.Label31.AutoSize = true;
            this.Label31.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.Label31.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.Label31.Location = new System.Drawing.Point(260, 57);
            this.Label31.Name = "Label31";
            this.Label31.Size = new System.Drawing.Size(20, 13);
            this.Label31.TabIndex = 43;
            this.Label31.Text = "CF";
            // 
            // Label30
            // 
            this.Label30.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.Label30.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.Label30.Location = new System.Drawing.Point(6, 243);
            this.Label30.Name = "Label30";
            this.Label30.Size = new System.Drawing.Size(122, 13);
            this.Label30.TabIndex = 42;
            this.Label30.Text = "* TARGET WIDTH";
            // 
            // Label29
            // 
            this.Label29.AutoSize = true;
            this.Label29.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.Label29.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.Label29.Location = new System.Drawing.Point(258, 15);
            this.Label29.Name = "Label29";
            this.Label29.Size = new System.Drawing.Size(25, 13);
            this.Label29.TabIndex = 41;
            this.Label29.Text = "X0*";
            this.Label29.Visible = false;
            // 
            // tbx_X0
            // 
            this.tbx_X0.AccessibleRole = System.Windows.Forms.AccessibleRole.ButtonDropDownGrid;
            this.tbx_X0.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.tbx_X0.Location = new System.Drawing.Point(251, 31);
            this.tbx_X0.Name = "tbx_X0";
            this.tbx_X0.Size = new System.Drawing.Size(30, 21);
            this.tbx_X0.TabIndex = 15;
            this.tbx_X0.Text = "35.0";
            this.tbx_X0.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tbx_X0.Visible = false;
            // 
            // Label17
            // 
            this.Label17.AutoSize = true;
            this.Label17.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.Label17.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.Label17.Location = new System.Drawing.Point(258, 189);
            this.Label17.Name = "Label17";
            this.Label17.Size = new System.Drawing.Size(19, 13);
            this.Label17.TabIndex = 39;
            this.Label17.Text = "B2";
            // 
            // Label18
            // 
            this.Label18.AutoSize = true;
            this.Label18.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.Label18.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.Label18.Location = new System.Drawing.Point(258, 97);
            this.Label18.Name = "Label18";
            this.Label18.Size = new System.Drawing.Size(19, 13);
            this.Label18.TabIndex = 38;
            this.Label18.Text = "B1";
            // 
            // Label16
            // 
            this.Label16.AutoSize = true;
            this.Label16.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.Label16.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.Label16.Location = new System.Drawing.Point(316, 141);
            this.Label16.Name = "Label16";
            this.Label16.Size = new System.Drawing.Size(13, 13);
            this.Label16.TabIndex = 37;
            this.Label16.Text = "S";
            // 
            // Label15
            // 
            this.Label15.AutoSize = true;
            this.Label15.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.Label15.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.Label15.Location = new System.Drawing.Point(313, 15);
            this.Label15.Name = "Label15";
            this.Label15.Size = new System.Drawing.Size(19, 13);
            this.Label15.TabIndex = 36;
            this.Label15.Text = "S0";
            // 
            // tbx_S0
            // 
            this.tbx_S0.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.tbx_S0.Location = new System.Drawing.Point(303, 31);
            this.tbx_S0.Name = "tbx_S0";
            this.tbx_S0.Size = new System.Drawing.Size(36, 21);
            this.tbx_S0.TabIndex = 16;
            this.tbx_S0.Text = "0.02";
            this.tbx_S0.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // Label14
            // 
            this.Label14.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.Label14.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.Label14.Location = new System.Drawing.Point(6, 34);
            this.Label14.Name = "Label14";
            this.Label14.Size = new System.Drawing.Size(228, 13);
            this.Label14.TabIndex = 34;
            this.Label14.Text = "PAVEMENT SLOPE";
            // 
            // Label1
            // 
            this.Label1.AutoSize = true;
            this.Label1.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.Label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.Label1.Location = new System.Drawing.Point(313, 189);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(19, 13);
            this.Label1.TabIndex = 33;
            this.Label1.Text = "S2";
            // 
            // lblSlope1
            // 
            this.lblSlope1.AutoSize = true;
            this.lblSlope1.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.lblSlope1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblSlope1.Location = new System.Drawing.Point(313, 97);
            this.lblSlope1.Name = "lblSlope1";
            this.lblSlope1.Size = new System.Drawing.Size(19, 13);
            this.lblSlope1.TabIndex = 32;
            this.lblSlope1.Text = "S1";
            // 
            // tbx_S2
            // 
            this.tbx_S2.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.tbx_S2.Location = new System.Drawing.Point(303, 205);
            this.tbx_S2.Name = "tbx_S2";
            this.tbx_S2.Size = new System.Drawing.Size(36, 21);
            this.tbx_S2.TabIndex = 20;
            this.tbx_S2.Text = "0.02";
            this.tbx_S2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // tbx_S1
            // 
            this.tbx_S1.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.tbx_S1.Location = new System.Drawing.Point(303, 113);
            this.tbx_S1.Name = "tbx_S1";
            this.tbx_S1.Size = new System.Drawing.Size(36, 21);
            this.tbx_S1.TabIndex = 18;
            this.tbx_S1.Text = "0.02";
            this.tbx_S1.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // tbx_B2
            // 
            this.tbx_B2.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.tbx_B2.Location = new System.Drawing.Point(251, 205);
            this.tbx_B2.Name = "tbx_B2";
            this.tbx_B2.Size = new System.Drawing.Size(30, 21);
            this.tbx_B2.TabIndex = 19;
            this.tbx_B2.Text = "2.0";
            this.tbx_B2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // Label6
            // 
            this.Label6.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.Label6.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.Label6.Location = new System.Drawing.Point(6, 208);
            this.Label6.Name = "Label6";
            this.Label6.Size = new System.Drawing.Size(248, 13);
            this.Label6.TabIndex = 25;
            this.Label6.Text = "BENCH2 (B2) @ BOUNDARY (WIDTH AND SLOPE)";
            // 
            // tbx_SG
            // 
            this.tbx_SG.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.tbx_SG.Location = new System.Drawing.Point(303, 157);
            this.tbx_SG.Name = "tbx_SG";
            this.tbx_SG.Size = new System.Drawing.Size(36, 21);
            this.tbx_SG.TabIndex = 14;
            this.tbx_SG.Text = "0.50";
            this.tbx_SG.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // Label5
            // 
            this.Label5.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.Label5.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.Label5.Location = new System.Drawing.Point(6, 160);
            this.Label5.Name = "Label5";
            this.Label5.Size = new System.Drawing.Size(228, 13);
            this.Label5.TabIndex = 23;
            this.Label5.Text = "SLOPE GRADE (2:1= 0.50)";
            // 
            // tbx_B1
            // 
            this.tbx_B1.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.tbx_B1.Location = new System.Drawing.Point(251, 113);
            this.tbx_B1.Name = "tbx_B1";
            this.tbx_B1.Size = new System.Drawing.Size(30, 21);
            this.tbx_B1.TabIndex = 17;
            this.tbx_B1.Text = "2.0";
            this.tbx_B1.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // Label4
            // 
            this.Label4.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.Label4.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.Label4.Location = new System.Drawing.Point(6, 116);
            this.Label4.Name = "Label4";
            this.Label4.Size = new System.Drawing.Size(228, 17);
            this.Label4.TabIndex = 21;
            this.Label4.Text = "BENCH1 (B1) @ CURB (WIDTH AND SLOPE)";
            // 
            // tbx_CF
            // 
            this.tbx_CF.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.tbx_CF.Location = new System.Drawing.Point(253, 73);
            this.tbx_CF.Name = "tbx_CF";
            this.tbx_CF.Size = new System.Drawing.Size(30, 21);
            this.tbx_CF.TabIndex = 13;
            this.tbx_CF.Text = "6.0";
            this.tbx_CF.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // Label3
            // 
            this.Label3.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.Label3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.Label3.Location = new System.Drawing.Point(6, 76);
            this.Label3.Name = "Label3";
            this.Label3.Size = new System.Drawing.Size(228, 13);
            this.Label3.TabIndex = 19;
            this.Label3.Text = "CURB HEIGHT (IN) (0 FOR NONE)";
            // 
            // btnSelectBoundaryRef
            // 
            this.btnSelectBoundaryRef.Location = new System.Drawing.Point(12, 12);
            this.btnSelectBoundaryRef.Name = "btnSelectBoundaryRef";
            this.btnSelectBoundaryRef.Size = new System.Drawing.Size(173, 48);
            this.btnSelectBoundaryRef.TabIndex = 54;
            this.btnSelectBoundaryRef.Text = "SELECT \r\nBOUNDARY REFERENCE\r\n(2D or 3D)";
            this.btnSelectBoundaryRef.UseVisualStyleBackColor = true;
            this.btnSelectBoundaryRef.Click += new System.EventHandler(this.btnSelectBoundaryRef_Click);
            // 
            // btn_UpdateCurb
            // 
            this.btn_UpdateCurb.Enabled = false;
            this.btn_UpdateCurb.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btn_UpdateCurb.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.btn_UpdateCurb.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_UpdateCurb.Location = new System.Drawing.Point(427, 292);
            this.btn_UpdateCurb.Name = "btn_UpdateCurb";
            this.btn_UpdateCurb.Size = new System.Drawing.Size(137, 36);
            this.btn_UpdateCurb.TabIndex = 57;
            this.btn_UpdateCurb.Text = "UPDATE CURB LIMITS";
            this.btn_UpdateCurb.UseVisualStyleBackColor = true;
            this.btn_UpdateCurb.Click += new System.EventHandler(this.btn_UpdateCurb_Click);
            // 
            // btnSelectBrklineDes
            // 
            this.btnSelectBrklineDes.Location = new System.Drawing.Point(12, 70);
            this.btnSelectBrklineDes.Name = "btnSelectBrklineDes";
            this.btnSelectBrklineDes.Size = new System.Drawing.Size(173, 46);
            this.btnSelectBrklineDes.TabIndex = 55;
            this.btnSelectBrklineDes.Text = "SELECT \r\nDESIGN BREAKLINE";
            this.btnSelectBrklineDes.UseVisualStyleBackColor = true;
            this.btnSelectBrklineDes.Click += new System.EventHandler(this.btnSelectBrklineDes_Click);
            // 
            // btn_LocateCurb
            // 
            this.btn_LocateCurb.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btn_LocateCurb.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.btn_LocateCurb.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_LocateCurb.Location = new System.Drawing.Point(216, 289);
            this.btn_LocateCurb.Name = "btn_LocateCurb";
            this.btn_LocateCurb.Size = new System.Drawing.Size(137, 36);
            this.btn_LocateCurb.TabIndex = 56;
            this.btn_LocateCurb.Text = "LOCATE CURB LIMITS";
            this.btn_LocateCurb.UseVisualStyleBackColor = true;
            this.btn_LocateCurb.Click += new System.EventHandler(this.btn_LocateCurb_Click);
            // 
            // StatusStrip1
            // 
            this.StatusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripStatusLabel1});
            this.StatusStrip1.Location = new System.Drawing.Point(0, 402);
            this.StatusStrip1.Name = "StatusStrip1";
            this.StatusStrip1.Size = new System.Drawing.Size(574, 22);
            this.StatusStrip1.TabIndex = 59;
            this.StatusStrip1.Text = "StatusStrip1";
            // 
            // ToolStripStatusLabel1
            // 
            this.ToolStripStatusLabel1.Name = "ToolStripStatusLabel1";
            this.ToolStripStatusLabel1.Size = new System.Drawing.Size(86, 17);
            this.ToolStripStatusLabel1.Text = "ACTIVE ALIGN:";
            // 
            // frmWall4
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(574, 424);
            this.Controls.Add(this.StatusStrip1);
            this.Controls.Add(this.gbx_Design);
            this.Controls.Add(this.btnSelectBoundaryRef);
            this.Controls.Add(this.btn_UpdateCurb);
            this.Controls.Add(this.btnSelectBrklineDes);
            this.Controls.Add(this.btn_LocateCurb);
            this.Name = "frmWall4";
            this.Text = "frmWall4";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmWall4_FormClosing);
            this.gbx_Design.ResumeLayout(false);
            this.gbx_Design.PerformLayout();
            this.StatusStrip1.ResumeLayout(false);
            this.StatusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.GroupBox gbx_Design;
        internal System.Windows.Forms.Label Label31;
        internal System.Windows.Forms.Label Label30;
        internal System.Windows.Forms.Label Label29;
        public System.Windows.Forms.TextBox tbx_X0;
        internal System.Windows.Forms.Label Label17;
        internal System.Windows.Forms.Label Label18;
        internal System.Windows.Forms.Label Label16;
        internal System.Windows.Forms.Label Label15;
        public System.Windows.Forms.TextBox tbx_S0;
        internal System.Windows.Forms.Label Label14;
        internal System.Windows.Forms.Label Label1;
        internal System.Windows.Forms.Label lblSlope1;
        public System.Windows.Forms.TextBox tbx_S2;
        public System.Windows.Forms.TextBox tbx_S1;
        public System.Windows.Forms.TextBox tbx_B2;
        internal System.Windows.Forms.Label Label6;
        public System.Windows.Forms.TextBox tbx_SG;
        internal System.Windows.Forms.Label Label5;
        public System.Windows.Forms.TextBox tbx_B1;
        internal System.Windows.Forms.Label Label4;
        public System.Windows.Forms.TextBox tbx_CF;
        internal System.Windows.Forms.Label Label3;
        internal System.Windows.Forms.StatusStrip StatusStrip1;
        internal System.Windows.Forms.ToolStripStatusLabel ToolStripStatusLabel1;
        public System.Windows.Forms.Button btnSelectBoundaryRef;
        public System.Windows.Forms.Button btn_UpdateCurb;
        public System.Windows.Forms.Button btnSelectBrklineDes;
        public System.Windows.Forms.Button btn_LocateCurb;
    }
}