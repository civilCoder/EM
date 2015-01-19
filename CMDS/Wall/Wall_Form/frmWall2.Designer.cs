namespace Wall.Wall_Form
{
    partial class frmWall2
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
            this.btnUpdateLeaders = new System.Windows.Forms.Button();
            this.btn_Update = new System.Windows.Forms.Button();
            this.btn_BuildWallProfiles = new System.Windows.Forms.Button();
            this.gbx_MinCover_Freeboard = new System.Windows.Forms.GroupBox();
            this.tbx_Cover = new System.Windows.Forms.TextBox();
            this.Label12 = new System.Windows.Forms.Label();
            this.lblFreeBoardHeight = new System.Windows.Forms.Label();
            this.tbx_Freeboard = new System.Windows.Forms.TextBox();
            this.lblCoverUnits = new System.Windows.Forms.Label();
            this.lblFreeboardUnits = new System.Windows.Forms.Label();
            this.lblFreeBoardTolerance = new System.Windows.Forms.Label();
            this.tbx_FreeboardTolerance = new System.Windows.Forms.TextBox();
            this.Label13 = new System.Windows.Forms.Label();
            this.gbx_WallMaterial = new System.Windows.Forms.GroupBox();
            this.tbx_SpaceH = new System.Windows.Forms.TextBox();
            this.lblSpacingH = new System.Windows.Forms.Label();
            this.lblSpacingV = new System.Windows.Forms.Label();
            this.tbx_SpaceV = new System.Windows.Forms.TextBox();
            this.tbx_StepH = new System.Windows.Forms.TextBox();
            this.lblStepH = new System.Windows.Forms.Label();
            this.tbx_StepV = new System.Windows.Forms.TextBox();
            this.lblStepV = new System.Windows.Forms.Label();
            this.lblSpaceUnitsH = new System.Windows.Forms.Label();
            this.lblSpaceUnitsV = new System.Windows.Forms.Label();
            this.lblStepUnitsH = new System.Windows.Forms.Label();
            this.lblStepUnitsV = new System.Windows.Forms.Label();
            this.gbx_ScreenWall = new System.Windows.Forms.GroupBox();
            this.cbx_HeightAboveFooting = new System.Windows.Forms.CheckBox();
            this.cbx_HeightAboveGround = new System.Windows.Forms.CheckBox();
            this.gbx_WallType = new System.Windows.Forms.GroupBox();
            this.opt_PANEL = new System.Windows.Forms.RadioButton();
            this.opt_BLOCK = new System.Windows.Forms.RadioButton();
            this.btnCreateWallProfileView = new System.Windows.Forms.Button();
            this.btnSelectBrkline2 = new System.Windows.Forms.Button();
            this.btnSelectBrkline1 = new System.Windows.Forms.Button();
            this.btnSelectWallAlign = new System.Windows.Forms.Button();
            this.ToolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.StatusStrip1 = new System.Windows.Forms.StatusStrip();
            this.gbx_MinCover_Freeboard.SuspendLayout();
            this.gbx_WallMaterial.SuspendLayout();
            this.gbx_ScreenWall.SuspendLayout();
            this.gbx_WallType.SuspendLayout();
            this.StatusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnUpdateLeaders
            // 
            this.btnUpdateLeaders.BackColor = System.Drawing.SystemColors.Control;
            this.btnUpdateLeaders.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnUpdateLeaders.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.btnUpdateLeaders.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnUpdateLeaders.Location = new System.Drawing.Point(18, 332);
            this.btnUpdateLeaders.Name = "btnUpdateLeaders";
            this.btnUpdateLeaders.Size = new System.Drawing.Size(140, 36);
            this.btnUpdateLeaders.TabIndex = 71;
            this.btnUpdateLeaders.Text = "UPDATE LEADERS";
            this.btnUpdateLeaders.UseVisualStyleBackColor = false;
            this.btnUpdateLeaders.Visible = false;
            this.btnUpdateLeaders.Click += new System.EventHandler(this.btnUpdateLeaders_Click);
            // 
            // btn_Update
            // 
            this.btn_Update.BackColor = System.Drawing.SystemColors.Control;
            this.btn_Update.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btn_Update.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.btn_Update.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_Update.Location = new System.Drawing.Point(437, 332);
            this.btn_Update.Name = "btn_Update";
            this.btn_Update.Size = new System.Drawing.Size(140, 36);
            this.btn_Update.TabIndex = 70;
            this.btn_Update.Text = "UPDATE WALL DESIGN";
            this.btn_Update.UseVisualStyleBackColor = false;
            this.btn_Update.Click += new System.EventHandler(this.btn_Update_Click);
            // 
            // btn_BuildWallProfiles
            // 
            this.btn_BuildWallProfiles.BackColor = System.Drawing.SystemColors.Control;
            this.btn_BuildWallProfiles.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btn_BuildWallProfiles.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.btn_BuildWallProfiles.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_BuildWallProfiles.Location = new System.Drawing.Point(205, 332);
            this.btn_BuildWallProfiles.Name = "btn_BuildWallProfiles";
            this.btn_BuildWallProfiles.Size = new System.Drawing.Size(140, 36);
            this.btn_BuildWallProfiles.TabIndex = 69;
            this.btn_BuildWallProfiles.Text = "BUILD WALL LIMITS -  TOW && TOF";
            this.btn_BuildWallProfiles.UseVisualStyleBackColor = false;
            this.btn_BuildWallProfiles.Click += new System.EventHandler(this.btn_BuildWallProfiles_Click);
            // 
            // gbx_MinCover_Freeboard
            // 
            this.gbx_MinCover_Freeboard.BackColor = System.Drawing.Color.Transparent;
            this.gbx_MinCover_Freeboard.Controls.Add(this.tbx_Cover);
            this.gbx_MinCover_Freeboard.Controls.Add(this.Label12);
            this.gbx_MinCover_Freeboard.Controls.Add(this.lblFreeBoardHeight);
            this.gbx_MinCover_Freeboard.Controls.Add(this.tbx_Freeboard);
            this.gbx_MinCover_Freeboard.Controls.Add(this.lblCoverUnits);
            this.gbx_MinCover_Freeboard.Controls.Add(this.lblFreeboardUnits);
            this.gbx_MinCover_Freeboard.Controls.Add(this.lblFreeBoardTolerance);
            this.gbx_MinCover_Freeboard.Controls.Add(this.tbx_FreeboardTolerance);
            this.gbx_MinCover_Freeboard.Controls.Add(this.Label13);
            this.gbx_MinCover_Freeboard.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.gbx_MinCover_Freeboard.ForeColor = System.Drawing.SystemColors.ControlText;
            this.gbx_MinCover_Freeboard.Location = new System.Drawing.Point(202, 216);
            this.gbx_MinCover_Freeboard.Name = "gbx_MinCover_Freeboard";
            this.gbx_MinCover_Freeboard.Padding = new System.Windows.Forms.Padding(0);
            this.gbx_MinCover_Freeboard.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.gbx_MinCover_Freeboard.Size = new System.Drawing.Size(372, 110);
            this.gbx_MinCover_Freeboard.TabIndex = 68;
            this.gbx_MinCover_Freeboard.TabStop = false;
            this.gbx_MinCover_Freeboard.Text = "MINIMUM COVER/FREEBOARD";
            // 
            // tbx_Cover
            // 
            this.tbx_Cover.AcceptsReturn = true;
            this.tbx_Cover.BackColor = System.Drawing.SystemColors.Window;
            this.tbx_Cover.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.tbx_Cover.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.tbx_Cover.ForeColor = System.Drawing.SystemColors.WindowText;
            this.tbx_Cover.Location = new System.Drawing.Point(280, 17);
            this.tbx_Cover.MaxLength = 0;
            this.tbx_Cover.Name = "tbx_Cover";
            this.tbx_Cover.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.tbx_Cover.Size = new System.Drawing.Size(42, 21);
            this.tbx_Cover.TabIndex = 39;
            // 
            // Label12
            // 
            this.Label12.AutoSize = true;
            this.Label12.BackColor = System.Drawing.Color.Transparent;
            this.Label12.Cursor = System.Windows.Forms.Cursors.Default;
            this.Label12.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.Label12.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Label12.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.Label12.Location = new System.Drawing.Point(3, 22);
            this.Label12.Name = "Label12";
            this.Label12.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Label12.Size = new System.Drawing.Size(90, 13);
            this.Label12.TabIndex = 1;
            this.Label12.Text = "FOOTING COVER";
            // 
            // lblFreeBoardHeight
            // 
            this.lblFreeBoardHeight.AutoSize = true;
            this.lblFreeBoardHeight.BackColor = System.Drawing.Color.Transparent;
            this.lblFreeBoardHeight.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblFreeBoardHeight.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.lblFreeBoardHeight.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblFreeBoardHeight.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblFreeBoardHeight.Location = new System.Drawing.Point(3, 52);
            this.lblFreeBoardHeight.Name = "lblFreeBoardHeight";
            this.lblFreeBoardHeight.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblFreeBoardHeight.Size = new System.Drawing.Size(137, 13);
            this.lblFreeBoardHeight.TabIndex = 2;
            this.lblFreeBoardHeight.Text = "TOP OF WALL FREEBOARD";
            // 
            // tbx_Freeboard
            // 
            this.tbx_Freeboard.AcceptsReturn = true;
            this.tbx_Freeboard.BackColor = System.Drawing.SystemColors.Window;
            this.tbx_Freeboard.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.tbx_Freeboard.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.tbx_Freeboard.ForeColor = System.Drawing.SystemColors.WindowText;
            this.tbx_Freeboard.Location = new System.Drawing.Point(280, 49);
            this.tbx_Freeboard.MaxLength = 0;
            this.tbx_Freeboard.Name = "tbx_Freeboard";
            this.tbx_Freeboard.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.tbx_Freeboard.Size = new System.Drawing.Size(42, 21);
            this.tbx_Freeboard.TabIndex = 40;
            // 
            // lblCoverUnits
            // 
            this.lblCoverUnits.BackColor = System.Drawing.Color.Transparent;
            this.lblCoverUnits.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblCoverUnits.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblCoverUnits.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblCoverUnits.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblCoverUnits.Location = new System.Drawing.Point(323, 21);
            this.lblCoverUnits.Name = "lblCoverUnits";
            this.lblCoverUnits.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblCoverUnits.Size = new System.Drawing.Size(38, 16);
            this.lblCoverUnits.TabIndex = 4;
            this.lblCoverUnits.Text = "(FT.)";
            // 
            // lblFreeboardUnits
            // 
            this.lblFreeboardUnits.BackColor = System.Drawing.Color.Transparent;
            this.lblFreeboardUnits.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblFreeboardUnits.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblFreeboardUnits.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblFreeboardUnits.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblFreeboardUnits.Location = new System.Drawing.Point(323, 53);
            this.lblFreeboardUnits.Name = "lblFreeboardUnits";
            this.lblFreeboardUnits.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblFreeboardUnits.Size = new System.Drawing.Size(38, 16);
            this.lblFreeboardUnits.TabIndex = 5;
            this.lblFreeboardUnits.Text = "(FT.)";
            // 
            // lblFreeBoardTolerance
            // 
            this.lblFreeBoardTolerance.AutoSize = true;
            this.lblFreeBoardTolerance.BackColor = System.Drawing.Color.Transparent;
            this.lblFreeBoardTolerance.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblFreeBoardTolerance.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.lblFreeBoardTolerance.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblFreeBoardTolerance.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblFreeBoardTolerance.Location = new System.Drawing.Point(3, 82);
            this.lblFreeBoardTolerance.Name = "lblFreeBoardTolerance";
            this.lblFreeBoardTolerance.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblFreeBoardTolerance.Size = new System.Drawing.Size(148, 13);
            this.lblFreeBoardTolerance.TabIndex = 6;
            this.lblFreeBoardTolerance.Text = "FREEBOARD TOLERANCE +/-";
            // 
            // tbx_FreeboardTolerance
            // 
            this.tbx_FreeboardTolerance.AcceptsReturn = true;
            this.tbx_FreeboardTolerance.BackColor = System.Drawing.SystemColors.Window;
            this.tbx_FreeboardTolerance.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.tbx_FreeboardTolerance.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.tbx_FreeboardTolerance.ForeColor = System.Drawing.SystemColors.WindowText;
            this.tbx_FreeboardTolerance.Location = new System.Drawing.Point(280, 81);
            this.tbx_FreeboardTolerance.MaxLength = 0;
            this.tbx_FreeboardTolerance.Name = "tbx_FreeboardTolerance";
            this.tbx_FreeboardTolerance.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.tbx_FreeboardTolerance.Size = new System.Drawing.Size(42, 21);
            this.tbx_FreeboardTolerance.TabIndex = 41;
            // 
            // Label13
            // 
            this.Label13.BackColor = System.Drawing.Color.Transparent;
            this.Label13.Cursor = System.Windows.Forms.Cursors.Default;
            this.Label13.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.Label13.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Label13.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.Label13.Location = new System.Drawing.Point(323, 85);
            this.Label13.Name = "Label13";
            this.Label13.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Label13.Size = new System.Drawing.Size(38, 16);
            this.Label13.TabIndex = 8;
            this.Label13.Text = "(FT.)";
            // 
            // gbx_WallMaterial
            // 
            this.gbx_WallMaterial.BackColor = System.Drawing.Color.Transparent;
            this.gbx_WallMaterial.Controls.Add(this.tbx_SpaceH);
            this.gbx_WallMaterial.Controls.Add(this.lblSpacingH);
            this.gbx_WallMaterial.Controls.Add(this.lblSpacingV);
            this.gbx_WallMaterial.Controls.Add(this.tbx_SpaceV);
            this.gbx_WallMaterial.Controls.Add(this.tbx_StepH);
            this.gbx_WallMaterial.Controls.Add(this.lblStepH);
            this.gbx_WallMaterial.Controls.Add(this.tbx_StepV);
            this.gbx_WallMaterial.Controls.Add(this.lblStepV);
            this.gbx_WallMaterial.Controls.Add(this.lblSpaceUnitsH);
            this.gbx_WallMaterial.Controls.Add(this.lblSpaceUnitsV);
            this.gbx_WallMaterial.Controls.Add(this.lblStepUnitsH);
            this.gbx_WallMaterial.Controls.Add(this.lblStepUnitsV);
            this.gbx_WallMaterial.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.gbx_WallMaterial.ForeColor = System.Drawing.SystemColors.ControlText;
            this.gbx_WallMaterial.Location = new System.Drawing.Point(202, 64);
            this.gbx_WallMaterial.Name = "gbx_WallMaterial";
            this.gbx_WallMaterial.Padding = new System.Windows.Forms.Padding(0);
            this.gbx_WallMaterial.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.gbx_WallMaterial.Size = new System.Drawing.Size(372, 144);
            this.gbx_WallMaterial.TabIndex = 67;
            this.gbx_WallMaterial.TabStop = false;
            this.gbx_WallMaterial.Text = "WALL MATERIAL UNIT PARAMETERS";
            // 
            // tbx_SpaceH
            // 
            this.tbx_SpaceH.AcceptsReturn = true;
            this.tbx_SpaceH.BackColor = System.Drawing.SystemColors.Window;
            this.tbx_SpaceH.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.tbx_SpaceH.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.tbx_SpaceH.ForeColor = System.Drawing.SystemColors.WindowText;
            this.tbx_SpaceH.Location = new System.Drawing.Point(280, 15);
            this.tbx_SpaceH.MaxLength = 0;
            this.tbx_SpaceH.Name = "tbx_SpaceH";
            this.tbx_SpaceH.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.tbx_SpaceH.Size = new System.Drawing.Size(42, 21);
            this.tbx_SpaceH.TabIndex = 36;
            // 
            // lblSpacingH
            // 
            this.lblSpacingH.AutoSize = true;
            this.lblSpacingH.BackColor = System.Drawing.Color.Transparent;
            this.lblSpacingH.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblSpacingH.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.lblSpacingH.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblSpacingH.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblSpacingH.Location = new System.Drawing.Point(3, 22);
            this.lblSpacingH.Name = "lblSpacingH";
            this.lblSpacingH.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblSpacingH.Size = new System.Drawing.Size(205, 13);
            this.lblSpacingH.TabIndex = 1;
            this.lblSpacingH.Text = "SPACING - CENTER TO CENTER - HORIZ.";
            // 
            // lblSpacingV
            // 
            this.lblSpacingV.AutoSize = true;
            this.lblSpacingV.BackColor = System.Drawing.Color.Transparent;
            this.lblSpacingV.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblSpacingV.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.lblSpacingV.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblSpacingV.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblSpacingV.Location = new System.Drawing.Point(3, 52);
            this.lblSpacingV.Name = "lblSpacingV";
            this.lblSpacingV.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblSpacingV.Size = new System.Drawing.Size(198, 13);
            this.lblSpacingV.TabIndex = 2;
            this.lblSpacingV.Text = "SPACING - CENTER TO CENTER - VERT.";
            // 
            // tbx_SpaceV
            // 
            this.tbx_SpaceV.AcceptsReturn = true;
            this.tbx_SpaceV.BackColor = System.Drawing.SystemColors.Window;
            this.tbx_SpaceV.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.tbx_SpaceV.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.tbx_SpaceV.ForeColor = System.Drawing.SystemColors.WindowText;
            this.tbx_SpaceV.Location = new System.Drawing.Point(280, 47);
            this.tbx_SpaceV.MaxLength = 0;
            this.tbx_SpaceV.Name = "tbx_SpaceV";
            this.tbx_SpaceV.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.tbx_SpaceV.Size = new System.Drawing.Size(42, 21);
            this.tbx_SpaceV.TabIndex = 37;
            // 
            // tbx_StepH
            // 
            this.tbx_StepH.AcceptsReturn = true;
            this.tbx_StepH.BackColor = System.Drawing.SystemColors.Window;
            this.tbx_StepH.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.tbx_StepH.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.tbx_StepH.ForeColor = System.Drawing.SystemColors.WindowText;
            this.tbx_StepH.Location = new System.Drawing.Point(280, 79);
            this.tbx_StepH.MaxLength = 0;
            this.tbx_StepH.Name = "tbx_StepH";
            this.tbx_StepH.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.tbx_StepH.Size = new System.Drawing.Size(42, 21);
            this.tbx_StepH.TabIndex = 38;
            // 
            // lblStepH
            // 
            this.lblStepH.AutoSize = true;
            this.lblStepH.BackColor = System.Drawing.Color.Transparent;
            this.lblStepH.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblStepH.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.lblStepH.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblStepH.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblStepH.Location = new System.Drawing.Point(3, 82);
            this.lblStepH.Name = "lblStepH";
            this.lblStepH.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblStepH.Size = new System.Drawing.Size(234, 13);
            this.lblStepH.TabIndex = 5;
            this.lblStepH.Text = "MINIMUM STEP - CENTER TO CENTER - HORIZ.";
            // 
            // tbx_StepV
            // 
            this.tbx_StepV.AcceptsReturn = true;
            this.tbx_StepV.BackColor = System.Drawing.SystemColors.Window;
            this.tbx_StepV.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.tbx_StepV.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.tbx_StepV.ForeColor = System.Drawing.SystemColors.WindowText;
            this.tbx_StepV.Location = new System.Drawing.Point(280, 111);
            this.tbx_StepV.MaxLength = 0;
            this.tbx_StepV.Name = "tbx_StepV";
            this.tbx_StepV.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.tbx_StepV.Size = new System.Drawing.Size(42, 21);
            this.tbx_StepV.TabIndex = 38;
            // 
            // lblStepV
            // 
            this.lblStepV.AutoSize = true;
            this.lblStepV.BackColor = System.Drawing.Color.Transparent;
            this.lblStepV.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblStepV.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.lblStepV.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblStepV.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblStepV.Location = new System.Drawing.Point(1, 112);
            this.lblStepV.Name = "lblStepV";
            this.lblStepV.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblStepV.Size = new System.Drawing.Size(227, 13);
            this.lblStepV.TabIndex = 7;
            this.lblStepV.Text = "MINIMUM STEP - CENTER TO CENTER - VERT.";
            // 
            // lblSpaceUnitsH
            // 
            this.lblSpaceUnitsH.BackColor = System.Drawing.Color.Transparent;
            this.lblSpaceUnitsH.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblSpaceUnitsH.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblSpaceUnitsH.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblSpaceUnitsH.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblSpaceUnitsH.Location = new System.Drawing.Point(325, 19);
            this.lblSpaceUnitsH.Name = "lblSpaceUnitsH";
            this.lblSpaceUnitsH.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblSpaceUnitsH.Size = new System.Drawing.Size(38, 16);
            this.lblSpaceUnitsH.TabIndex = 8;
            this.lblSpaceUnitsH.Text = "(FT.)";
            // 
            // lblSpaceUnitsV
            // 
            this.lblSpaceUnitsV.BackColor = System.Drawing.Color.Transparent;
            this.lblSpaceUnitsV.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblSpaceUnitsV.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblSpaceUnitsV.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblSpaceUnitsV.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblSpaceUnitsV.Location = new System.Drawing.Point(325, 51);
            this.lblSpaceUnitsV.Name = "lblSpaceUnitsV";
            this.lblSpaceUnitsV.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblSpaceUnitsV.Size = new System.Drawing.Size(38, 16);
            this.lblSpaceUnitsV.TabIndex = 9;
            this.lblSpaceUnitsV.Text = "(FT.)";
            // 
            // lblStepUnitsH
            // 
            this.lblStepUnitsH.BackColor = System.Drawing.Color.Transparent;
            this.lblStepUnitsH.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblStepUnitsH.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblStepUnitsH.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblStepUnitsH.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblStepUnitsH.Location = new System.Drawing.Point(325, 83);
            this.lblStepUnitsH.Name = "lblStepUnitsH";
            this.lblStepUnitsH.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblStepUnitsH.Size = new System.Drawing.Size(38, 16);
            this.lblStepUnitsH.TabIndex = 10;
            this.lblStepUnitsH.Text = "(FT.)";
            // 
            // lblStepUnitsV
            // 
            this.lblStepUnitsV.BackColor = System.Drawing.Color.Transparent;
            this.lblStepUnitsV.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblStepUnitsV.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblStepUnitsV.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblStepUnitsV.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblStepUnitsV.Location = new System.Drawing.Point(325, 115);
            this.lblStepUnitsV.Name = "lblStepUnitsV";
            this.lblStepUnitsV.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblStepUnitsV.Size = new System.Drawing.Size(38, 16);
            this.lblStepUnitsV.TabIndex = 11;
            this.lblStepUnitsV.Text = "(FT.)";
            // 
            // gbx_ScreenWall
            // 
            this.gbx_ScreenWall.BackColor = System.Drawing.Color.Transparent;
            this.gbx_ScreenWall.Controls.Add(this.cbx_HeightAboveFooting);
            this.gbx_ScreenWall.Controls.Add(this.cbx_HeightAboveGround);
            this.gbx_ScreenWall.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbx_ScreenWall.Location = new System.Drawing.Point(348, 10);
            this.gbx_ScreenWall.Name = "gbx_ScreenWall";
            this.gbx_ScreenWall.Size = new System.Drawing.Size(226, 48);
            this.gbx_ScreenWall.TabIndex = 66;
            this.gbx_ScreenWall.TabStop = false;
            this.gbx_ScreenWall.Text = "SCREEN WALL";
            // 
            // cbx_HeightAboveFooting
            // 
            this.cbx_HeightAboveFooting.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbx_HeightAboveFooting.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.cbx_HeightAboveFooting.Location = new System.Drawing.Point(2, 15);
            this.cbx_HeightAboveFooting.Margin = new System.Windows.Forms.Padding(3, 0, 0, 3);
            this.cbx_HeightAboveFooting.Name = "cbx_HeightAboveFooting";
            this.cbx_HeightAboveFooting.Size = new System.Drawing.Size(110, 31);
            this.cbx_HeightAboveFooting.TabIndex = 0;
            this.cbx_HeightAboveFooting.Text = "HEIGHT ABOVE FOOTING";
            this.cbx_HeightAboveFooting.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.cbx_HeightAboveFooting.UseVisualStyleBackColor = true;
            this.cbx_HeightAboveFooting.Click += new System.EventHandler(this.cbx_HeightAboveFooting_Click);
            // 
            // cbx_HeightAboveGround
            // 
            this.cbx_HeightAboveGround.BackColor = System.Drawing.Color.Transparent;
            this.cbx_HeightAboveGround.Cursor = System.Windows.Forms.Cursors.Default;
            this.cbx_HeightAboveGround.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.cbx_HeightAboveGround.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cbx_HeightAboveGround.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cbx_HeightAboveGround.Location = new System.Drawing.Point(116, 15);
            this.cbx_HeightAboveGround.Margin = new System.Windows.Forms.Padding(3, 0, 0, 3);
            this.cbx_HeightAboveGround.Name = "cbx_HeightAboveGround";
            this.cbx_HeightAboveGround.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.cbx_HeightAboveGround.Size = new System.Drawing.Size(104, 31);
            this.cbx_HeightAboveGround.TabIndex = 35;
            this.cbx_HeightAboveGround.Text = "HEIGHT ABOVE GROUND";
            this.cbx_HeightAboveGround.UseVisualStyleBackColor = false;
            this.cbx_HeightAboveGround.Click += new System.EventHandler(this.cbx_HeightAboveGround_Click);
            // 
            // gbx_WallType
            // 
            this.gbx_WallType.BackColor = System.Drawing.Color.Transparent;
            this.gbx_WallType.Controls.Add(this.opt_PANEL);
            this.gbx_WallType.Controls.Add(this.opt_BLOCK);
            this.gbx_WallType.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.gbx_WallType.ForeColor = System.Drawing.SystemColors.ControlText;
            this.gbx_WallType.Location = new System.Drawing.Point(202, 10);
            this.gbx_WallType.Name = "gbx_WallType";
            this.gbx_WallType.Padding = new System.Windows.Forms.Padding(0);
            this.gbx_WallType.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.gbx_WallType.Size = new System.Drawing.Size(140, 48);
            this.gbx_WallType.TabIndex = 65;
            this.gbx_WallType.TabStop = false;
            this.gbx_WallType.Text = "WALL TYPE OPTIONS";
            // 
            // opt_PANEL
            // 
            this.opt_PANEL.AutoSize = true;
            this.opt_PANEL.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.opt_PANEL.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.opt_PANEL.Location = new System.Drawing.Point(90, 19);
            this.opt_PANEL.Name = "opt_PANEL";
            this.opt_PANEL.Size = new System.Drawing.Size(56, 17);
            this.opt_PANEL.TabIndex = 34;
            this.opt_PANEL.TabStop = true;
            this.opt_PANEL.Text = "PANEL";
            this.opt_PANEL.UseVisualStyleBackColor = true;
            this.opt_PANEL.Click += new System.EventHandler(this.opt_PANEL_Click);
            // 
            // opt_BLOCK
            // 
            this.opt_BLOCK.AutoSize = true;
            this.opt_BLOCK.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.opt_BLOCK.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.opt_BLOCK.Location = new System.Drawing.Point(13, 19);
            this.opt_BLOCK.Name = "opt_BLOCK";
            this.opt_BLOCK.Size = new System.Drawing.Size(57, 17);
            this.opt_BLOCK.TabIndex = 33;
            this.opt_BLOCK.TabStop = true;
            this.opt_BLOCK.Text = "BLOCK";
            this.opt_BLOCK.UseVisualStyleBackColor = true;
            this.opt_BLOCK.Click += new System.EventHandler(this.opt_BLOCK_Click);
            // 
            // btnCreateWallProfileView
            // 
            this.btnCreateWallProfileView.Location = new System.Drawing.Point(19, 174);
            this.btnCreateWallProfileView.Name = "btnCreateWallProfileView";
            this.btnCreateWallProfileView.Size = new System.Drawing.Size(172, 52);
            this.btnCreateWallProfileView.TabIndex = 64;
            this.btnCreateWallProfileView.Text = "CREATE WALL PROFILEVIEW";
            this.btnCreateWallProfileView.UseVisualStyleBackColor = true;
            this.btnCreateWallProfileView.Click += new System.EventHandler(this.btnCreateWallProfileView_Click);
            // 
            // btnSelectBrkline2
            // 
            this.btnSelectBrkline2.Location = new System.Drawing.Point(19, 113);
            this.btnSelectBrkline2.Name = "btnSelectBrkline2";
            this.btnSelectBrkline2.Size = new System.Drawing.Size(172, 52);
            this.btnSelectBrkline2.TabIndex = 63;
            this.btnSelectBrkline2.Text = "SELECT \r\nDESIGN BREAKLINE - RIGHT";
            this.btnSelectBrkline2.UseVisualStyleBackColor = true;
            this.btnSelectBrkline2.Click += new System.EventHandler(this.btnSelectBrkline2_Click);
            // 
            // btnSelectBrkline1
            // 
            this.btnSelectBrkline1.Location = new System.Drawing.Point(18, 61);
            this.btnSelectBrkline1.Name = "btnSelectBrkline1";
            this.btnSelectBrkline1.Size = new System.Drawing.Size(173, 46);
            this.btnSelectBrkline1.TabIndex = 62;
            this.btnSelectBrkline1.Text = "SELECT \r\nDESIGN BREAKLINE - LEFT";
            this.btnSelectBrkline1.UseVisualStyleBackColor = true;
            this.btnSelectBrkline1.Click += new System.EventHandler(this.btnSelectBrkline1_Click);
            // 
            // btnSelectWallAlign
            // 
            this.btnSelectWallAlign.Location = new System.Drawing.Point(18, 7);
            this.btnSelectWallAlign.Name = "btnSelectWallAlign";
            this.btnSelectWallAlign.Size = new System.Drawing.Size(173, 48);
            this.btnSelectWallAlign.TabIndex = 61;
            this.btnSelectWallAlign.Text = "SELECT \r\nWALL ALIGNMENT";
            this.btnSelectWallAlign.UseVisualStyleBackColor = true;
            this.btnSelectWallAlign.Click += new System.EventHandler(this.btnSelectWallAlign_Click);
            // 
            // ToolStripStatusLabel1
            // 
            this.ToolStripStatusLabel1.Name = "ToolStripStatusLabel1";
            this.ToolStripStatusLabel1.Size = new System.Drawing.Size(86, 17);
            this.ToolStripStatusLabel1.Text = "ACTIVE ALIGN:";
            // 
            // StatusStrip1
            // 
            this.StatusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripStatusLabel1});
            this.StatusStrip1.Location = new System.Drawing.Point(0, 406);
            this.StatusStrip1.Name = "StatusStrip1";
            this.StatusStrip1.Size = new System.Drawing.Size(584, 22);
            this.StatusStrip1.TabIndex = 60;
            this.StatusStrip1.Text = "StatusStrip1";
            // 
            // frmWall2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 428);
            this.Controls.Add(this.btnUpdateLeaders);
            this.Controls.Add(this.btn_Update);
            this.Controls.Add(this.btn_BuildWallProfiles);
            this.Controls.Add(this.gbx_MinCover_Freeboard);
            this.Controls.Add(this.gbx_WallMaterial);
            this.Controls.Add(this.gbx_ScreenWall);
            this.Controls.Add(this.gbx_WallType);
            this.Controls.Add(this.btnCreateWallProfileView);
            this.Controls.Add(this.btnSelectBrkline2);
            this.Controls.Add(this.btnSelectBrkline1);
            this.Controls.Add(this.btnSelectWallAlign);
            this.Controls.Add(this.StatusStrip1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmWall2";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "frmWall2";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmWall2_FormClosing);
            this.gbx_MinCover_Freeboard.ResumeLayout(false);
            this.gbx_MinCover_Freeboard.PerformLayout();
            this.gbx_WallMaterial.ResumeLayout(false);
            this.gbx_WallMaterial.PerformLayout();
            this.gbx_ScreenWall.ResumeLayout(false);
            this.gbx_WallType.ResumeLayout(false);
            this.gbx_WallType.PerformLayout();
            this.StatusStrip1.ResumeLayout(false);
            this.StatusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.GroupBox gbx_MinCover_Freeboard;
        public System.Windows.Forms.TextBox tbx_Cover;
        public System.Windows.Forms.Label Label12;
        public System.Windows.Forms.Label lblFreeBoardHeight;
        public System.Windows.Forms.TextBox tbx_Freeboard;
        public System.Windows.Forms.Label lblCoverUnits;
        public System.Windows.Forms.Label lblFreeboardUnits;
        public System.Windows.Forms.Label lblFreeBoardTolerance;
        public System.Windows.Forms.TextBox tbx_FreeboardTolerance;
        public System.Windows.Forms.Label Label13;
        public System.Windows.Forms.GroupBox gbx_WallMaterial;
        public System.Windows.Forms.TextBox tbx_SpaceH;
        public System.Windows.Forms.Label lblSpacingH;
        public System.Windows.Forms.Label lblSpacingV;
        public System.Windows.Forms.TextBox tbx_SpaceV;
        public System.Windows.Forms.TextBox tbx_StepH;
        public System.Windows.Forms.Label lblStepH;
        public System.Windows.Forms.TextBox tbx_StepV;
        public System.Windows.Forms.Label lblStepV;
        public System.Windows.Forms.Label lblSpaceUnitsH;
        public System.Windows.Forms.Label lblSpaceUnitsV;
        public System.Windows.Forms.Label lblStepUnitsH;
        public System.Windows.Forms.Label lblStepUnitsV;
        internal System.Windows.Forms.GroupBox gbx_ScreenWall;
        public System.Windows.Forms.CheckBox cbx_HeightAboveFooting;
        public System.Windows.Forms.CheckBox cbx_HeightAboveGround;
        public System.Windows.Forms.GroupBox gbx_WallType;
        public System.Windows.Forms.RadioButton opt_PANEL;
        public System.Windows.Forms.RadioButton opt_BLOCK;
        internal System.Windows.Forms.ToolStripStatusLabel ToolStripStatusLabel1;
        internal System.Windows.Forms.StatusStrip StatusStrip1;
        public System.Windows.Forms.Button btnUpdateLeaders;
        public System.Windows.Forms.Button btn_Update;
        public System.Windows.Forms.Button btn_BuildWallProfiles;
        public System.Windows.Forms.Button btnCreateWallProfileView;
        public System.Windows.Forms.Button btnSelectBrkline2;
        public System.Windows.Forms.Button btnSelectBrkline1;
        public System.Windows.Forms.Button btnSelectWallAlign;
    }
}