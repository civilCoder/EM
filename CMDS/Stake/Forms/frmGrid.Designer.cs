namespace Stake.Forms
{
    partial class frmGrid
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
            this.Frame01 = new System.Windows.Forms.GroupBox();
            this.cmdSelectGrid = new System.Windows.Forms.Button();
            this.Frame02 = new System.Windows.Forms.GroupBox();
            this.Frame03 = new System.Windows.Forms.GroupBox();
            this.cmdIdGridSets = new System.Windows.Forms.Button();
            this.cmdEditBuildNames = new System.Windows.Forms.Button();
            this.Frame04 = new System.Windows.Forms.GroupBox();
            this.cmdGetNumeric = new System.Windows.Forms.Button();
            this.cmdGetAlpha = new System.Windows.Forms.Button();
            this.cmdIdSecondary = new System.Windows.Forms.Button();
            this.Frame05 = new System.Windows.Forms.GroupBox();
            this.cboxOmitO = new System.Windows.Forms.CheckBox();
            this.cbxAlpha = new System.Windows.Forms.ComboBox();
            this.cbxNumeric = new System.Windows.Forms.ComboBox();
            this.lblNumeric = new System.Windows.Forms.Label();
            this.lblAlpha = new System.Windows.Forms.Label();
            this.cmdGridLabel = new System.Windows.Forms.Button();
            this.cboxUseI = new System.Windows.Forms.CheckBox();
            this.Frame07 = new System.Windows.Forms.GroupBox();
            this.cmdDeleteAll = new System.Windows.Forms.Button();
            this.cmdAlignCreate = new System.Windows.Forms.Button();
            this.cmdAlignDelete = new System.Windows.Forms.Button();
            this.Frame08 = new System.Windows.Forms.GroupBox();
            this.Frame06 = new System.Windows.Forms.GroupBox();
            this.cmdGridEdit = new System.Windows.Forms.Button();
            this.cmdGridAdd = new System.Windows.Forms.Button();
            this.cmdGridDelete = new System.Windows.Forms.Button();
            this.Frame09 = new System.Windows.Forms.GroupBox();
            this.Frame7B = new System.Windows.Forms.GroupBox();
            this.optIN = new System.Windows.Forms.RadioButton();
            this.optOUT = new System.Windows.Forms.RadioButton();
            this.optBOTH = new System.Windows.Forms.RadioButton();
            this.Frame7C = new System.Windows.Forms.GroupBox();
            this.optALL = new System.Windows.Forms.RadioButton();
            this.optALT = new System.Windows.Forms.RadioButton();
            this.Frame7A = new System.Windows.Forms.GroupBox();
            this.optRBC = new System.Windows.Forms.RadioButton();
            this.optPBC = new System.Windows.Forms.RadioButton();
            this.Label1 = new System.Windows.Forms.Label();
            this.tbxOffsetV = new System.Windows.Forms.TextBox();
            this.Label2 = new System.Windows.Forms.Label();
            this.tbxOffsetH = new System.Windows.Forms.TextBox();
            this.Frame11 = new System.Windows.Forms.GroupBox();
            this.Frame10 = new System.Windows.Forms.GroupBox();
            this.cmdDone = new System.Windows.Forms.Button();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.Frame01.SuspendLayout();
            this.Frame03.SuspendLayout();
            this.Frame04.SuspendLayout();
            this.Frame05.SuspendLayout();
            this.Frame07.SuspendLayout();
            this.Frame06.SuspendLayout();
            this.Frame09.SuspendLayout();
            this.Frame7B.SuspendLayout();
            this.Frame7C.SuspendLayout();
            this.Frame7A.SuspendLayout();
            this.SuspendLayout();
            // 
            // Frame01
            // 
            this.Frame01.BackColor = System.Drawing.SystemColors.Control;
            this.Frame01.Controls.Add(this.cmdSelectGrid);
            this.Frame01.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Frame01.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Frame01.Location = new System.Drawing.Point(12, 12);
            this.Frame01.Name = "Frame01";
            this.Frame01.Padding = new System.Windows.Forms.Padding(0);
            this.Frame01.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Frame01.Size = new System.Drawing.Size(410, 56);
            this.Frame01.TabIndex = 10;
            this.Frame01.TabStop = false;
            this.Frame01.Text = "1.  SELECT BUILDING GRID";
            // 
            // cmdSelectGrid
            // 
            this.cmdSelectGrid.BackColor = System.Drawing.SystemColors.Control;
            this.cmdSelectGrid.Cursor = System.Windows.Forms.Cursors.Default;
            this.cmdSelectGrid.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdSelectGrid.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cmdSelectGrid.Location = new System.Drawing.Point(64, 16);
            this.cmdSelectGrid.Name = "cmdSelectGrid";
            this.cmdSelectGrid.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.cmdSelectGrid.Size = new System.Drawing.Size(298, 32);
            this.cmdSelectGrid.TabIndex = 0;
            this.cmdSelectGrid.Text = "SELECT ANY BUILDING GRID IN \"CNTL\" FILE";
            this.cmdSelectGrid.UseVisualStyleBackColor = false;
            // 
            // Frame02
            // 
            this.Frame02.BackColor = System.Drawing.SystemColors.Control;
            this.Frame02.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Frame02.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Frame02.Location = new System.Drawing.Point(12, 74);
            this.Frame02.Name = "Frame02";
            this.Frame02.Padding = new System.Windows.Forms.Padding(0);
            this.Frame02.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Frame02.Size = new System.Drawing.Size(410, 24);
            this.Frame02.TabIndex = 11;
            this.Frame02.TabStop = false;
            this.Frame02.Text = "2.  ADD GRID LINES AT BUILDING LIMITS (by USER)";
            // 
            // Frame03
            // 
            this.Frame03.BackColor = System.Drawing.SystemColors.Control;
            this.Frame03.Controls.Add(this.cmdIdGridSets);
            this.Frame03.Controls.Add(this.cmdEditBuildNames);
            this.Frame03.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Frame03.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Frame03.Location = new System.Drawing.Point(12, 104);
            this.Frame03.Name = "Frame03";
            this.Frame03.Padding = new System.Windows.Forms.Padding(0);
            this.Frame03.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Frame03.Size = new System.Drawing.Size(410, 56);
            this.Frame03.TabIndex = 12;
            this.Frame03.TabStop = false;
            this.Frame03.Text = "3.  ID BUILDING GRID(S)";
            // 
            // cmdIdGridSets
            // 
            this.cmdIdGridSets.BackColor = System.Drawing.SystemColors.Control;
            this.cmdIdGridSets.Cursor = System.Windows.Forms.Cursors.Default;
            this.cmdIdGridSets.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdIdGridSets.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cmdIdGridSets.Location = new System.Drawing.Point(10, 16);
            this.cmdIdGridSets.Name = "cmdIdGridSets";
            this.cmdIdGridSets.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.cmdIdGridSets.Size = new System.Drawing.Size(120, 32);
            this.cmdIdGridSets.TabIndex = 0;
            this.cmdIdGridSets.Text = "ID BLDG GRID SET(S)";
            this.cmdIdGridSets.UseVisualStyleBackColor = false;
            // 
            // cmdEditBuildNames
            // 
            this.cmdEditBuildNames.BackColor = System.Drawing.SystemColors.Control;
            this.cmdEditBuildNames.Cursor = System.Windows.Forms.Cursors.Default;
            this.cmdEditBuildNames.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdEditBuildNames.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cmdEditBuildNames.Location = new System.Drawing.Point(146, 16);
            this.cmdEditBuildNames.Name = "cmdEditBuildNames";
            this.cmdEditBuildNames.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.cmdEditBuildNames.Size = new System.Drawing.Size(120, 32);
            this.cmdEditBuildNames.TabIndex = 1;
            this.cmdEditBuildNames.Text = "EDIT BLDG NAME(S)";
            this.cmdEditBuildNames.UseVisualStyleBackColor = false;
            // 
            // Frame04
            // 
            this.Frame04.BackColor = System.Drawing.SystemColors.Control;
            this.Frame04.Controls.Add(this.cmdGetNumeric);
            this.Frame04.Controls.Add(this.cmdGetAlpha);
            this.Frame04.Controls.Add(this.cmdIdSecondary);
            this.Frame04.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Frame04.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Frame04.Location = new System.Drawing.Point(12, 166);
            this.Frame04.Name = "Frame04";
            this.Frame04.Padding = new System.Windows.Forms.Padding(0);
            this.Frame04.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Frame04.Size = new System.Drawing.Size(410, 64);
            this.Frame04.TabIndex = 13;
            this.Frame04.TabStop = false;
            this.Frame04.Text = "4.   ID GRID LINES";
            // 
            // cmdGetNumeric
            // 
            this.cmdGetNumeric.BackColor = System.Drawing.SystemColors.Control;
            this.cmdGetNumeric.Cursor = System.Windows.Forms.Cursors.Default;
            this.cmdGetNumeric.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdGetNumeric.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cmdGetNumeric.Location = new System.Drawing.Point(130, 24);
            this.cmdGetNumeric.Name = "cmdGetNumeric";
            this.cmdGetNumeric.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.cmdGetNumeric.Size = new System.Drawing.Size(112, 32);
            this.cmdGetNumeric.TabIndex = 0;
            this.cmdGetNumeric.Text = "ID GRID LINE 1";
            this.cmdGetNumeric.UseVisualStyleBackColor = false;
            // 
            // cmdGetAlpha
            // 
            this.cmdGetAlpha.BackColor = System.Drawing.SystemColors.Control;
            this.cmdGetAlpha.Cursor = System.Windows.Forms.Cursors.Default;
            this.cmdGetAlpha.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdGetAlpha.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cmdGetAlpha.Location = new System.Drawing.Point(10, 24);
            this.cmdGetAlpha.Name = "cmdGetAlpha";
            this.cmdGetAlpha.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.cmdGetAlpha.Size = new System.Drawing.Size(112, 32);
            this.cmdGetAlpha.TabIndex = 1;
            this.cmdGetAlpha.Text = "ID GRID LINE A";
            this.cmdGetAlpha.UseVisualStyleBackColor = false;
            // 
            // cmdIdSecondary
            // 
            this.cmdIdSecondary.BackColor = System.Drawing.SystemColors.Control;
            this.cmdIdSecondary.Cursor = System.Windows.Forms.Cursors.Default;
            this.cmdIdSecondary.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdIdSecondary.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cmdIdSecondary.Location = new System.Drawing.Point(250, 24);
            this.cmdIdSecondary.Name = "cmdIdSecondary";
            this.cmdIdSecondary.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.cmdIdSecondary.Size = new System.Drawing.Size(152, 32);
            this.cmdIdSecondary.TabIndex = 2;
            this.cmdIdSecondary.Text = "ID SECONDARY GRIDS";
            this.cmdIdSecondary.UseVisualStyleBackColor = false;
            // 
            // Frame05
            // 
            this.Frame05.BackColor = System.Drawing.SystemColors.Control;
            this.Frame05.Controls.Add(this.cboxOmitO);
            this.Frame05.Controls.Add(this.cbxAlpha);
            this.Frame05.Controls.Add(this.cbxNumeric);
            this.Frame05.Controls.Add(this.lblNumeric);
            this.Frame05.Controls.Add(this.lblAlpha);
            this.Frame05.Controls.Add(this.cmdGridLabel);
            this.Frame05.Controls.Add(this.cboxUseI);
            this.Frame05.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Frame05.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Frame05.Location = new System.Drawing.Point(12, 236);
            this.Frame05.Name = "Frame05";
            this.Frame05.Padding = new System.Windows.Forms.Padding(0);
            this.Frame05.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Frame05.Size = new System.Drawing.Size(410, 120);
            this.Frame05.TabIndex = 14;
            this.Frame05.TabStop = false;
            this.Frame05.Text = "5.  LABEL GRID LINES";
            // 
            // cboxOmitO
            // 
            this.cboxOmitO.BackColor = System.Drawing.SystemColors.Control;
            this.cboxOmitO.Cursor = System.Windows.Forms.Cursors.Default;
            this.cboxOmitO.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboxOmitO.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cboxOmitO.Location = new System.Drawing.Point(311, 48);
            this.cboxOmitO.Name = "cboxOmitO";
            this.cboxOmitO.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.cboxOmitO.Size = new System.Drawing.Size(91, 24);
            this.cboxOmitO.TabIndex = 6;
            this.cboxOmitO.Text = "OMIT \'O\'";
            this.cboxOmitO.UseVisualStyleBackColor = false;
            // 
            // cbxAlpha
            // 
            this.cbxAlpha.BackColor = System.Drawing.SystemColors.Window;
            this.cbxAlpha.Cursor = System.Windows.Forms.Cursors.Default;
            this.cbxAlpha.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbxAlpha.ForeColor = System.Drawing.SystemColors.WindowText;
            this.cbxAlpha.Location = new System.Drawing.Point(178, 48);
            this.cbxAlpha.Name = "cbxAlpha";
            this.cbxAlpha.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.cbxAlpha.Size = new System.Drawing.Size(64, 22);
            this.cbxAlpha.TabIndex = 0;
            // 
            // cbxNumeric
            // 
            this.cbxNumeric.BackColor = System.Drawing.SystemColors.Window;
            this.cbxNumeric.Cursor = System.Windows.Forms.Cursors.Default;
            this.cbxNumeric.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbxNumeric.ForeColor = System.Drawing.SystemColors.WindowText;
            this.cbxNumeric.Location = new System.Drawing.Point(178, 84);
            this.cbxNumeric.Name = "cbxNumeric";
            this.cbxNumeric.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.cbxNumeric.Size = new System.Drawing.Size(64, 22);
            this.cbxNumeric.TabIndex = 1;
            // 
            // lblNumeric
            // 
            this.lblNumeric.BackColor = System.Drawing.SystemColors.Control;
            this.lblNumeric.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblNumeric.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNumeric.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblNumeric.Location = new System.Drawing.Point(10, 52);
            this.lblNumeric.Name = "lblNumeric";
            this.lblNumeric.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblNumeric.Size = new System.Drawing.Size(160, 20);
            this.lblNumeric.TabIndex = 2;
            this.lblNumeric.Text = "ENTER LETTER RANGE:     A  to ";
            this.lblNumeric.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblAlpha
            // 
            this.lblAlpha.BackColor = System.Drawing.SystemColors.Control;
            this.lblAlpha.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblAlpha.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAlpha.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblAlpha.Location = new System.Drawing.Point(10, 84);
            this.lblAlpha.Name = "lblAlpha";
            this.lblAlpha.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblAlpha.Size = new System.Drawing.Size(160, 20);
            this.lblAlpha.TabIndex = 3;
            this.lblAlpha.Text = "ENTER NUMBER RANGE:   1  to";
            this.lblAlpha.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cmdGridLabel
            // 
            this.cmdGridLabel.BackColor = System.Drawing.SystemColors.Control;
            this.cmdGridLabel.Cursor = System.Windows.Forms.Cursors.Default;
            this.cmdGridLabel.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdGridLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cmdGridLabel.Location = new System.Drawing.Point(250, 80);
            this.cmdGridLabel.Name = "cmdGridLabel";
            this.cmdGridLabel.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.cmdGridLabel.Size = new System.Drawing.Size(152, 32);
            this.cmdGridLabel.TabIndex = 4;
            this.cmdGridLabel.Text = "LABEL GRID LINES";
            this.cmdGridLabel.UseVisualStyleBackColor = false;
            // 
            // cboxUseI
            // 
            this.cboxUseI.BackColor = System.Drawing.SystemColors.Control;
            this.cboxUseI.Cursor = System.Windows.Forms.Cursors.Default;
            this.cboxUseI.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboxUseI.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cboxUseI.Location = new System.Drawing.Point(250, 48);
            this.cboxUseI.Name = "cboxUseI";
            this.cboxUseI.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.cboxUseI.Size = new System.Drawing.Size(66, 24);
            this.cboxUseI.TabIndex = 5;
            this.cboxUseI.Text = "USE \'I\'";
            this.cboxUseI.UseVisualStyleBackColor = false;
            // 
            // Frame07
            // 
            this.Frame07.BackColor = System.Drawing.SystemColors.Control;
            this.Frame07.Controls.Add(this.cmdDeleteAll);
            this.Frame07.Controls.Add(this.cmdAlignCreate);
            this.Frame07.Controls.Add(this.cmdAlignDelete);
            this.Frame07.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Frame07.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Frame07.Location = new System.Drawing.Point(12, 426);
            this.Frame07.Name = "Frame07";
            this.Frame07.Padding = new System.Windows.Forms.Padding(0);
            this.Frame07.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Frame07.Size = new System.Drawing.Size(410, 56);
            this.Frame07.TabIndex = 15;
            this.Frame07.TabStop = false;
            this.Frame07.Text = "7.  CREATE / DELETE ALIGNMENT";
            // 
            // cmdDeleteAll
            // 
            this.cmdDeleteAll.BackColor = System.Drawing.SystemColors.Control;
            this.cmdDeleteAll.Cursor = System.Windows.Forms.Cursors.Default;
            this.cmdDeleteAll.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdDeleteAll.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cmdDeleteAll.Location = new System.Drawing.Point(3, 16);
            this.cmdDeleteAll.Name = "cmdDeleteAll";
            this.cmdDeleteAll.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.cmdDeleteAll.Size = new System.Drawing.Size(145, 32);
            this.cmdDeleteAll.TabIndex = 2;
            this.cmdDeleteAll.Text = "DELETE ALL GRID CRAP";
            this.cmdDeleteAll.UseVisualStyleBackColor = false;
            this.cmdDeleteAll.Click += new System.EventHandler(this.cmdDeleteAll_Click_1);
            // 
            // cmdAlignCreate
            // 
            this.cmdAlignCreate.BackColor = System.Drawing.SystemColors.Control;
            this.cmdAlignCreate.Cursor = System.Windows.Forms.Cursors.Default;
            this.cmdAlignCreate.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdAlignCreate.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cmdAlignCreate.Location = new System.Drawing.Point(281, 16);
            this.cmdAlignCreate.Name = "cmdAlignCreate";
            this.cmdAlignCreate.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.cmdAlignCreate.Size = new System.Drawing.Size(121, 32);
            this.cmdAlignCreate.TabIndex = 0;
            this.cmdAlignCreate.Text = "CREATE ALIGNMENT";
            this.cmdAlignCreate.UseVisualStyleBackColor = false;
            // 
            // cmdAlignDelete
            // 
            this.cmdAlignDelete.BackColor = System.Drawing.SystemColors.Control;
            this.cmdAlignDelete.Cursor = System.Windows.Forms.Cursors.Default;
            this.cmdAlignDelete.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdAlignDelete.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cmdAlignDelete.Location = new System.Drawing.Point(154, 16);
            this.cmdAlignDelete.Name = "cmdAlignDelete";
            this.cmdAlignDelete.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.cmdAlignDelete.Size = new System.Drawing.Size(120, 32);
            this.cmdAlignDelete.TabIndex = 1;
            this.cmdAlignDelete.Text = "DELETE GRID ALIGN";
            this.cmdAlignDelete.UseVisualStyleBackColor = false;
            // 
            // Frame08
            // 
            this.Frame08.BackColor = System.Drawing.SystemColors.Control;
            this.Frame08.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Frame08.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Frame08.Location = new System.Drawing.Point(12, 490);
            this.Frame08.Name = "Frame08";
            this.Frame08.Padding = new System.Windows.Forms.Padding(0);
            this.Frame08.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Frame08.Size = new System.Drawing.Size(410, 80);
            this.Frame08.TabIndex = 19;
            this.Frame08.TabStop = false;
            this.Frame08.Text = "8.  CHECK FINISH FLOOR GRADES";
            // 
            // Frame06
            // 
            this.Frame06.BackColor = System.Drawing.SystemColors.Control;
            this.Frame06.Controls.Add(this.cmdGridEdit);
            this.Frame06.Controls.Add(this.cmdGridAdd);
            this.Frame06.Controls.Add(this.cmdGridDelete);
            this.Frame06.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Frame06.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Frame06.Location = new System.Drawing.Point(12, 362);
            this.Frame06.Name = "Frame06";
            this.Frame06.Padding = new System.Windows.Forms.Padding(0);
            this.Frame06.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Frame06.Size = new System.Drawing.Size(410, 56);
            this.Frame06.TabIndex = 16;
            this.Frame06.TabStop = false;
            this.Frame06.Text = "6.  ADD/DELETE GRID LINE or EDIT GRID IDs";
            // 
            // cmdGridEdit
            // 
            this.cmdGridEdit.BackColor = System.Drawing.SystemColors.Control;
            this.cmdGridEdit.Cursor = System.Windows.Forms.Cursors.Default;
            this.cmdGridEdit.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdGridEdit.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cmdGridEdit.Location = new System.Drawing.Point(290, 16);
            this.cmdGridEdit.Name = "cmdGridEdit";
            this.cmdGridEdit.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.cmdGridEdit.Size = new System.Drawing.Size(112, 32);
            this.cmdGridEdit.TabIndex = 0;
            this.cmdGridEdit.Text = "EDIT GRID ID";
            this.cmdGridEdit.UseVisualStyleBackColor = false;
            // 
            // cmdGridAdd
            // 
            this.cmdGridAdd.BackColor = System.Drawing.SystemColors.Control;
            this.cmdGridAdd.Cursor = System.Windows.Forms.Cursors.Default;
            this.cmdGridAdd.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdGridAdd.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cmdGridAdd.Location = new System.Drawing.Point(10, 16);
            this.cmdGridAdd.Name = "cmdGridAdd";
            this.cmdGridAdd.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.cmdGridAdd.Size = new System.Drawing.Size(112, 32);
            this.cmdGridAdd.TabIndex = 1;
            this.cmdGridAdd.Text = "ADD GRID LINE";
            this.cmdGridAdd.UseVisualStyleBackColor = false;
            // 
            // cmdGridDelete
            // 
            this.cmdGridDelete.BackColor = System.Drawing.SystemColors.Control;
            this.cmdGridDelete.Cursor = System.Windows.Forms.Cursors.Default;
            this.cmdGridDelete.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdGridDelete.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cmdGridDelete.Location = new System.Drawing.Point(130, 16);
            this.cmdGridDelete.Name = "cmdGridDelete";
            this.cmdGridDelete.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.cmdGridDelete.Size = new System.Drawing.Size(112, 32);
            this.cmdGridDelete.TabIndex = 2;
            this.cmdGridDelete.Text = "DELETE GRID LINE";
            this.cmdGridDelete.UseVisualStyleBackColor = false;
            // 
            // Frame09
            // 
            this.Frame09.BackColor = System.Drawing.SystemColors.Control;
            this.Frame09.Controls.Add(this.Frame7B);
            this.Frame09.Controls.Add(this.Frame7C);
            this.Frame09.Controls.Add(this.Frame7A);
            this.Frame09.Controls.Add(this.Label1);
            this.Frame09.Controls.Add(this.tbxOffsetV);
            this.Frame09.Controls.Add(this.Label2);
            this.Frame09.Controls.Add(this.tbxOffsetH);
            this.Frame09.Controls.Add(this.Frame11);
            this.Frame09.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Frame09.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Frame09.Location = new System.Drawing.Point(12, 578);
            this.Frame09.Name = "Frame09";
            this.Frame09.Padding = new System.Windows.Forms.Padding(0);
            this.Frame09.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Frame09.Size = new System.Drawing.Size(410, 144);
            this.Frame09.TabIndex = 17;
            this.Frame09.TabStop = false;
            this.Frame09.Text = "9.  STAKE SETUP";
            // 
            // Frame7B
            // 
            this.Frame7B.BackColor = System.Drawing.SystemColors.Control;
            this.Frame7B.Controls.Add(this.optIN);
            this.Frame7B.Controls.Add(this.optOUT);
            this.Frame7B.Controls.Add(this.optBOTH);
            this.Frame7B.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Frame7B.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Frame7B.Location = new System.Drawing.Point(10, 88);
            this.Frame7B.Name = "Frame7B";
            this.Frame7B.Padding = new System.Windows.Forms.Padding(0);
            this.Frame7B.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Frame7B.Size = new System.Drawing.Size(240, 48);
            this.Frame7B.TabIndex = 1;
            this.Frame7B.TabStop = false;
            this.Frame7B.Text = "LOCATION";
            // 
            // optIN
            // 
            this.optIN.AutoSize = true;
            this.optIN.Location = new System.Drawing.Point(93, 19);
            this.optIN.Name = "optIN";
            this.optIN.Size = new System.Drawing.Size(70, 18);
            this.optIN.TabIndex = 12;
            this.optIN.TabStop = true;
            this.optIN.Text = "INTERIOR";
            this.optIN.UseVisualStyleBackColor = true;
            // 
            // optOUT
            // 
            this.optOUT.AutoSize = true;
            this.optOUT.Location = new System.Drawing.Point(13, 19);
            this.optOUT.Name = "optOUT";
            this.optOUT.Size = new System.Drawing.Size(79, 18);
            this.optOUT.TabIndex = 13;
            this.optOUT.TabStop = true;
            this.optOUT.Text = "PERIMETER";
            this.optOUT.UseVisualStyleBackColor = true;
            // 
            // optBOTH
            // 
            this.optBOTH.AutoSize = true;
            this.optBOTH.Location = new System.Drawing.Point(171, 18);
            this.optBOTH.Name = "optBOTH";
            this.optBOTH.Size = new System.Drawing.Size(53, 18);
            this.optBOTH.TabIndex = 11;
            this.optBOTH.TabStop = true;
            this.optBOTH.Text = "BOTH";
            this.optBOTH.UseVisualStyleBackColor = true;
            // 
            // Frame7C
            // 
            this.Frame7C.BackColor = System.Drawing.SystemColors.Control;
            this.Frame7C.Controls.Add(this.optALL);
            this.Frame7C.Controls.Add(this.optALT);
            this.Frame7C.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Frame7C.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Frame7C.Location = new System.Drawing.Point(258, 88);
            this.Frame7C.Name = "Frame7C";
            this.Frame7C.Padding = new System.Windows.Forms.Padding(0);
            this.Frame7C.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Frame7C.Size = new System.Drawing.Size(144, 48);
            this.Frame7C.TabIndex = 2;
            this.Frame7C.TabStop = false;
            this.Frame7C.Text = "FREQUENCY";
            // 
            // optALL
            // 
            this.optALL.AutoSize = true;
            this.optALL.Location = new System.Drawing.Point(18, 18);
            this.optALL.Name = "optALL";
            this.optALL.Size = new System.Drawing.Size(45, 18);
            this.optALL.TabIndex = 2;
            this.optALL.TabStop = true;
            this.optALL.Text = "ALL";
            this.optALL.UseVisualStyleBackColor = true;
            // 
            // optALT
            // 
            this.optALT.AutoSize = true;
            this.optALT.Location = new System.Drawing.Point(69, 18);
            this.optALT.Name = "optALT";
            this.optALT.Size = new System.Drawing.Size(71, 18);
            this.optALT.TabIndex = 1;
            this.optALT.TabStop = true;
            this.optALT.Text = "SKIP ONE";
            this.optALT.UseVisualStyleBackColor = true;
            // 
            // Frame7A
            // 
            this.Frame7A.BackColor = System.Drawing.SystemColors.Control;
            this.Frame7A.Controls.Add(this.optRBC);
            this.Frame7A.Controls.Add(this.optPBC);
            this.Frame7A.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Frame7A.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Frame7A.Location = new System.Drawing.Point(13, 32);
            this.Frame7A.Name = "Frame7A";
            this.Frame7A.Padding = new System.Windows.Forms.Padding(0);
            this.Frame7A.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Frame7A.Size = new System.Drawing.Size(160, 40);
            this.Frame7A.TabIndex = 0;
            this.Frame7A.TabStop = false;
            this.Frame7A.Text = "TYPE";
            // 
            // optRBC
            // 
            this.optRBC.AutoSize = true;
            this.optRBC.Location = new System.Drawing.Point(104, 16);
            this.optRBC.Name = "optRBC";
            this.optRBC.Size = new System.Drawing.Size(45, 18);
            this.optRBC.TabIndex = 3;
            this.optRBC.TabStop = true;
            this.optRBC.Text = "PAD";
            this.optRBC.UseVisualStyleBackColor = true;
            // 
            // optPBC
            // 
            this.optPBC.AutoSize = true;
            this.optPBC.BackColor = System.Drawing.SystemColors.Control;
            this.optPBC.Location = new System.Drawing.Point(10, 16);
            this.optPBC.Name = "optPBC";
            this.optPBC.Size = new System.Drawing.Size(66, 18);
            this.optPBC.TabIndex = 2;
            this.optPBC.TabStop = true;
            this.optPBC.Text = "PRECISE";
            this.optPBC.UseVisualStyleBackColor = false;
            // 
            // Label1
            // 
            this.Label1.BackColor = System.Drawing.SystemColors.Control;
            this.Label1.Cursor = System.Windows.Forms.Cursors.Default;
            this.Label1.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Label1.Location = new System.Drawing.Point(274, 8);
            this.Label1.Name = "Label1";
            this.Label1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Label1.Size = new System.Drawing.Size(104, 32);
            this.Label1.TabIndex = 3;
            this.Label1.Text = "VERTICAL OFFSET (FT.)   +/-";
            this.Label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // tbxOffsetV
            // 
            this.tbxOffsetV.AcceptsReturn = true;
            this.tbxOffsetV.BackColor = System.Drawing.SystemColors.Window;
            this.tbxOffsetV.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.tbxOffsetV.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbxOffsetV.ForeColor = System.Drawing.SystemColors.WindowText;
            this.tbxOffsetV.Location = new System.Drawing.Point(282, 45);
            this.tbxOffsetV.MaxLength = 0;
            this.tbxOffsetV.Name = "tbxOffsetV";
            this.tbxOffsetV.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.tbxOffsetV.Size = new System.Drawing.Size(80, 20);
            this.tbxOffsetV.TabIndex = 4;
            this.tbxOffsetV.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // Label2
            // 
            this.Label2.BackColor = System.Drawing.SystemColors.Control;
            this.Label2.Cursor = System.Windows.Forms.Cursors.Default;
            this.Label2.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Label2.Location = new System.Drawing.Point(178, 8);
            this.Label2.Name = "Label2";
            this.Label2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Label2.Size = new System.Drawing.Size(96, 32);
            this.Label2.TabIndex = 5;
            this.Label2.Text = "HORIZONTAL OFFSET (FT.)   +/-";
            this.Label2.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // tbxOffsetH
            // 
            this.tbxOffsetH.AcceptsReturn = true;
            this.tbxOffsetH.BackColor = System.Drawing.SystemColors.Window;
            this.tbxOffsetH.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.tbxOffsetH.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbxOffsetH.ForeColor = System.Drawing.SystemColors.WindowText;
            this.tbxOffsetH.Location = new System.Drawing.Point(186, 45);
            this.tbxOffsetH.MaxLength = 0;
            this.tbxOffsetH.Name = "tbxOffsetH";
            this.tbxOffsetH.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.tbxOffsetH.Size = new System.Drawing.Size(80, 20);
            this.tbxOffsetH.TabIndex = 6;
            this.tbxOffsetH.Text = "10";
            this.tbxOffsetH.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // Frame11
            // 
            this.Frame11.BackColor = System.Drawing.SystemColors.Control;
            this.Frame11.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Frame11.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Frame11.Location = new System.Drawing.Point(2, 152);
            this.Frame11.Name = "Frame11";
            this.Frame11.Padding = new System.Windows.Forms.Padding(0);
            this.Frame11.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Frame11.Size = new System.Drawing.Size(416, 56);
            this.Frame11.TabIndex = 7;
            this.Frame11.TabStop = false;
            this.Frame11.Text = "10.  STAKE";
            // 
            // Frame10
            // 
            this.Frame10.BackColor = System.Drawing.SystemColors.Control;
            this.Frame10.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Frame10.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Frame10.Location = new System.Drawing.Point(12, 730);
            this.Frame10.Name = "Frame10";
            this.Frame10.Padding = new System.Windows.Forms.Padding(0);
            this.Frame10.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Frame10.Size = new System.Drawing.Size(410, 56);
            this.Frame10.TabIndex = 18;
            this.Frame10.TabStop = false;
            this.Frame10.Text = "10.  STAKE";
            // 
            // cmdDone
            // 
            this.cmdDone.Location = new System.Drawing.Point(279, 796);
            this.cmdDone.Name = "cmdDone";
            this.cmdDone.Size = new System.Drawing.Size(143, 41);
            this.cmdDone.TabIndex = 20;
            this.cmdDone.Text = "DONE";
            this.cmdDone.UseVisualStyleBackColor = true;
            this.cmdDone.Click += new System.EventHandler(this.cmdDone_Click);
            // 
            // cmdCancel
            // 
            this.cmdCancel.Location = new System.Drawing.Point(129, 796);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(143, 41);
            this.cmdCancel.TabIndex = 21;
            this.cmdCancel.Text = "CANCEL";
            this.cmdCancel.UseVisualStyleBackColor = true;
            this.cmdCancel.Click += new System.EventHandler(this.cmdCancel_Click);
            // 
            // frmGrid
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(472, 742);
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.cmdDone);
            this.Controls.Add(this.Frame07);
            this.Controls.Add(this.Frame08);
            this.Controls.Add(this.Frame06);
            this.Controls.Add(this.Frame09);
            this.Controls.Add(this.Frame10);
            this.Controls.Add(this.Frame05);
            this.Controls.Add(this.Frame04);
            this.Controls.Add(this.Frame03);
            this.Controls.Add(this.Frame02);
            this.Controls.Add(this.Frame01);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmGrid";
            this.Text = "STAKE - BUILDING GRID";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmGrid_FormClosing);
            this.Load += new System.EventHandler(this.frmGrid_Load);
            this.Frame01.ResumeLayout(false);
            this.Frame03.ResumeLayout(false);
            this.Frame04.ResumeLayout(false);
            this.Frame05.ResumeLayout(false);
            this.Frame07.ResumeLayout(false);
            this.Frame06.ResumeLayout(false);
            this.Frame09.ResumeLayout(false);
            this.Frame09.PerformLayout();
            this.Frame7B.ResumeLayout(false);
            this.Frame7B.PerformLayout();
            this.Frame7C.ResumeLayout(false);
            this.Frame7C.PerformLayout();
            this.Frame7A.ResumeLayout(false);
            this.Frame7A.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.GroupBox Frame01;
        public System.Windows.Forms.Button cmdSelectGrid;
        public System.Windows.Forms.GroupBox Frame02;
        public System.Windows.Forms.GroupBox Frame03;
        public System.Windows.Forms.Button cmdIdGridSets;
        public System.Windows.Forms.Button cmdEditBuildNames;
        public System.Windows.Forms.GroupBox Frame04;
        public System.Windows.Forms.Button cmdGetNumeric;
        public System.Windows.Forms.Button cmdGetAlpha;
        public System.Windows.Forms.Button cmdIdSecondary;
        public System.Windows.Forms.GroupBox Frame05;
        public System.Windows.Forms.ComboBox cbxAlpha;
        public System.Windows.Forms.ComboBox cbxNumeric;
        public System.Windows.Forms.Label lblNumeric;
        public System.Windows.Forms.Label lblAlpha;
        public System.Windows.Forms.Button cmdGridLabel;
        public System.Windows.Forms.CheckBox cboxUseI;
        public System.Windows.Forms.GroupBox Frame07;
        public System.Windows.Forms.Button cmdAlignCreate;
        public System.Windows.Forms.Button cmdAlignDelete;
        public System.Windows.Forms.GroupBox Frame08;
        public System.Windows.Forms.GroupBox Frame06;
        public System.Windows.Forms.Button cmdGridEdit;
        public System.Windows.Forms.Button cmdGridAdd;
        public System.Windows.Forms.Button cmdGridDelete;
        public System.Windows.Forms.GroupBox Frame09;
        public System.Windows.Forms.GroupBox Frame7B;
        internal System.Windows.Forms.RadioButton optIN;
        internal System.Windows.Forms.RadioButton optOUT;
        internal System.Windows.Forms.RadioButton optBOTH;
        public System.Windows.Forms.GroupBox Frame7C;
        internal System.Windows.Forms.RadioButton optALL;
        internal System.Windows.Forms.RadioButton optALT;
        public System.Windows.Forms.GroupBox Frame7A;
        internal System.Windows.Forms.RadioButton optRBC;
        internal System.Windows.Forms.RadioButton optPBC;
        public System.Windows.Forms.Label Label1;
        public System.Windows.Forms.TextBox tbxOffsetV;
        public System.Windows.Forms.Label Label2;
        public System.Windows.Forms.TextBox tbxOffsetH;
        public System.Windows.Forms.GroupBox Frame11;
        public System.Windows.Forms.GroupBox Frame10;
        public System.Windows.Forms.CheckBox cboxOmitO;
        private System.Windows.Forms.Button cmdDone;
        private System.Windows.Forms.Button cmdCancel;
        public System.Windows.Forms.Button cmdDeleteAll;
    }
}