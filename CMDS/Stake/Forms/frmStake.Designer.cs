namespace Stake.Forms
{
    partial class frmStake
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.optUG = new System.Windows.Forms.RadioButton();
            this.optWALL = new System.Windows.Forms.RadioButton();
            this.optSD = new System.Windows.Forms.RadioButton();
            this.optSEWER = new System.Windows.Forms.RadioButton();
            this.optMISC = new System.Windows.Forms.RadioButton();
            this.optFL = new System.Windows.Forms.RadioButton();
            this.optCURB = new System.Windows.Forms.RadioButton();
            this.optBLDG = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cmdAlignReverse = new System.Windows.Forms.Button();
            this.cmdActivateAlign = new System.Windows.Forms.Button();
            this.cmdGetEnt = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.cmdCurbTrans = new System.Windows.Forms.Button();
            this.cmdAddCrossings = new System.Windows.Forms.Button();
            this.cmdCopyStyles = new System.Windows.Forms.Button();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.cmdStakeSingle = new System.Windows.Forms.Button();
            this.cmdStakeSta = new System.Windows.Forms.Button();
            this.cmdStakeAll = new System.Windows.Forms.Button();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.cmdExportPoints = new System.Windows.Forms.Button();
            this.cmdAddPointsToAlign = new System.Windows.Forms.Button();
            this.cmdFlipSideAdd = new System.Windows.Forms.Button();
            this.cmdFlipSideMove = new System.Windows.Forms.Button();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.cmdPrintDictionary = new System.Windows.Forms.Button();
            this.cmdChangeAlignStartPoint = new System.Windows.Forms.Button();
            this.cmdPrintCollection = new System.Windows.Forms.Button();
            this.cbxUseRefAlign = new System.Windows.Forms.CheckBox();
            this.lblAlignRefName = new System.Windows.Forms.Label();
            this.Frame3 = new System.Windows.Forms.GroupBox();
            this.lblOffset = new System.Windows.Forms.Label();
            this.cboOffset = new System.Windows.Forms.ComboBox();
            this.lblInterval = new System.Windows.Forms.Label();
            this.cboInterval = new System.Windows.Forms.ComboBox();
            this.lblHeight = new System.Windows.Forms.Label();
            this.cboHeight = new System.Windows.Forms.ComboBox();
            this.Frame1a = new System.Windows.Forms.GroupBox();
            this.optNo = new System.Windows.Forms.RadioButton();
            this.optYes = new System.Windows.Forms.RadioButton();
            this.Frame1b = new System.Windows.Forms.GroupBox();
            this.cboTolerance = new System.Windows.Forms.ComboBox();
            this.lblDelta = new System.Windows.Forms.Label();
            this.cboDelta = new System.Windows.Forms.ComboBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.Frame3.SuspendLayout();
            this.Frame1a.SuspendLayout();
            this.Frame1b.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.optUG);
            this.groupBox1.Controls.Add(this.optWALL);
            this.groupBox1.Controls.Add(this.optSD);
            this.groupBox1.Controls.Add(this.optSEWER);
            this.groupBox1.Controls.Add(this.optMISC);
            this.groupBox1.Controls.Add(this.optFL);
            this.groupBox1.Controls.Add(this.optCURB);
            this.groupBox1.Controls.Add(this.optBLDG);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.groupBox1.Location = new System.Drawing.Point(4, 4);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(0);
            this.groupBox1.Size = new System.Drawing.Size(480, 60);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "OBJECT TYPE";
            // 
            // optUG
            // 
            this.optUG.AutoSize = true;
            this.optUG.Location = new System.Drawing.Point(360, 36);
            this.optUG.Name = "optUG";
            this.optUG.Size = new System.Drawing.Size(85, 17);
            this.optUG.TabIndex = 7;
            this.optUG.Text = "UG UTILITY";
            this.optUG.UseVisualStyleBackColor = true;
            this.optUG.Click += new System.EventHandler(this.optUG_Click);
            // 
            // optWALL
            // 
            this.optWALL.AutoSize = true;
            this.optWALL.Location = new System.Drawing.Point(236, 36);
            this.optWALL.Name = "optWALL";
            this.optWALL.Size = new System.Drawing.Size(55, 17);
            this.optWALL.TabIndex = 6;
            this.optWALL.Text = "WALL";
            this.optWALL.UseVisualStyleBackColor = true;
            this.optWALL.Click += new System.EventHandler(this.optWALL_Click);
            // 
            // optSD
            // 
            this.optSD.AutoSize = true;
            this.optSD.Location = new System.Drawing.Point(110, 36);
            this.optSD.Name = "optSD";
            this.optSD.Size = new System.Drawing.Size(101, 17);
            this.optSD.TabIndex = 5;
            this.optSD.Text = "STORM DRAIN";
            this.optSD.UseVisualStyleBackColor = true;
            this.optSD.Click += new System.EventHandler(this.optSD_Click);
            // 
            // optSEWER
            // 
            this.optSEWER.AutoSize = true;
            this.optSEWER.Location = new System.Drawing.Point(13, 36);
            this.optSEWER.Name = "optSEWER";
            this.optSEWER.Size = new System.Drawing.Size(65, 17);
            this.optSEWER.TabIndex = 4;
            this.optSEWER.Text = "SEWER";
            this.optSEWER.UseVisualStyleBackColor = true;
            this.optSEWER.Click += new System.EventHandler(this.optSEWER_Click);
            // 
            // optMISC
            // 
            this.optMISC.AutoSize = true;
            this.optMISC.Location = new System.Drawing.Point(360, 13);
            this.optMISC.Name = "optMISC";
            this.optMISC.Size = new System.Drawing.Size(115, 17);
            this.optMISC.TabIndex = 3;
            this.optMISC.Text = "MISCELLANEOUS";
            this.optMISC.UseVisualStyleBackColor = true;
            this.optMISC.Click += new System.EventHandler(this.optMISC_Click);
            // 
            // optFL
            // 
            this.optFL.AutoSize = true;
            this.optFL.Location = new System.Drawing.Point(236, 13);
            this.optFL.Name = "optFL";
            this.optFL.Size = new System.Drawing.Size(80, 17);
            this.optFL.TabIndex = 2;
            this.optFL.Text = "FLOWLINE";
            this.optFL.UseVisualStyleBackColor = true;
            this.optFL.Click += new System.EventHandler(this.optFL_Click);
            // 
            // optCURB
            // 
            this.optCURB.AutoSize = true;
            this.optCURB.Checked = true;
            this.optCURB.Location = new System.Drawing.Point(110, 13);
            this.optCURB.Name = "optCURB";
            this.optCURB.Size = new System.Drawing.Size(55, 17);
            this.optCURB.TabIndex = 1;
            this.optCURB.TabStop = true;
            this.optCURB.Text = "CURB";
            this.optCURB.UseVisualStyleBackColor = true;
            this.optCURB.Click += new System.EventHandler(this.optCURB_Click);
            // 
            // optBLDG
            // 
            this.optBLDG.AutoSize = true;
            this.optBLDG.CausesValidation = false;
            this.optBLDG.Location = new System.Drawing.Point(13, 13);
            this.optBLDG.Name = "optBLDG";
            this.optBLDG.Size = new System.Drawing.Size(76, 17);
            this.optBLDG.TabIndex = 0;
            this.optBLDG.Text = "BUILDING";
            this.optBLDG.UseVisualStyleBackColor = true;
            this.optBLDG.Click += new System.EventHandler(this.optBLDG_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cmdAlignReverse);
            this.groupBox2.Controls.Add(this.cmdActivateAlign);
            this.groupBox2.Controls.Add(this.cmdGetEnt);
            this.groupBox2.Location = new System.Drawing.Point(4, 64);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(0);
            this.groupBox2.Size = new System.Drawing.Size(480, 48);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            // 
            // cmdAlignReverse
            // 
            this.cmdAlignReverse.Location = new System.Drawing.Point(322, 10);
            this.cmdAlignReverse.Name = "cmdAlignReverse";
            this.cmdAlignReverse.Size = new System.Drawing.Size(156, 30);
            this.cmdAlignReverse.TabIndex = 2;
            this.cmdAlignReverse.Text = "REVERSE ALIGNMENT";
            this.cmdAlignReverse.UseVisualStyleBackColor = true;
            this.cmdAlignReverse.Click += new System.EventHandler(this.cmdAlignReverse_Click);
            // 
            // cmdActivateAlign
            // 
            this.cmdActivateAlign.Location = new System.Drawing.Point(170, 10);
            this.cmdActivateAlign.Name = "cmdActivateAlign";
            this.cmdActivateAlign.Size = new System.Drawing.Size(142, 30);
            this.cmdActivateAlign.TabIndex = 1;
            this.cmdActivateAlign.Text = "ACTIVATE ALIGNMENT";
            this.cmdActivateAlign.UseVisualStyleBackColor = true;
            this.cmdActivateAlign.Click += new System.EventHandler(this.cmdActivateAlign_Click);
            // 
            // cmdGetEnt
            // 
            this.cmdGetEnt.Location = new System.Drawing.Point(3, 10);
            this.cmdGetEnt.Name = "cmdGetEnt";
            this.cmdGetEnt.Size = new System.Drawing.Size(160, 30);
            this.cmdGetEnt.TabIndex = 0;
            this.cmdGetEnt.Text = "SELECT ENTITY TO STAKE";
            this.cmdGetEnt.UseVisualStyleBackColor = true;
            this.cmdGetEnt.Click += new System.EventHandler(this.cmdGetEnt_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.cmdCurbTrans);
            this.groupBox4.Controls.Add(this.cmdAddCrossings);
            this.groupBox4.Controls.Add(this.cmdCopyStyles);
            this.groupBox4.Location = new System.Drawing.Point(5, 255);
            this.groupBox4.Margin = new System.Windows.Forms.Padding(0);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Padding = new System.Windows.Forms.Padding(0);
            this.groupBox4.Size = new System.Drawing.Size(480, 50);
            this.groupBox4.TabIndex = 3;
            this.groupBox4.TabStop = false;
            // 
            // cmdCurbTrans
            // 
            this.cmdCurbTrans.Location = new System.Drawing.Point(180, 10);
            this.cmdCurbTrans.Name = "cmdCurbTrans";
            this.cmdCurbTrans.Size = new System.Drawing.Size(150, 30);
            this.cmdCurbTrans.TabIndex = 5;
            this.cmdCurbTrans.Text = "ID CURB TRANSITIONS";
            this.cmdCurbTrans.UseVisualStyleBackColor = true;
            // 
            // cmdAddCrossings
            // 
            this.cmdAddCrossings.Location = new System.Drawing.Point(342, 10);
            this.cmdAddCrossings.Name = "cmdAddCrossings";
            this.cmdAddCrossings.Size = new System.Drawing.Size(136, 30);
            this.cmdAddCrossings.TabIndex = 4;
            this.cmdAddCrossings.Text = "ADD CROSSINGS";
            this.cmdAddCrossings.UseVisualStyleBackColor = true;
            // 
            // cmdCopyStyles
            // 
            this.cmdCopyStyles.Location = new System.Drawing.Point(3, 10);
            this.cmdCopyStyles.Name = "cmdCopyStyles";
            this.cmdCopyStyles.Size = new System.Drawing.Size(163, 30);
            this.cmdCopyStyles.TabIndex = 3;
            this.cmdCopyStyles.Text = "COPY \"STAKE\" STYLES";
            this.cmdCopyStyles.UseVisualStyleBackColor = true;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.cmdStakeSingle);
            this.groupBox5.Controls.Add(this.cmdStakeSta);
            this.groupBox5.Controls.Add(this.cmdStakeAll);
            this.groupBox5.Location = new System.Drawing.Point(5, 305);
            this.groupBox5.Margin = new System.Windows.Forms.Padding(0);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Padding = new System.Windows.Forms.Padding(0);
            this.groupBox5.Size = new System.Drawing.Size(480, 83);
            this.groupBox5.TabIndex = 4;
            this.groupBox5.TabStop = false;
            // 
            // cmdStakeSingle
            // 
            this.cmdStakeSingle.Location = new System.Drawing.Point(322, 10);
            this.cmdStakeSingle.Name = "cmdStakeSingle";
            this.cmdStakeSingle.Size = new System.Drawing.Size(156, 63);
            this.cmdStakeSingle.TabIndex = 5;
            this.cmdStakeSingle.Text = "STAKE SINGLE POINT";
            this.cmdStakeSingle.UseVisualStyleBackColor = true;
            this.cmdStakeSingle.Click += new System.EventHandler(this.cmdStakeSingle_Click);
            // 
            // cmdStakeSta
            // 
            this.cmdStakeSta.Location = new System.Drawing.Point(169, 10);
            this.cmdStakeSta.Name = "cmdStakeSta";
            this.cmdStakeSta.Size = new System.Drawing.Size(136, 63);
            this.cmdStakeSta.TabIndex = 4;
            this.cmdStakeSta.Text = "STAKE POINTS ON ACTIVE ALIGNMENT BY STATION RANGE";
            this.cmdStakeSta.UseVisualStyleBackColor = true;
            // 
            // cmdStakeAll
            // 
            this.cmdStakeAll.Location = new System.Drawing.Point(3, 10);
            this.cmdStakeAll.Name = "cmdStakeAll";
            this.cmdStakeAll.Size = new System.Drawing.Size(149, 63);
            this.cmdStakeAll.TabIndex = 3;
            this.cmdStakeAll.Text = "STAKE ALL POINTS ON ACTIVE ALIGNMENT";
            this.cmdStakeAll.UseVisualStyleBackColor = true;
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.cmdExportPoints);
            this.groupBox6.Controls.Add(this.cmdAddPointsToAlign);
            this.groupBox6.Controls.Add(this.cmdFlipSideAdd);
            this.groupBox6.Controls.Add(this.cmdFlipSideMove);
            this.groupBox6.Location = new System.Drawing.Point(5, 388);
            this.groupBox6.Margin = new System.Windows.Forms.Padding(0);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Padding = new System.Windows.Forms.Padding(0);
            this.groupBox6.Size = new System.Drawing.Size(480, 60);
            this.groupBox6.TabIndex = 5;
            this.groupBox6.TabStop = false;
            // 
            // cmdExportPoints
            // 
            this.cmdExportPoints.Location = new System.Drawing.Point(361, 10);
            this.cmdExportPoints.Name = "cmdExportPoints";
            this.cmdExportPoints.Size = new System.Drawing.Size(116, 46);
            this.cmdExportPoints.TabIndex = 6;
            this.cmdExportPoints.Text = "EXPORT POINTS";
            this.cmdExportPoints.UseVisualStyleBackColor = true;
            // 
            // cmdAddPointsToAlign
            // 
            this.cmdAddPointsToAlign.Location = new System.Drawing.Point(235, 11);
            this.cmdAddPointsToAlign.Name = "cmdAddPointsToAlign";
            this.cmdAddPointsToAlign.Size = new System.Drawing.Size(120, 46);
            this.cmdAddPointsToAlign.TabIndex = 5;
            this.cmdAddPointsToAlign.Text = "ADD STAKED POINT TO ACTIVE ALIGN";
            this.cmdAddPointsToAlign.UseVisualStyleBackColor = true;
            this.cmdAddPointsToAlign.Click += new System.EventHandler(this.cmdAddPointsToAlign_Click);
            // 
            // cmdFlipSideAdd
            // 
            this.cmdFlipSideAdd.Location = new System.Drawing.Point(119, 10);
            this.cmdFlipSideAdd.Name = "cmdFlipSideAdd";
            this.cmdFlipSideAdd.Size = new System.Drawing.Size(110, 46);
            this.cmdFlipSideAdd.TabIndex = 4;
            this.cmdFlipSideAdd.Text = "ADD POINTS TO OPPOSITE SIDE";
            this.cmdFlipSideAdd.UseVisualStyleBackColor = true;
            // 
            // cmdFlipSideMove
            // 
            this.cmdFlipSideMove.Location = new System.Drawing.Point(3, 10);
            this.cmdFlipSideMove.Name = "cmdFlipSideMove";
            this.cmdFlipSideMove.Size = new System.Drawing.Size(110, 46);
            this.cmdFlipSideMove.TabIndex = 3;
            this.cmdFlipSideMove.Text = "MOVE POINTS TO OPPOSITE SIDE";
            this.cmdFlipSideMove.UseVisualStyleBackColor = true;
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.cmdPrintDictionary);
            this.groupBox7.Controls.Add(this.cmdChangeAlignStartPoint);
            this.groupBox7.Controls.Add(this.cmdPrintCollection);
            this.groupBox7.Location = new System.Drawing.Point(5, 448);
            this.groupBox7.Margin = new System.Windows.Forms.Padding(0);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Padding = new System.Windows.Forms.Padding(0);
            this.groupBox7.Size = new System.Drawing.Size(480, 60);
            this.groupBox7.TabIndex = 6;
            this.groupBox7.TabStop = false;
            // 
            // cmdPrintDictionary
            // 
            this.cmdPrintDictionary.Location = new System.Drawing.Point(322, 10);
            this.cmdPrintDictionary.Name = "cmdPrintDictionary";
            this.cmdPrintDictionary.Size = new System.Drawing.Size(156, 36);
            this.cmdPrintDictionary.TabIndex = 5;
            this.cmdPrintDictionary.Text = "PRINT DICTIONARY";
            this.cmdPrintDictionary.UseVisualStyleBackColor = true;
            // 
            // cmdChangeAlignStartPoint
            // 
            this.cmdChangeAlignStartPoint.Location = new System.Drawing.Point(180, 10);
            this.cmdChangeAlignStartPoint.Name = "cmdChangeAlignStartPoint";
            this.cmdChangeAlignStartPoint.Size = new System.Drawing.Size(136, 42);
            this.cmdChangeAlignStartPoint.TabIndex = 4;
            this.cmdChangeAlignStartPoint.Text = "CHANGE ALIGNMENT START POINT";
            this.cmdChangeAlignStartPoint.UseVisualStyleBackColor = true;
            // 
            // cmdPrintCollection
            // 
            this.cmdPrintCollection.Location = new System.Drawing.Point(3, 10);
            this.cmdPrintCollection.Name = "cmdPrintCollection";
            this.cmdPrintCollection.Size = new System.Drawing.Size(163, 36);
            this.cmdPrintCollection.TabIndex = 3;
            this.cmdPrintCollection.Text = "PRINT COLLECTION";
            this.cmdPrintCollection.UseVisualStyleBackColor = true;
            // 
            // cbxUseRefAlign
            // 
            this.cbxUseRefAlign.AutoSize = true;
            this.cbxUseRefAlign.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbxUseRefAlign.Location = new System.Drawing.Point(6, 13);
            this.cbxUseRefAlign.Name = "cbxUseRefAlign";
            this.cbxUseRefAlign.Size = new System.Drawing.Size(182, 17);
            this.cbxUseRefAlign.TabIndex = 0;
            this.cbxUseRefAlign.Text = "USE REFERENCE ALIGNMENT";
            this.cbxUseRefAlign.UseVisualStyleBackColor = true;
            // 
            // lblAlignRefName
            // 
            this.lblAlignRefName.AutoSize = true;
            this.lblAlignRefName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAlignRefName.Location = new System.Drawing.Point(201, 14);
            this.lblAlignRefName.Name = "lblAlignRefName";
            this.lblAlignRefName.Size = new System.Drawing.Size(63, 13);
            this.lblAlignRefName.TabIndex = 1;
            this.lblAlignRefName.Text = "REF ALIGN";
            // 
            // Frame3
            // 
            this.Frame3.BackColor = System.Drawing.SystemColors.Control;
            this.Frame3.Controls.Add(this.lblOffset);
            this.Frame3.Controls.Add(this.lblAlignRefName);
            this.Frame3.Controls.Add(this.cboOffset);
            this.Frame3.Controls.Add(this.lblInterval);
            this.Frame3.Controls.Add(this.cbxUseRefAlign);
            this.Frame3.Controls.Add(this.cboInterval);
            this.Frame3.Controls.Add(this.lblHeight);
            this.Frame3.Controls.Add(this.cboHeight);
            this.Frame3.Controls.Add(this.Frame1a);
            this.Frame3.Controls.Add(this.Frame1b);
            this.Frame3.Controls.Add(this.lblDelta);
            this.Frame3.Controls.Add(this.cboDelta);
            this.Frame3.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Frame3.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Frame3.Location = new System.Drawing.Point(5, 115);
            this.Frame3.Name = "Frame3";
            this.Frame3.Padding = new System.Windows.Forms.Padding(0);
            this.Frame3.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Frame3.Size = new System.Drawing.Size(477, 137);
            this.Frame3.TabIndex = 7;
            this.Frame3.TabStop = false;
            // 
            // lblOffset
            // 
            this.lblOffset.BackColor = System.Drawing.SystemColors.Control;
            this.lblOffset.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblOffset.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblOffset.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblOffset.Location = new System.Drawing.Point(7, 44);
            this.lblOffset.Name = "lblOffset";
            this.lblOffset.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblOffset.Size = new System.Drawing.Size(72, 16);
            this.lblOffset.TabIndex = 5;
            this.lblOffset.Text = "OFFSET (FT)";
            // 
            // cboOffset
            // 
            this.cboOffset.BackColor = System.Drawing.SystemColors.Window;
            this.cboOffset.Cursor = System.Windows.Forms.Cursors.Default;
            this.cboOffset.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboOffset.ForeColor = System.Drawing.SystemColors.WindowText;
            this.cboOffset.Location = new System.Drawing.Point(119, 40);
            this.cboOffset.Name = "cboOffset";
            this.cboOffset.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.cboOffset.Size = new System.Drawing.Size(56, 21);
            this.cboOffset.TabIndex = 0;
            // 
            // lblInterval
            // 
            this.lblInterval.BackColor = System.Drawing.SystemColors.Control;
            this.lblInterval.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblInterval.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblInterval.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblInterval.Location = new System.Drawing.Point(7, 76);
            this.lblInterval.Name = "lblInterval";
            this.lblInterval.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblInterval.Size = new System.Drawing.Size(80, 16);
            this.lblInterval.TabIndex = 6;
            this.lblInterval.Text = "INTERVAL (FT)";
            // 
            // cboInterval
            // 
            this.cboInterval.BackColor = System.Drawing.SystemColors.Window;
            this.cboInterval.Cursor = System.Windows.Forms.Cursors.Default;
            this.cboInterval.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboInterval.ForeColor = System.Drawing.SystemColors.WindowText;
            this.cboInterval.Location = new System.Drawing.Point(119, 72);
            this.cboInterval.Name = "cboInterval";
            this.cboInterval.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.cboInterval.Size = new System.Drawing.Size(56, 21);
            this.cboInterval.TabIndex = 1;
            // 
            // lblHeight
            // 
            this.lblHeight.BackColor = System.Drawing.SystemColors.Control;
            this.lblHeight.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblHeight.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHeight.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblHeight.Location = new System.Drawing.Point(7, 112);
            this.lblHeight.Name = "lblHeight";
            this.lblHeight.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblHeight.Size = new System.Drawing.Size(112, 16);
            this.lblHeight.TabIndex = 7;
            this.lblHeight.Text = "CURB HEIGHT  (IN)";
            // 
            // cboHeight
            // 
            this.cboHeight.BackColor = System.Drawing.SystemColors.Window;
            this.cboHeight.Cursor = System.Windows.Forms.Cursors.Default;
            this.cboHeight.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboHeight.ForeColor = System.Drawing.SystemColors.WindowText;
            this.cboHeight.Location = new System.Drawing.Point(119, 108);
            this.cboHeight.Name = "cboHeight";
            this.cboHeight.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.cboHeight.Size = new System.Drawing.Size(56, 21);
            this.cboHeight.TabIndex = 2;
            // 
            // Frame1a
            // 
            this.Frame1a.BackColor = System.Drawing.SystemColors.Control;
            this.Frame1a.Controls.Add(this.optNo);
            this.Frame1a.Controls.Add(this.optYes);
            this.Frame1a.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Frame1a.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Frame1a.Location = new System.Drawing.Point(204, 56);
            this.Frame1a.Name = "Frame1a";
            this.Frame1a.Padding = new System.Windows.Forms.Padding(0);
            this.Frame1a.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Frame1a.Size = new System.Drawing.Size(96, 48);
            this.Frame1a.TabIndex = 3;
            this.Frame1a.TabStop = false;
            this.Frame1a.Text = "ON STATION?";
            // 
            // optNo
            // 
            this.optNo.AutoSize = true;
            this.optNo.Location = new System.Drawing.Point(51, 16);
            this.optNo.Name = "optNo";
            this.optNo.Size = new System.Drawing.Size(41, 17);
            this.optNo.TabIndex = 3;
            this.optNo.TabStop = true;
            this.optNo.Text = "NO";
            this.optNo.UseVisualStyleBackColor = true;
            // 
            // optYes
            // 
            this.optYes.AutoSize = true;
            this.optYes.Location = new System.Drawing.Point(6, 16);
            this.optYes.Name = "optYes";
            this.optYes.Size = new System.Drawing.Size(46, 17);
            this.optYes.TabIndex = 2;
            this.optYes.TabStop = true;
            this.optYes.Text = "YES";
            this.optYes.UseVisualStyleBackColor = true;
            // 
            // Frame1b
            // 
            this.Frame1b.BackColor = System.Drawing.SystemColors.Control;
            this.Frame1b.Controls.Add(this.cboTolerance);
            this.Frame1b.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Frame1b.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Frame1b.Location = new System.Drawing.Point(327, 56);
            this.Frame1b.Name = "Frame1b";
            this.Frame1b.Padding = new System.Windows.Forms.Padding(0);
            this.Frame1b.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Frame1b.Size = new System.Drawing.Size(144, 48);
            this.Frame1b.TabIndex = 4;
            this.Frame1b.TabStop = false;
            this.Frame1b.Text = "SPACING TOLERANCE?";
            // 
            // cboTolerance
            // 
            this.cboTolerance.BackColor = System.Drawing.SystemColors.Window;
            this.cboTolerance.Cursor = System.Windows.Forms.Cursors.Default;
            this.cboTolerance.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboTolerance.ForeColor = System.Drawing.SystemColors.WindowText;
            this.cboTolerance.Location = new System.Drawing.Point(82, 16);
            this.cboTolerance.Name = "cboTolerance";
            this.cboTolerance.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.cboTolerance.Size = new System.Drawing.Size(56, 22);
            this.cboTolerance.TabIndex = 0;
            // 
            // lblDelta
            // 
            this.lblDelta.BackColor = System.Drawing.SystemColors.Control;
            this.lblDelta.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblDelta.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDelta.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblDelta.Location = new System.Drawing.Point(295, 112);
            this.lblDelta.Name = "lblDelta";
            this.lblDelta.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblDelta.Size = new System.Drawing.Size(112, 16);
            this.lblDelta.TabIndex = 8;
            this.lblDelta.Text = "DELTA (10< R<=50\')";
            // 
            // cboDelta
            // 
            this.cboDelta.BackColor = System.Drawing.SystemColors.Window;
            this.cboDelta.Cursor = System.Windows.Forms.Cursors.Default;
            this.cboDelta.Enabled = false;
            this.cboDelta.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboDelta.ForeColor = System.Drawing.SystemColors.WindowText;
            this.cboDelta.Location = new System.Drawing.Point(411, 108);
            this.cboDelta.Name = "cboDelta";
            this.cboDelta.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.cboDelta.Size = new System.Drawing.Size(56, 21);
            this.cboDelta.TabIndex = 9;
            // 
            // frmStake
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(490, 508);
            this.Controls.Add(this.Frame3);
            this.Controls.Add(this.groupBox7);
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MinimizeBox = false;
            this.Name = "frmStake";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "SURVEY STAKING";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmStake_FormClosing);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            this.groupBox7.ResumeLayout(false);
            this.Frame3.ResumeLayout(false);
            this.Frame3.PerformLayout();
            this.Frame1a.ResumeLayout(false);
            this.Frame1a.PerformLayout();
            this.Frame1b.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton optWALL;
        private System.Windows.Forms.RadioButton optSD;
        private System.Windows.Forms.RadioButton optSEWER;
        private System.Windows.Forms.RadioButton optFL;
        private System.Windows.Forms.RadioButton optCURB;
        private System.Windows.Forms.RadioButton optBLDG;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button cmdAlignReverse;
        private System.Windows.Forms.Button cmdActivateAlign;
        private System.Windows.Forms.Button cmdGetEnt;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button cmdCurbTrans;
        private System.Windows.Forms.Button cmdAddCrossings;
        private System.Windows.Forms.Button cmdCopyStyles;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Button cmdStakeSingle;
        private System.Windows.Forms.Button cmdStakeSta;
        private System.Windows.Forms.Button cmdStakeAll;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.Button cmdAddPointsToAlign;
        private System.Windows.Forms.Button cmdFlipSideAdd;
        private System.Windows.Forms.Button cmdFlipSideMove;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.Button cmdPrintDictionary;
        private System.Windows.Forms.Button cmdChangeAlignStartPoint;
        private System.Windows.Forms.Button cmdPrintCollection;
        private System.Windows.Forms.CheckBox cbxUseRefAlign;
        private System.Windows.Forms.Label lblAlignRefName;
        public System.Windows.Forms.GroupBox Frame3;
        public System.Windows.Forms.Label lblOffset;
        public System.Windows.Forms.ComboBox cboOffset;
        public System.Windows.Forms.Label lblInterval;
        public System.Windows.Forms.ComboBox cboInterval;
        public System.Windows.Forms.Label lblHeight;
        public System.Windows.Forms.ComboBox cboHeight;
        public System.Windows.Forms.GroupBox Frame1a;
        internal System.Windows.Forms.RadioButton optNo;
        internal System.Windows.Forms.RadioButton optYes;
        public System.Windows.Forms.GroupBox Frame1b;
        public System.Windows.Forms.ComboBox cboTolerance;
        public System.Windows.Forms.Label lblDelta;
        public System.Windows.Forms.ComboBox cboDelta;
        public System.Windows.Forms.RadioButton optUG;
        public System.Windows.Forms.RadioButton optMISC;
        private System.Windows.Forms.Button cmdExportPoints;
    }
}