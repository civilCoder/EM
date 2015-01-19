namespace Stake.Forms
{
    partial class frmExport
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
            this.cmdCreateCutSheet = new System.Windows.Forms.Button();
            this.fraSelectPnts = new System.Windows.Forms.GroupBox();
            this.cmdOnScreen = new System.Windows.Forms.Button();
            this.cmdByRange = new System.Windows.Forms.Button();
            this.cmdByAlignment = new System.Windows.Forms.Button();
            this.fraHeadings = new System.Windows.Forms.GroupBox();
            this.cmdLayer = new System.Windows.Forms.Button();
            this.cmdObject = new System.Windows.Forms.Button();
            this.lblPartialRange = new System.Windows.Forms.Label();
            this.lblNumPoints = new System.Windows.Forms.Label();
            this.cmdRange = new System.Windows.Forms.Button();
            this.fraPntDesc = new System.Windows.Forms.GroupBox();
            this.fraSelectPnts.SuspendLayout();
            this.fraHeadings.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmdCreateCutSheet
            // 
            this.cmdCreateCutSheet.BackColor = System.Drawing.SystemColors.Control;
            this.cmdCreateCutSheet.Cursor = System.Windows.Forms.Cursors.Default;
            this.cmdCreateCutSheet.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdCreateCutSheet.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cmdCreateCutSheet.Location = new System.Drawing.Point(542, 452);
            this.cmdCreateCutSheet.Name = "cmdCreateCutSheet";
            this.cmdCreateCutSheet.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.cmdCreateCutSheet.Size = new System.Drawing.Size(182, 32);
            this.cmdCreateCutSheet.TabIndex = 4;
            this.cmdCreateCutSheet.Text = "CREATE CUTSHEET";
            this.cmdCreateCutSheet.UseVisualStyleBackColor = false;
            this.cmdCreateCutSheet.Click += new System.EventHandler(this.cmdCreateCutSheet_Click);
            // 
            // fraSelectPnts
            // 
            this.fraSelectPnts.BackColor = System.Drawing.SystemColors.Control;
            this.fraSelectPnts.Controls.Add(this.cmdOnScreen);
            this.fraSelectPnts.Controls.Add(this.cmdByRange);
            this.fraSelectPnts.Controls.Add(this.cmdByAlignment);
            this.fraSelectPnts.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fraSelectPnts.ForeColor = System.Drawing.SystemColors.WindowText;
            this.fraSelectPnts.Location = new System.Drawing.Point(12, 12);
            this.fraSelectPnts.Name = "fraSelectPnts";
            this.fraSelectPnts.Padding = new System.Windows.Forms.Padding(0);
            this.fraSelectPnts.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.fraSelectPnts.Size = new System.Drawing.Size(712, 56);
            this.fraSelectPnts.TabIndex = 5;
            this.fraSelectPnts.TabStop = false;
            this.fraSelectPnts.Text = "POINT SELECTION MODE:";
            // 
            // cmdOnScreen
            // 
            this.cmdOnScreen.BackColor = System.Drawing.SystemColors.Control;
            this.cmdOnScreen.Cursor = System.Windows.Forms.Cursors.Default;
            this.cmdOnScreen.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdOnScreen.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cmdOnScreen.Location = new System.Drawing.Point(10, 16);
            this.cmdOnScreen.Name = "cmdOnScreen";
            this.cmdOnScreen.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.cmdOnScreen.Size = new System.Drawing.Size(174, 32);
            this.cmdOnScreen.TabIndex = 0;
            this.cmdOnScreen.Text = "ON SCREEN";
            this.cmdOnScreen.UseVisualStyleBackColor = false;
            this.cmdOnScreen.Click += new System.EventHandler(this.cmdOnScreen_Click);
            // 
            // cmdByRange
            // 
            this.cmdByRange.BackColor = System.Drawing.SystemColors.Control;
            this.cmdByRange.Cursor = System.Windows.Forms.Cursors.Default;
            this.cmdByRange.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdByRange.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cmdByRange.Location = new System.Drawing.Point(266, 16);
            this.cmdByRange.Name = "cmdByRange";
            this.cmdByRange.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.cmdByRange.Size = new System.Drawing.Size(174, 32);
            this.cmdByRange.TabIndex = 1;
            this.cmdByRange.Text = "BY RANGE";
            this.cmdByRange.UseVisualStyleBackColor = false;
            this.cmdByRange.Click += new System.EventHandler(this.cmdByRange_Click);
            // 
            // cmdByAlignment
            // 
            this.cmdByAlignment.BackColor = System.Drawing.SystemColors.Control;
            this.cmdByAlignment.Cursor = System.Windows.Forms.Cursors.Default;
            this.cmdByAlignment.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdByAlignment.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cmdByAlignment.Location = new System.Drawing.Point(530, 16);
            this.cmdByAlignment.Name = "cmdByAlignment";
            this.cmdByAlignment.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.cmdByAlignment.Size = new System.Drawing.Size(174, 32);
            this.cmdByAlignment.TabIndex = 2;
            this.cmdByAlignment.Text = "BY ALIGNMENT";
            this.cmdByAlignment.UseVisualStyleBackColor = false;
            this.cmdByAlignment.Click += new System.EventHandler(this.cmdByAlignment_Click);
            // 
            // fraHeadings
            // 
            this.fraHeadings.BackColor = System.Drawing.SystemColors.Control;
            this.fraHeadings.Controls.Add(this.cmdLayer);
            this.fraHeadings.Controls.Add(this.cmdObject);
            this.fraHeadings.Controls.Add(this.lblPartialRange);
            this.fraHeadings.Controls.Add(this.lblNumPoints);
            this.fraHeadings.Controls.Add(this.cmdRange);
            this.fraHeadings.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fraHeadings.ForeColor = System.Drawing.SystemColors.ControlText;
            this.fraHeadings.Location = new System.Drawing.Point(12, 76);
            this.fraHeadings.Name = "fraHeadings";
            this.fraHeadings.Padding = new System.Windows.Forms.Padding(0);
            this.fraHeadings.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.fraHeadings.Size = new System.Drawing.Size(712, 56);
            this.fraHeadings.TabIndex = 6;
            this.fraHeadings.TabStop = false;
            // 
            // cmdLayer
            // 
            this.cmdLayer.BackColor = System.Drawing.SystemColors.Control;
            this.cmdLayer.Cursor = System.Windows.Forms.Cursors.Default;
            this.cmdLayer.Font = new System.Drawing.Font("Tahoma", 9.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdLayer.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cmdLayer.Location = new System.Drawing.Point(18, 16);
            this.cmdLayer.Name = "cmdLayer";
            this.cmdLayer.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.cmdLayer.Size = new System.Drawing.Size(107, 32);
            this.cmdLayer.TabIndex = 0;
            this.cmdLayer.Text = "LAYER NAME";
            this.cmdLayer.UseVisualStyleBackColor = false;
            this.cmdLayer.Click += new System.EventHandler(this.cmdLayer_Click);
            // 
            // cmdObject
            // 
            this.cmdObject.BackColor = System.Drawing.SystemColors.Control;
            this.cmdObject.Cursor = System.Windows.Forms.Cursors.Default;
            this.cmdObject.Font = new System.Drawing.Font("Tahoma", 9.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdObject.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cmdObject.Location = new System.Drawing.Point(154, 16);
            this.cmdObject.Name = "cmdObject";
            this.cmdObject.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.cmdObject.Size = new System.Drawing.Size(107, 32);
            this.cmdObject.TabIndex = 1;
            this.cmdObject.Text = "OBJECT";
            this.cmdObject.UseVisualStyleBackColor = false;
            this.cmdObject.Click += new System.EventHandler(this.cmdObject_Click);
            // 
            // lblPartialRange
            // 
            this.lblPartialRange.BackColor = System.Drawing.SystemColors.Control;
            this.lblPartialRange.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblPartialRange.Font = new System.Drawing.Font("Tahoma", 9.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPartialRange.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblPartialRange.Location = new System.Drawing.Point(538, 24);
            this.lblPartialRange.Name = "lblPartialRange";
            this.lblPartialRange.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblPartialRange.Size = new System.Drawing.Size(134, 24);
            this.lblPartialRange.TabIndex = 2;
            this.lblPartialRange.Text = "PARTIAL RANGE";
            this.lblPartialRange.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // lblNumPoints
            // 
            this.lblNumPoints.BackColor = System.Drawing.SystemColors.Control;
            this.lblNumPoints.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblNumPoints.Font = new System.Drawing.Font("Tahoma", 9.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNumPoints.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblNumPoints.Location = new System.Drawing.Point(269, 24);
            this.lblNumPoints.Name = "lblNumPoints";
            this.lblNumPoints.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblNumPoints.Size = new System.Drawing.Size(80, 24);
            this.lblNumPoints.TabIndex = 3;
            this.lblNumPoints.Text = "# POINTS";
            this.lblNumPoints.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // cmdRange
            // 
            this.cmdRange.BackColor = System.Drawing.SystemColors.Control;
            this.cmdRange.Cursor = System.Windows.Forms.Cursors.Default;
            this.cmdRange.Font = new System.Drawing.Font("Tahoma", 9.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdRange.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cmdRange.Location = new System.Drawing.Point(354, 16);
            this.cmdRange.Name = "cmdRange";
            this.cmdRange.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.cmdRange.Size = new System.Drawing.Size(107, 32);
            this.cmdRange.TabIndex = 4;
            this.cmdRange.Text = "POINT RANGE";
            this.cmdRange.UseVisualStyleBackColor = false;
            this.cmdRange.Click += new System.EventHandler(this.cmdRange_Click);
            // 
            // fraPntDesc
            // 
            this.fraPntDesc.BackColor = System.Drawing.SystemColors.Control;
            this.fraPntDesc.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fraPntDesc.ForeColor = System.Drawing.SystemColors.ControlText;
            this.fraPntDesc.Location = new System.Drawing.Point(12, 140);
            this.fraPntDesc.Name = "fraPntDesc";
            this.fraPntDesc.Padding = new System.Windows.Forms.Padding(0);
            this.fraPntDesc.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.fraPntDesc.Size = new System.Drawing.Size(712, 296);
            this.fraPntDesc.TabIndex = 7;
            this.fraPntDesc.TabStop = false;
            // 
            // frmExport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(732, 492);
            this.Controls.Add(this.cmdCreateCutSheet);
            this.Controls.Add(this.fraSelectPnts);
            this.Controls.Add(this.fraHeadings);
            this.Controls.Add(this.fraPntDesc);
            this.Name = "frmExport";
            this.Text = "Export";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmExport_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmExport_FormClosed);
            this.Load += new System.EventHandler(this.frmExport_Load);
            this.fraSelectPnts.ResumeLayout(false);
            this.fraHeadings.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.Button cmdCreateCutSheet;
        public System.Windows.Forms.GroupBox fraSelectPnts;
        public System.Windows.Forms.Button cmdOnScreen;
        public System.Windows.Forms.Button cmdByRange;
        public System.Windows.Forms.Button cmdByAlignment;
        public System.Windows.Forms.GroupBox fraHeadings;
        public System.Windows.Forms.Button cmdLayer;
        public System.Windows.Forms.Button cmdObject;
        public System.Windows.Forms.Label lblPartialRange;
        public System.Windows.Forms.Label lblNumPoints;
        public System.Windows.Forms.Button cmdRange;
        public System.Windows.Forms.GroupBox fraPntDesc;
    }
}