namespace EW.Forms
{
    partial class frmEarthwork
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
            this.cmdSetup = new System.Windows.Forms.Button();
            this.cmdBuildAreas = new System.Windows.Forms.Button();
            this.cmdGetSpreadsheet = new System.Windows.Forms.Button();
            this.cmdGetSegs = new System.Windows.Forms.Button();
            this.cmdMakeSurfaceSG = new System.Windows.Forms.Button();
            this.cmdMakeSurfaceOX = new System.Windows.Forms.Button();
            this.cmdResetSG = new System.Windows.Forms.Button();
            this.cmdResetOX = new System.Windows.Forms.Button();
            this.Frame1 = new System.Windows.Forms.GroupBox();
            this.cmdGrid = new System.Windows.Forms.Button();
            this.cmdTest = new System.Windows.Forms.Button();
            this.cmdMakeSurfaceBOT = new System.Windows.Forms.Button();
            this.cmdMakeSurfaceVolEXIST_BOT = new System.Windows.Forms.Button();
            this.cmdMakeSurfaceVolBOT_SG = new System.Windows.Forms.Button();
            this.cmdBalanceSite = new System.Windows.Forms.Button();
            this.cmdUpdateWS = new System.Windows.Forms.Button();
            this.cmdDisplaySections = new System.Windows.Forms.Button();
            this.ckbDebug = new System.Windows.Forms.CheckBox();
            this.Frame1.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmdSetup
            // 
            this.cmdSetup.BackColor = System.Drawing.SystemColors.Control;
            this.cmdSetup.Cursor = System.Windows.Forms.Cursors.Default;
            this.cmdSetup.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdSetup.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cmdSetup.Location = new System.Drawing.Point(12, 3);
            this.cmdSetup.Name = "cmdSetup";
            this.cmdSetup.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.cmdSetup.Size = new System.Drawing.Size(240, 32);
            this.cmdSetup.TabIndex = 14;
            this.cmdSetup.Text = "EARTHWORK SETUP";
            this.cmdSetup.UseVisualStyleBackColor = false;
            this.cmdSetup.Click += new System.EventHandler(this.cmdSetup_Click);
            // 
            // cmdBuildAreas
            // 
            this.cmdBuildAreas.BackColor = System.Drawing.SystemColors.Control;
            this.cmdBuildAreas.Cursor = System.Windows.Forms.Cursors.Default;
            this.cmdBuildAreas.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdBuildAreas.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cmdBuildAreas.Location = new System.Drawing.Point(12, 41);
            this.cmdBuildAreas.Name = "cmdBuildAreas";
            this.cmdBuildAreas.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.cmdBuildAreas.Size = new System.Drawing.Size(240, 32);
            this.cmdBuildAreas.TabIndex = 15;
            this.cmdBuildAreas.Text = "BUILD AREAS";
            this.cmdBuildAreas.UseVisualStyleBackColor = false;
            this.cmdBuildAreas.Click += new System.EventHandler(this.cmdBuildAreas_Click);
            // 
            // cmdGetSpreadsheet
            // 
            this.cmdGetSpreadsheet.BackColor = System.Drawing.SystemColors.Control;
            this.cmdGetSpreadsheet.Cursor = System.Windows.Forms.Cursors.Default;
            this.cmdGetSpreadsheet.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdGetSpreadsheet.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cmdGetSpreadsheet.Location = new System.Drawing.Point(12, 79);
            this.cmdGetSpreadsheet.Name = "cmdGetSpreadsheet";
            this.cmdGetSpreadsheet.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.cmdGetSpreadsheet.Size = new System.Drawing.Size(240, 32);
            this.cmdGetSpreadsheet.TabIndex = 16;
            this.cmdGetSpreadsheet.Text = "CREATE/OPEN  \"EW\" SPREADSHEET";
            this.cmdGetSpreadsheet.UseVisualStyleBackColor = false;
            this.cmdGetSpreadsheet.Click += new System.EventHandler(this.cmdGetSpreadsheet_Click);
            // 
            // cmdGetSegs
            // 
            this.cmdGetSegs.BackColor = System.Drawing.SystemColors.Control;
            this.cmdGetSegs.Cursor = System.Windows.Forms.Cursors.Default;
            this.cmdGetSegs.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdGetSegs.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cmdGetSegs.Location = new System.Drawing.Point(12, 117);
            this.cmdGetSegs.Name = "cmdGetSegs";
            this.cmdGetSegs.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.cmdGetSegs.Size = new System.Drawing.Size(240, 28);
            this.cmdGetSegs.TabIndex = 17;
            this.cmdGetSegs.Text = "COPY CPNT-ON SURFACE DATA";
            this.cmdGetSegs.UseVisualStyleBackColor = false;
            this.cmdGetSegs.Click += new System.EventHandler(this.cmdGetSegs_Click);
            // 
            // cmdMakeSurfaceSG
            // 
            this.cmdMakeSurfaceSG.BackColor = System.Drawing.SystemColors.Control;
            this.cmdMakeSurfaceSG.Cursor = System.Windows.Forms.Cursors.Default;
            this.cmdMakeSurfaceSG.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdMakeSurfaceSG.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cmdMakeSurfaceSG.Location = new System.Drawing.Point(12, 151);
            this.cmdMakeSurfaceSG.Name = "cmdMakeSurfaceSG";
            this.cmdMakeSurfaceSG.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.cmdMakeSurfaceSG.Size = new System.Drawing.Size(192, 28);
            this.cmdMakeSurfaceSG.TabIndex = 18;
            this.cmdMakeSurfaceSG.Text = "MAKE SURFACE \"SG\" ";
            this.cmdMakeSurfaceSG.UseVisualStyleBackColor = false;
            this.cmdMakeSurfaceSG.Click += new System.EventHandler(this.cmdMakeSurfaceSG_Click);
            // 
            // cmdMakeSurfaceOX
            // 
            this.cmdMakeSurfaceOX.BackColor = System.Drawing.SystemColors.Control;
            this.cmdMakeSurfaceOX.Cursor = System.Windows.Forms.Cursors.Default;
            this.cmdMakeSurfaceOX.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdMakeSurfaceOX.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cmdMakeSurfaceOX.Location = new System.Drawing.Point(12, 183);
            this.cmdMakeSurfaceOX.Name = "cmdMakeSurfaceOX";
            this.cmdMakeSurfaceOX.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.cmdMakeSurfaceOX.Size = new System.Drawing.Size(192, 28);
            this.cmdMakeSurfaceOX.TabIndex = 19;
            this.cmdMakeSurfaceOX.Text = "MAKE SURFACE \"OX\"";
            this.cmdMakeSurfaceOX.UseVisualStyleBackColor = false;
            this.cmdMakeSurfaceOX.Click += new System.EventHandler(this.cmdMakeSurfaceOX_Click);
            // 
            // cmdResetSG
            // 
            this.cmdResetSG.BackColor = System.Drawing.SystemColors.Control;
            this.cmdResetSG.Cursor = System.Windows.Forms.Cursors.Default;
            this.cmdResetSG.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdResetSG.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cmdResetSG.Location = new System.Drawing.Point(204, 151);
            this.cmdResetSG.Name = "cmdResetSG";
            this.cmdResetSG.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.cmdResetSG.Size = new System.Drawing.Size(48, 28);
            this.cmdResetSG.TabIndex = 20;
            this.cmdResetSG.Text = "RESET";
            this.cmdResetSG.UseVisualStyleBackColor = false;
            this.cmdResetSG.Click += new System.EventHandler(this.cmdResetSG_Click);
            // 
            // cmdResetOX
            // 
            this.cmdResetOX.BackColor = System.Drawing.SystemColors.Control;
            this.cmdResetOX.Cursor = System.Windows.Forms.Cursors.Default;
            this.cmdResetOX.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdResetOX.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cmdResetOX.Location = new System.Drawing.Point(204, 183);
            this.cmdResetOX.Name = "cmdResetOX";
            this.cmdResetOX.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.cmdResetOX.Size = new System.Drawing.Size(48, 28);
            this.cmdResetOX.TabIndex = 21;
            this.cmdResetOX.Text = "RESET";
            this.cmdResetOX.UseVisualStyleBackColor = false;
            this.cmdResetOX.Click += new System.EventHandler(this.cmdResetOX_Click);
            // 
            // Frame1
            // 
            this.Frame1.BackColor = System.Drawing.SystemColors.Menu;
            this.Frame1.Controls.Add(this.cmdGrid);
            this.Frame1.Controls.Add(this.cmdTest);
            this.Frame1.Controls.Add(this.cmdMakeSurfaceBOT);
            this.Frame1.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Frame1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Frame1.Location = new System.Drawing.Point(12, 217);
            this.Frame1.Name = "Frame1";
            this.Frame1.Padding = new System.Windows.Forms.Padding(0);
            this.Frame1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Frame1.Size = new System.Drawing.Size(240, 124);
            this.Frame1.TabIndex = 22;
            this.Frame1.TabStop = false;
            this.Frame1.Text = "MAKE SURFACE \"BOT\"";
            // 
            // cmdGrid
            // 
            this.cmdGrid.BackColor = System.Drawing.SystemColors.Control;
            this.cmdGrid.Cursor = System.Windows.Forms.Cursors.Default;
            this.cmdGrid.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdGrid.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cmdGrid.Location = new System.Drawing.Point(10, 16);
            this.cmdGrid.Name = "cmdGrid";
            this.cmdGrid.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.cmdGrid.Size = new System.Drawing.Size(220, 28);
            this.cmdGrid.TabIndex = 0;
            this.cmdGrid.Text = "SET UP GRID POINTS";
            this.cmdGrid.UseVisualStyleBackColor = false;
            this.cmdGrid.Click += new System.EventHandler(this.cmdGrid_Click);
            // 
            // cmdTest
            // 
            this.cmdTest.BackColor = System.Drawing.SystemColors.Control;
            this.cmdTest.Cursor = System.Windows.Forms.Cursors.Default;
            this.cmdTest.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdTest.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cmdTest.Location = new System.Drawing.Point(10, 50);
            this.cmdTest.Name = "cmdTest";
            this.cmdTest.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.cmdTest.Size = new System.Drawing.Size(220, 28);
            this.cmdTest.TabIndex = 1;
            this.cmdTest.Text = "DETERMINE BOTTOM ELEVATION";
            this.cmdTest.UseVisualStyleBackColor = false;
            this.cmdTest.Click += new System.EventHandler(this.cmdTest_Click);
            // 
            // cmdMakeSurfaceBOT
            // 
            this.cmdMakeSurfaceBOT.BackColor = System.Drawing.SystemColors.Control;
            this.cmdMakeSurfaceBOT.Cursor = System.Windows.Forms.Cursors.Default;
            this.cmdMakeSurfaceBOT.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdMakeSurfaceBOT.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cmdMakeSurfaceBOT.Location = new System.Drawing.Point(10, 84);
            this.cmdMakeSurfaceBOT.Name = "cmdMakeSurfaceBOT";
            this.cmdMakeSurfaceBOT.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.cmdMakeSurfaceBOT.Size = new System.Drawing.Size(220, 28);
            this.cmdMakeSurfaceBOT.TabIndex = 2;
            this.cmdMakeSurfaceBOT.Text = "MAKE SURFACE \"BOT\"";
            this.cmdMakeSurfaceBOT.UseVisualStyleBackColor = false;
            this.cmdMakeSurfaceBOT.Click += new System.EventHandler(this.cmdMakeSurfaceBOT_Click);
            // 
            // cmdMakeSurfaceVolEXIST_BOT
            // 
            this.cmdMakeSurfaceVolEXIST_BOT.BackColor = System.Drawing.SystemColors.Control;
            this.cmdMakeSurfaceVolEXIST_BOT.Cursor = System.Windows.Forms.Cursors.Default;
            this.cmdMakeSurfaceVolEXIST_BOT.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdMakeSurfaceVolEXIST_BOT.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cmdMakeSurfaceVolEXIST_BOT.Location = new System.Drawing.Point(12, 347);
            this.cmdMakeSurfaceVolEXIST_BOT.Name = "cmdMakeSurfaceVolEXIST_BOT";
            this.cmdMakeSurfaceVolEXIST_BOT.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.cmdMakeSurfaceVolEXIST_BOT.Size = new System.Drawing.Size(240, 28);
            this.cmdMakeSurfaceVolEXIST_BOT.TabIndex = 23;
            this.cmdMakeSurfaceVolEXIST_BOT.Text = "MAKE \"CUT\" VOLUME SURFACE";
            this.cmdMakeSurfaceVolEXIST_BOT.UseVisualStyleBackColor = false;
            this.cmdMakeSurfaceVolEXIST_BOT.Click += new System.EventHandler(this.cmdMakeSurfaceVolEXIST_BOT_Click);
            // 
            // cmdMakeSurfaceVolBOT_SG
            // 
            this.cmdMakeSurfaceVolBOT_SG.BackColor = System.Drawing.SystemColors.Control;
            this.cmdMakeSurfaceVolBOT_SG.Cursor = System.Windows.Forms.Cursors.Default;
            this.cmdMakeSurfaceVolBOT_SG.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdMakeSurfaceVolBOT_SG.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cmdMakeSurfaceVolBOT_SG.Location = new System.Drawing.Point(12, 379);
            this.cmdMakeSurfaceVolBOT_SG.Name = "cmdMakeSurfaceVolBOT_SG";
            this.cmdMakeSurfaceVolBOT_SG.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.cmdMakeSurfaceVolBOT_SG.Size = new System.Drawing.Size(240, 28);
            this.cmdMakeSurfaceVolBOT_SG.TabIndex = 24;
            this.cmdMakeSurfaceVolBOT_SG.Text = "MAKE \"FILL\" VOLUME SURFACE";
            this.cmdMakeSurfaceVolBOT_SG.UseVisualStyleBackColor = false;
            this.cmdMakeSurfaceVolBOT_SG.Click += new System.EventHandler(this.cmdMakeSurfaceVolBOT_SG_Click);
            // 
            // cmdBalanceSite
            // 
            this.cmdBalanceSite.BackColor = System.Drawing.SystemColors.Control;
            this.cmdBalanceSite.Cursor = System.Windows.Forms.Cursors.Default;
            this.cmdBalanceSite.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdBalanceSite.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cmdBalanceSite.Location = new System.Drawing.Point(12, 411);
            this.cmdBalanceSite.Name = "cmdBalanceSite";
            this.cmdBalanceSite.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.cmdBalanceSite.Size = new System.Drawing.Size(240, 28);
            this.cmdBalanceSite.TabIndex = 25;
            this.cmdBalanceSite.Text = "BALANCE SITE";
            this.cmdBalanceSite.UseVisualStyleBackColor = false;
            this.cmdBalanceSite.Click += new System.EventHandler(this.cmdBalanceSite_Click);
            // 
            // cmdUpdateWS
            // 
            this.cmdUpdateWS.BackColor = System.Drawing.SystemColors.Control;
            this.cmdUpdateWS.Cursor = System.Windows.Forms.Cursors.Default;
            this.cmdUpdateWS.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdUpdateWS.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cmdUpdateWS.Location = new System.Drawing.Point(12, 443);
            this.cmdUpdateWS.Name = "cmdUpdateWS";
            this.cmdUpdateWS.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.cmdUpdateWS.Size = new System.Drawing.Size(240, 28);
            this.cmdUpdateWS.TabIndex = 26;
            this.cmdUpdateWS.Text = "DISPLAY/UPDATE WORKSHEET";
            this.cmdUpdateWS.UseVisualStyleBackColor = false;
            this.cmdUpdateWS.Click += new System.EventHandler(this.cmdUpdateWS_Click);
            // 
            // cmdDisplaySections
            // 
            this.cmdDisplaySections.BackColor = System.Drawing.SystemColors.Control;
            this.cmdDisplaySections.Cursor = System.Windows.Forms.Cursors.Default;
            this.cmdDisplaySections.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdDisplaySections.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cmdDisplaySections.Location = new System.Drawing.Point(12, 475);
            this.cmdDisplaySections.Name = "cmdDisplaySections";
            this.cmdDisplaySections.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.cmdDisplaySections.Size = new System.Drawing.Size(240, 28);
            this.cmdDisplaySections.TabIndex = 27;
            this.cmdDisplaySections.Text = "DISPLAY SECTIONS";
            this.cmdDisplaySections.UseVisualStyleBackColor = false;
            this.cmdDisplaySections.Click += new System.EventHandler(this.cmdDisplaySections_Click);
            // 
            // ckbDebug
            // 
            this.ckbDebug.BackColor = System.Drawing.SystemColors.Control;
            this.ckbDebug.Cursor = System.Windows.Forms.Cursors.Default;
            this.ckbDebug.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckbDebug.ForeColor = System.Drawing.SystemColors.ControlText;
            this.ckbDebug.Location = new System.Drawing.Point(12, 509);
            this.ckbDebug.Name = "ckbDebug";
            this.ckbDebug.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.ckbDebug.Size = new System.Drawing.Size(160, 24);
            this.ckbDebug.TabIndex = 28;
            this.ckbDebug.Text = "DEBUG MODE";
            this.ckbDebug.UseVisualStyleBackColor = false;
            this.ckbDebug.CheckedChanged += new System.EventHandler(this.ckbDebug_CheckStateChanged);
            // 
            // frmEarthwork
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(266, 509);
            this.Controls.Add(this.ckbDebug);
            this.Controls.Add(this.cmdMakeSurfaceVolEXIST_BOT);
            this.Controls.Add(this.cmdMakeSurfaceVolBOT_SG);
            this.Controls.Add(this.cmdBalanceSite);
            this.Controls.Add(this.cmdUpdateWS);
            this.Controls.Add(this.cmdDisplaySections);
            this.Controls.Add(this.Frame1);
            this.Controls.Add(this.cmdMakeSurfaceSG);
            this.Controls.Add(this.cmdMakeSurfaceOX);
            this.Controls.Add(this.cmdResetSG);
            this.Controls.Add(this.cmdResetOX);
            this.Controls.Add(this.cmdGetSegs);
            this.Controls.Add(this.cmdGetSpreadsheet);
            this.Controls.Add(this.cmdBuildAreas);
            this.Controls.Add(this.cmdSetup);
            this.Name = "frmEarthwork";
            this.Text = "frmEarthwork";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmEarthwork_FormClosing);
            this.Frame1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.Button cmdSetup;
        public System.Windows.Forms.Button cmdBuildAreas;
        public System.Windows.Forms.Button cmdGetSpreadsheet;
        public System.Windows.Forms.Button cmdGetSegs;
        public System.Windows.Forms.Button cmdMakeSurfaceSG;
        public System.Windows.Forms.Button cmdMakeSurfaceOX;
        public System.Windows.Forms.Button cmdResetSG;
        public System.Windows.Forms.Button cmdResetOX;
        public System.Windows.Forms.GroupBox Frame1;
        public System.Windows.Forms.Button cmdGrid;
        public System.Windows.Forms.Button cmdTest;
        public System.Windows.Forms.Button cmdMakeSurfaceBOT;
        public System.Windows.Forms.Button cmdMakeSurfaceVolEXIST_BOT;
        public System.Windows.Forms.Button cmdMakeSurfaceVolBOT_SG;
        public System.Windows.Forms.Button cmdBalanceSite;
        public System.Windows.Forms.Button cmdUpdateWS;
        public System.Windows.Forms.Button cmdDisplaySections;
        public System.Windows.Forms.CheckBox ckbDebug;
    }
}