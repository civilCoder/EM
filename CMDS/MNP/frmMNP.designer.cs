namespace MNP
{
    partial class frmMNP
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
            this.optFIRE = new System.Windows.Forms.RadioButton();
            this.optWTR = new System.Windows.Forms.RadioButton();
            this.optSS = new System.Windows.Forms.RadioButton();
            this.optSD = new System.Windows.Forms.RadioButton();
            this.cmdAlignMake = new System.Windows.Forms.Button();
            this.cmdEdit = new System.Windows.Forms.Button();
            this.cmdMakePipeNetwork = new System.Windows.Forms.Button();
            this.cmdProfileView = new System.Windows.Forms.Button();
            this.cmdEditNetwork = new System.Windows.Forms.Button();
            this.gbxAlignOpts = new System.Windows.Forms.GroupBox();
            this.s = new System.Windows.Forms.GroupBox();
            this.cbxLineLat = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tbxSuffix = new System.Windows.Forms.TextBox();
            this.tbxIndex = new System.Windows.Forms.TextBox();
            this.tbxType = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.optNumeric = new System.Windows.Forms.RadioButton();
            this.optAlpha = new System.Windows.Forms.RadioButton();
            this.gbxAlignStyleLabel = new System.Windows.Forms.GroupBox();
            this.cbxAlignLabelSetStyles = new System.Windows.Forms.ComboBox();
            this.gbxAlignStyle = new System.Windows.Forms.GroupBox();
            this.cbxAlignStyle = new System.Windows.Forms.ComboBox();
            this.cbxAddCurves = new System.Windows.Forms.CheckBox();
            this.gbxProfileStyle = new System.Windows.Forms.GroupBox();
            this.cbxProfileStyleDE = new System.Windows.Forms.ComboBox();
            this.gbxProfileOpts = new System.Windows.Forms.GroupBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.cbxProfileLabelSetEX = new System.Windows.Forms.ComboBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.cbxProfileStyleEX = new System.Windows.Forms.ComboBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.cbxProfileLabelSetDE = new System.Windows.Forms.ComboBox();
            this.gbxProfileViewStyle = new System.Windows.Forms.GroupBox();
            this.cbxProfileViewStyle = new System.Windows.Forms.ComboBox();
            this.StatusStrip1 = new System.Windows.Forms.StatusStrip();
            this.ToolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.cmdAlignActivate = new System.Windows.Forms.Button();
            this.cmdAddNetworkToProfileView = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.gbxAlignOpts.SuspendLayout();
            this.s.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.gbxAlignStyleLabel.SuspendLayout();
            this.gbxAlignStyle.SuspendLayout();
            this.gbxProfileStyle.SuspendLayout();
            this.gbxProfileOpts.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.gbxProfileViewStyle.SuspendLayout();
            this.StatusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.optFIRE);
            this.groupBox1.Controls.Add(this.optWTR);
            this.groupBox1.Controls.Add(this.optSS);
            this.groupBox1.Controls.Add(this.optSD);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(1, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(459, 47);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "UTILITY TYPE";
            // 
            // optFIRE
            // 
            this.optFIRE.AutoSize = true;
            this.optFIRE.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.optFIRE.Location = new System.Drawing.Point(395, 19);
            this.optFIRE.Name = "optFIRE";
            this.optFIRE.Size = new System.Drawing.Size(56, 20);
            this.optFIRE.TabIndex = 3;
            this.optFIRE.TabStop = true;
            this.optFIRE.Text = "FIRE";
            this.optFIRE.UseVisualStyleBackColor = true;
            this.optFIRE.Click += new System.EventHandler(this.optFIRE_Click);
            // 
            // optWTR
            // 
            this.optWTR.AutoSize = true;
            this.optWTR.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.optWTR.Location = new System.Drawing.Point(284, 19);
            this.optWTR.Name = "optWTR";
            this.optWTR.Size = new System.Drawing.Size(58, 20);
            this.optWTR.TabIndex = 2;
            this.optWTR.TabStop = true;
            this.optWTR.Text = "WTR";
            this.optWTR.UseVisualStyleBackColor = true;
            this.optWTR.Click += new System.EventHandler(this.optWTR_Click);
            // 
            // optSS
            // 
            this.optSS.AutoSize = true;
            this.optSS.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.optSS.Location = new System.Drawing.Point(146, 19);
            this.optSS.Name = "optSS";
            this.optSS.Size = new System.Drawing.Size(44, 20);
            this.optSS.TabIndex = 1;
            this.optSS.TabStop = true;
            this.optSS.Text = "SS";
            this.optSS.UseVisualStyleBackColor = true;
            this.optSS.Click += new System.EventHandler(this.optSS_Click);
            // 
            // optSD
            // 
            this.optSD.AutoSize = true;
            this.optSD.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.optSD.Location = new System.Drawing.Point(6, 19);
            this.optSD.Name = "optSD";
            this.optSD.Size = new System.Drawing.Size(45, 20);
            this.optSD.TabIndex = 0;
            this.optSD.TabStop = true;
            this.optSD.Text = "SD";
            this.optSD.UseVisualStyleBackColor = true;
            this.optSD.Click += new System.EventHandler(this.optSD_Click);
            // 
            // cmdAlignMake
            // 
            this.cmdAlignMake.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdAlignMake.Location = new System.Drawing.Point(5, 53);
            this.cmdAlignMake.Name = "cmdAlignMake";
            this.cmdAlignMake.Size = new System.Drawing.Size(78, 40);
            this.cmdAlignMake.TabIndex = 1;
            this.cmdAlignMake.Text = "Make Alignment";
            this.cmdAlignMake.UseVisualStyleBackColor = true;
            this.cmdAlignMake.Click += new System.EventHandler(this.cmdMakeAlign_Click);
            // 
            // cmdEdit
            // 
            this.cmdEdit.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdEdit.Location = new System.Drawing.Point(5, 99);
            this.cmdEdit.Name = "cmdEdit";
            this.cmdEdit.Size = new System.Drawing.Size(162, 40);
            this.cmdEdit.TabIndex = 2;
            this.cmdEdit.Text = "Edit Alignment";
            this.cmdEdit.UseVisualStyleBackColor = true;
            this.cmdEdit.Click += new System.EventHandler(this.cmdEdit_Click);
            // 
            // cmdMakePipeNetwork
            // 
            this.cmdMakePipeNetwork.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdMakePipeNetwork.Location = new System.Drawing.Point(5, 191);
            this.cmdMakePipeNetwork.Name = "cmdMakePipeNetwork";
            this.cmdMakePipeNetwork.Size = new System.Drawing.Size(162, 40);
            this.cmdMakePipeNetwork.TabIndex = 4;
            this.cmdMakePipeNetwork.Text = "Make Pipe Network";
            this.cmdMakePipeNetwork.UseVisualStyleBackColor = true;
            this.cmdMakePipeNetwork.Click += new System.EventHandler(this.cmdMakePipeNetwork_Click);
            // 
            // cmdProfileView
            // 
            this.cmdProfileView.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdProfileView.Location = new System.Drawing.Point(5, 145);
            this.cmdProfileView.Name = "cmdProfileView";
            this.cmdProfileView.Size = new System.Drawing.Size(162, 40);
            this.cmdProfileView.TabIndex = 3;
            this.cmdProfileView.Text = "Display Profile View";
            this.cmdProfileView.UseVisualStyleBackColor = true;
            this.cmdProfileView.Click += new System.EventHandler(this.cmdProfileView_Click);
            // 
            // cmdEditNetwork
            // 
            this.cmdEditNetwork.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdEditNetwork.Location = new System.Drawing.Point(7, 237);
            this.cmdEditNetwork.Name = "cmdEditNetwork";
            this.cmdEditNetwork.Size = new System.Drawing.Size(162, 40);
            this.cmdEditNetwork.TabIndex = 5;
            this.cmdEditNetwork.Text = "Edit Pipe Network";
            this.cmdEditNetwork.UseVisualStyleBackColor = true;
            this.cmdEditNetwork.Click += new System.EventHandler(this.cmdEditNetwork_Click);
            // 
            // gbxAlignOpts
            // 
            this.gbxAlignOpts.Controls.Add(this.s);
            this.gbxAlignOpts.Controls.Add(this.gbxAlignStyleLabel);
            this.gbxAlignOpts.Controls.Add(this.gbxAlignStyle);
            this.gbxAlignOpts.Controls.Add(this.cbxAddCurves);
            this.gbxAlignOpts.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbxAlignOpts.Location = new System.Drawing.Point(175, 53);
            this.gbxAlignOpts.Name = "gbxAlignOpts";
            this.gbxAlignOpts.Size = new System.Drawing.Size(285, 285);
            this.gbxAlignOpts.TabIndex = 9;
            this.gbxAlignOpts.TabStop = false;
            this.gbxAlignOpts.Text = "Alignment Options:";
            // 
            // s
            // 
            this.s.Controls.Add(this.cbxLineLat);
            this.s.Controls.Add(this.label4);
            this.s.Controls.Add(this.label2);
            this.s.Controls.Add(this.label3);
            this.s.Controls.Add(this.label1);
            this.s.Controls.Add(this.tbxSuffix);
            this.s.Controls.Add(this.tbxIndex);
            this.s.Controls.Add(this.tbxType);
            this.s.Controls.Add(this.groupBox2);
            this.s.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.s.Location = new System.Drawing.Point(5, 46);
            this.s.Name = "s";
            this.s.Size = new System.Drawing.Size(272, 121);
            this.s.TabIndex = 15;
            this.s.TabStop = false;
            this.s.Text = "Alignment Name Builder";
            // 
            // cbxLineLat
            // 
            this.cbxLineLat.AllowDrop = true;
            this.cbxLineLat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxLineLat.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.cbxLineLat.FormattingEnabled = true;
            this.cbxLineLat.Items.AddRange(new object[] {
            "LINE",
            "LAT"});
            this.cbxLineLat.Location = new System.Drawing.Point(59, 90);
            this.cbxLineLat.Name = "cbxLineLat";
            this.cbxLineLat.Size = new System.Drawing.Size(70, 23);
            this.cbxLineLat.TabIndex = 21;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(56, 76);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(47, 13);
            this.label4.TabIndex = 20;
            this.label4.Text = "Line/Lat";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(6, 76);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(31, 13);
            this.label2.TabIndex = 19;
            this.label2.Text = "Type";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(180, 76);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(33, 13);
            this.label3.TabIndex = 18;
            this.label3.Text = "Suffix";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(136, 76);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(33, 13);
            this.label1.TabIndex = 17;
            this.label1.Text = "Index";
            // 
            // tbxSuffix
            // 
            this.tbxSuffix.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbxSuffix.Location = new System.Drawing.Point(175, 92);
            this.tbxSuffix.Name = "tbxSuffix";
            this.tbxSuffix.Size = new System.Drawing.Size(50, 21);
            this.tbxSuffix.TabIndex = 15;
            // 
            // tbxIndex
            // 
            this.tbxIndex.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbxIndex.Location = new System.Drawing.Point(136, 92);
            this.tbxIndex.Name = "tbxIndex";
            this.tbxIndex.Size = new System.Drawing.Size(33, 21);
            this.tbxIndex.TabIndex = 14;
            // 
            // tbxType
            // 
            this.tbxType.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbxType.Location = new System.Drawing.Point(6, 92);
            this.tbxType.Name = "tbxType";
            this.tbxType.Size = new System.Drawing.Size(44, 21);
            this.tbxType.TabIndex = 13;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.optNumeric);
            this.groupBox2.Controls.Add(this.optAlpha);
            this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(6, 20);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(142, 37);
            this.groupBox2.TabIndex = 11;
            this.groupBox2.TabStop = false;
            // 
            // optNumeric
            // 
            this.optNumeric.AutoSize = true;
            this.optNumeric.Location = new System.Drawing.Point(76, 16);
            this.optNumeric.Name = "optNumeric";
            this.optNumeric.Size = new System.Drawing.Size(64, 17);
            this.optNumeric.TabIndex = 1;
            this.optNumeric.TabStop = true;
            this.optNumeric.Text = "Numeric";
            this.optNumeric.UseVisualStyleBackColor = true;
            this.optNumeric.Click += new System.EventHandler(this.optNumeric_Click);
            // 
            // optAlpha
            // 
            this.optAlpha.AutoSize = true;
            this.optAlpha.Location = new System.Drawing.Point(18, 16);
            this.optAlpha.Name = "optAlpha";
            this.optAlpha.Size = new System.Drawing.Size(52, 17);
            this.optAlpha.TabIndex = 0;
            this.optAlpha.TabStop = true;
            this.optAlpha.Text = "Alpha";
            this.optAlpha.UseVisualStyleBackColor = true;
            this.optAlpha.Click += new System.EventHandler(this.optAlpha_Click);
            // 
            // gbxAlignStyleLabel
            // 
            this.gbxAlignStyleLabel.Controls.Add(this.cbxAlignLabelSetStyles);
            this.gbxAlignStyleLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbxAlignStyleLabel.Location = new System.Drawing.Point(7, 230);
            this.gbxAlignStyleLabel.Name = "gbxAlignStyleLabel";
            this.gbxAlignStyleLabel.Size = new System.Drawing.Size(272, 48);
            this.gbxAlignStyleLabel.TabIndex = 14;
            this.gbxAlignStyleLabel.TabStop = false;
            this.gbxAlignStyleLabel.Text = "Alignment Label Set";
            // 
            // cbxAlignLabelSetStyles
            // 
            this.cbxAlignLabelSetStyles.AllowDrop = true;
            this.cbxAlignLabelSetStyles.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxAlignLabelSetStyles.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.cbxAlignLabelSetStyles.FormattingEnabled = true;
            this.cbxAlignLabelSetStyles.Location = new System.Drawing.Point(57, 20);
            this.cbxAlignLabelSetStyles.Name = "cbxAlignLabelSetStyles";
            this.cbxAlignLabelSetStyles.Size = new System.Drawing.Size(205, 23);
            this.cbxAlignLabelSetStyles.TabIndex = 1;
            // 
            // gbxAlignStyle
            // 
            this.gbxAlignStyle.Controls.Add(this.cbxAlignStyle);
            this.gbxAlignStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbxAlignStyle.Location = new System.Drawing.Point(7, 176);
            this.gbxAlignStyle.Name = "gbxAlignStyle";
            this.gbxAlignStyle.Size = new System.Drawing.Size(272, 48);
            this.gbxAlignStyle.TabIndex = 13;
            this.gbxAlignStyle.TabStop = false;
            this.gbxAlignStyle.Text = "Alignment Style";
            // 
            // cbxAlignStyle
            // 
            this.cbxAlignStyle.AllowDrop = true;
            this.cbxAlignStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxAlignStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.cbxAlignStyle.FormattingEnabled = true;
            this.cbxAlignStyle.Location = new System.Drawing.Point(57, 19);
            this.cbxAlignStyle.Name = "cbxAlignStyle";
            this.cbxAlignStyle.Size = new System.Drawing.Size(205, 23);
            this.cbxAlignStyle.TabIndex = 0;
            this.cbxAlignStyle.SelectedIndexChanged += new System.EventHandler(this.cbxAlignStyle_SelectedIndexChanged);
            // 
            // cbxAddCurves
            // 
            this.cbxAddCurves.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbxAddCurves.Location = new System.Drawing.Point(6, 19);
            this.cbxAddCurves.Name = "cbxAddCurves";
            this.cbxAddCurves.Size = new System.Drawing.Size(265, 27);
            this.cbxAddCurves.TabIndex = 9;
            this.cbxAddCurves.Text = "Add curves at PIs (default radius = 200\')";
            this.cbxAddCurves.UseVisualStyleBackColor = true;
            // 
            // gbxProfileStyle
            // 
            this.gbxProfileStyle.Controls.Add(this.cbxProfileStyleDE);
            this.gbxProfileStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbxProfileStyle.Location = new System.Drawing.Point(7, 21);
            this.gbxProfileStyle.Name = "gbxProfileStyle";
            this.gbxProfileStyle.Size = new System.Drawing.Size(200, 48);
            this.gbxProfileStyle.TabIndex = 15;
            this.gbxProfileStyle.TabStop = false;
            this.gbxProfileStyle.Text = "Profile Style - Design";
            // 
            // cbxProfileStyleDE
            // 
            this.cbxProfileStyleDE.AllowDrop = true;
            this.cbxProfileStyleDE.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxProfileStyleDE.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.cbxProfileStyleDE.FormattingEnabled = true;
            this.cbxProfileStyleDE.Location = new System.Drawing.Point(6, 19);
            this.cbxProfileStyleDE.Name = "cbxProfileStyleDE";
            this.cbxProfileStyleDE.Size = new System.Drawing.Size(180, 23);
            this.cbxProfileStyleDE.TabIndex = 2;
            // 
            // gbxProfileOpts
            // 
            this.gbxProfileOpts.Controls.Add(this.groupBox5);
            this.gbxProfileOpts.Controls.Add(this.groupBox4);
            this.gbxProfileOpts.Controls.Add(this.groupBox3);
            this.gbxProfileOpts.Controls.Add(this.gbxProfileStyle);
            this.gbxProfileOpts.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbxProfileOpts.Location = new System.Drawing.Point(7, 344);
            this.gbxProfileOpts.Name = "gbxProfileOpts";
            this.gbxProfileOpts.Size = new System.Drawing.Size(453, 135);
            this.gbxProfileOpts.TabIndex = 16;
            this.gbxProfileOpts.TabStop = false;
            this.gbxProfileOpts.Text = "Profile Options";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.cbxProfileLabelSetEX);
            this.groupBox5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox5.Location = new System.Drawing.Point(237, 75);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(200, 48);
            this.groupBox5.TabIndex = 19;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Profile Label Set - Existing";
            // 
            // cbxProfileLabelSetEX
            // 
            this.cbxProfileLabelSetEX.AllowDrop = true;
            this.cbxProfileLabelSetEX.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxProfileLabelSetEX.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.cbxProfileLabelSetEX.FormattingEnabled = true;
            this.cbxProfileLabelSetEX.Location = new System.Drawing.Point(6, 19);
            this.cbxProfileLabelSetEX.Name = "cbxProfileLabelSetEX";
            this.cbxProfileLabelSetEX.Size = new System.Drawing.Size(180, 23);
            this.cbxProfileLabelSetEX.TabIndex = 2;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.cbxProfileStyleEX);
            this.groupBox4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox4.Location = new System.Drawing.Point(239, 23);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(200, 48);
            this.groupBox4.TabIndex = 18;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Profile Style - Existing";
            // 
            // cbxProfileStyleEX
            // 
            this.cbxProfileStyleEX.AllowDrop = true;
            this.cbxProfileStyleEX.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxProfileStyleEX.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.cbxProfileStyleEX.FormattingEnabled = true;
            this.cbxProfileStyleEX.Location = new System.Drawing.Point(6, 19);
            this.cbxProfileStyleEX.Name = "cbxProfileStyleEX";
            this.cbxProfileStyleEX.Size = new System.Drawing.Size(180, 23);
            this.cbxProfileStyleEX.TabIndex = 2;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.cbxProfileLabelSetDE);
            this.groupBox3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox3.Location = new System.Drawing.Point(7, 75);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(200, 48);
            this.groupBox3.TabIndex = 17;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Profile Label Set - Design";
            // 
            // cbxProfileLabelSetDE
            // 
            this.cbxProfileLabelSetDE.AllowDrop = true;
            this.cbxProfileLabelSetDE.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxProfileLabelSetDE.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.cbxProfileLabelSetDE.FormattingEnabled = true;
            this.cbxProfileLabelSetDE.Location = new System.Drawing.Point(6, 19);
            this.cbxProfileLabelSetDE.Name = "cbxProfileLabelSetDE";
            this.cbxProfileLabelSetDE.Size = new System.Drawing.Size(180, 23);
            this.cbxProfileLabelSetDE.TabIndex = 2;
            // 
            // gbxProfileViewStyle
            // 
            this.gbxProfileViewStyle.Controls.Add(this.cbxProfileViewStyle);
            this.gbxProfileViewStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbxProfileViewStyle.Location = new System.Drawing.Point(14, 485);
            this.gbxProfileViewStyle.Name = "gbxProfileViewStyle";
            this.gbxProfileViewStyle.Size = new System.Drawing.Size(200, 48);
            this.gbxProfileViewStyle.TabIndex = 16;
            this.gbxProfileViewStyle.TabStop = false;
            this.gbxProfileViewStyle.Text = "Profile View Style";
            // 
            // cbxProfileViewStyle
            // 
            this.cbxProfileViewStyle.AllowDrop = true;
            this.cbxProfileViewStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxProfileViewStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbxProfileViewStyle.FormattingEnabled = true;
            this.cbxProfileViewStyle.Location = new System.Drawing.Point(8, 19);
            this.cbxProfileViewStyle.Name = "cbxProfileViewStyle";
            this.cbxProfileViewStyle.Size = new System.Drawing.Size(180, 23);
            this.cbxProfileViewStyle.TabIndex = 3;
            // 
            // StatusStrip1
            // 
            this.StatusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripStatusLabel1});
            this.StatusStrip1.Location = new System.Drawing.Point(0, 540);
            this.StatusStrip1.Name = "StatusStrip1";
            this.StatusStrip1.Size = new System.Drawing.Size(466, 22);
            this.StatusStrip1.TabIndex = 48;
            this.StatusStrip1.Text = "StatusStrip1";
            // 
            // ToolStripStatusLabel1
            // 
            this.ToolStripStatusLabel1.Name = "ToolStripStatusLabel1";
            this.ToolStripStatusLabel1.Size = new System.Drawing.Size(86, 17);
            this.ToolStripStatusLabel1.Text = "ACTIVE ALIGN:";
            // 
            // cmdAlignActivate
            // 
            this.cmdAlignActivate.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdAlignActivate.Location = new System.Drawing.Point(88, 53);
            this.cmdAlignActivate.Name = "cmdAlignActivate";
            this.cmdAlignActivate.Size = new System.Drawing.Size(78, 40);
            this.cmdAlignActivate.TabIndex = 49;
            this.cmdAlignActivate.Text = "Activate Alignment";
            this.cmdAlignActivate.UseVisualStyleBackColor = true;
            // 
            // cmdAddNetworkToProfileView
            // 
            this.cmdAddNetworkToProfileView.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdAddNetworkToProfileView.Location = new System.Drawing.Point(7, 283);
            this.cmdAddNetworkToProfileView.Name = "cmdAddNetworkToProfileView";
            this.cmdAddNetworkToProfileView.Size = new System.Drawing.Size(162, 40);
            this.cmdAddNetworkToProfileView.TabIndex = 6;
            this.cmdAddNetworkToProfileView.Text = "Add Network to Profile View";
            this.cmdAddNetworkToProfileView.UseVisualStyleBackColor = true;
            this.cmdAddNetworkToProfileView.Click += new System.EventHandler(this.cmdAddNetworkToProfileView_Click);
            // 
            // frmMNP
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(466, 562);
            this.Controls.Add(this.cmdAlignActivate);
            this.Controls.Add(this.StatusStrip1);
            this.Controls.Add(this.gbxProfileViewStyle);
            this.Controls.Add(this.gbxProfileOpts);
            this.Controls.Add(this.gbxAlignOpts);
            this.Controls.Add(this.cmdAddNetworkToProfileView);
            this.Controls.Add(this.cmdEditNetwork);
            this.Controls.Add(this.cmdMakePipeNetwork);
            this.Controls.Add(this.cmdProfileView);
            this.Controls.Add(this.cmdEdit);
            this.Controls.Add(this.cmdAlignMake);
            this.Controls.Add(this.groupBox1);
            this.Name = "frmMNP";
            this.Text = "Create Utility Network";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMNP_FormClosing);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.gbxAlignOpts.ResumeLayout(false);
            this.s.ResumeLayout(false);
            this.s.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.gbxAlignStyleLabel.ResumeLayout(false);
            this.gbxAlignStyle.ResumeLayout(false);
            this.gbxProfileStyle.ResumeLayout(false);
            this.gbxProfileOpts.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.gbxProfileViewStyle.ResumeLayout(false);
            this.StatusStrip1.ResumeLayout(false);
            this.StatusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton optFIRE;
        private System.Windows.Forms.RadioButton optWTR;
        private System.Windows.Forms.RadioButton optSS;
        private System.Windows.Forms.RadioButton optSD;
        private System.Windows.Forms.Button cmdAlignMake;
        private System.Windows.Forms.Button cmdEdit;
        private System.Windows.Forms.Button cmdMakePipeNetwork;
        private System.Windows.Forms.Button cmdProfileView;
        private System.Windows.Forms.Button cmdEditNetwork;
        private System.Windows.Forms.GroupBox gbxAlignOpts;
        private System.Windows.Forms.GroupBox gbxAlignStyleLabel;
        private System.Windows.Forms.GroupBox gbxAlignStyle;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton optNumeric;
        private System.Windows.Forms.RadioButton optAlpha;
        private System.Windows.Forms.CheckBox cbxAddCurves;
        private System.Windows.Forms.GroupBox gbxProfileStyle;
        private System.Windows.Forms.GroupBox gbxProfileOpts;
        private System.Windows.Forms.GroupBox gbxProfileViewStyle;
        internal System.Windows.Forms.StatusStrip StatusStrip1;
        internal System.Windows.Forms.ToolStripStatusLabel ToolStripStatusLabel1;
        private System.Windows.Forms.Button cmdAlignActivate;
        private System.Windows.Forms.GroupBox s;
        public System.Windows.Forms.TextBox tbxIndex;
        public System.Windows.Forms.TextBox tbxType;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.TextBox tbxSuffix;
        private System.Windows.Forms.ComboBox cbxAlignStyle;
        private System.Windows.Forms.ComboBox cbxLineLat;
        private System.Windows.Forms.ComboBox cbxAlignLabelSetStyles;
        private System.Windows.Forms.ComboBox cbxProfileStyleDE;
        private System.Windows.Forms.ComboBox cbxProfileViewStyle;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ComboBox cbxProfileLabelSetDE;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.ComboBox cbxProfileStyleEX;
        private System.Windows.Forms.Button cmdAddNetworkToProfileView;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.ComboBox cbxProfileLabelSetEX;

    }
}