namespace EW.Forms
{
    partial class frmBuildAreas
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
            this.cmdSelectLayers2Freeze = new System.Windows.Forms.Button();
            this.cmdBoundary = new System.Windows.Forms.Button();
            this.cmdMakePolyline = new System.Windows.Forms.Button();
            this.cmdSAP = new System.Windows.Forms.Button();
            this.cmdIdLayer = new System.Windows.Forms.Button();
            this.cmdCheckAreaLimits = new System.Windows.Forms.Button();
            this.cmdTransfer = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // cmdSelectLayers2Freeze
            // 
            this.cmdSelectLayers2Freeze.BackColor = System.Drawing.SystemColors.Control;
            this.cmdSelectLayers2Freeze.Cursor = System.Windows.Forms.Cursors.Default;
            this.cmdSelectLayers2Freeze.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdSelectLayers2Freeze.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cmdSelectLayers2Freeze.Location = new System.Drawing.Point(15, 88);
            this.cmdSelectLayers2Freeze.Name = "cmdSelectLayers2Freeze";
            this.cmdSelectLayers2Freeze.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.cmdSelectLayers2Freeze.Size = new System.Drawing.Size(168, 32);
            this.cmdSelectLayers2Freeze.TabIndex = 6;
            this.cmdSelectLayers2Freeze.Text = "FREEZE XREF LAYERS";
            this.cmdSelectLayers2Freeze.UseVisualStyleBackColor = false;
            this.cmdSelectLayers2Freeze.Click += new System.EventHandler(this.cmdSelectLayers2Freeze_Click);
            // 
            // cmdBoundary
            // 
            this.cmdBoundary.BackColor = System.Drawing.SystemColors.Control;
            this.cmdBoundary.Cursor = System.Windows.Forms.Cursors.Default;
            this.cmdBoundary.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdBoundary.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cmdBoundary.Location = new System.Drawing.Point(15, 128);
            this.cmdBoundary.Name = "cmdBoundary";
            this.cmdBoundary.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.cmdBoundary.Size = new System.Drawing.Size(168, 32);
            this.cmdBoundary.TabIndex = 7;
            this.cmdBoundary.Text = "BOUNDARY COMMAND";
            this.cmdBoundary.UseVisualStyleBackColor = false;
            this.cmdBoundary.Click += new System.EventHandler(this.cmdBoundary_Click);
            // 
            // cmdMakePolyline
            // 
            this.cmdMakePolyline.BackColor = System.Drawing.SystemColors.Control;
            this.cmdMakePolyline.Cursor = System.Windows.Forms.Cursors.Default;
            this.cmdMakePolyline.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdMakePolyline.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cmdMakePolyline.Location = new System.Drawing.Point(15, 168);
            this.cmdMakePolyline.Name = "cmdMakePolyline";
            this.cmdMakePolyline.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.cmdMakePolyline.Size = new System.Drawing.Size(168, 32);
            this.cmdMakePolyline.TabIndex = 8;
            this.cmdMakePolyline.Text = "MAKE POLYLINE";
            this.cmdMakePolyline.UseVisualStyleBackColor = false;
            this.cmdMakePolyline.Click += new System.EventHandler(this.cmdMakePolyline_Click);
            // 
            // cmdSAP
            // 
            this.cmdSAP.BackColor = System.Drawing.SystemColors.Control;
            this.cmdSAP.Cursor = System.Windows.Forms.Cursors.Default;
            this.cmdSAP.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdSAP.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cmdSAP.Location = new System.Drawing.Point(15, 208);
            this.cmdSAP.Name = "cmdSAP";
            this.cmdSAP.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.cmdSAP.Size = new System.Drawing.Size(168, 32);
            this.cmdSAP.TabIndex = 9;
            this.cmdSAP.Text = "SET AREA PROPERTIES";
            this.cmdSAP.UseVisualStyleBackColor = false;
            this.cmdSAP.Click += new System.EventHandler(this.cmdSAP_Click);
            // 
            // cmdIdLayer
            // 
            this.cmdIdLayer.BackColor = System.Drawing.SystemColors.Control;
            this.cmdIdLayer.Cursor = System.Windows.Forms.Cursors.Default;
            this.cmdIdLayer.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdIdLayer.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cmdIdLayer.Location = new System.Drawing.Point(15, 48);
            this.cmdIdLayer.Name = "cmdIdLayer";
            this.cmdIdLayer.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.cmdIdLayer.Size = new System.Drawing.Size(168, 32);
            this.cmdIdLayer.TabIndex = 10;
            this.cmdIdLayer.Text = "ID XREF LAYER";
            this.cmdIdLayer.UseVisualStyleBackColor = false;
            this.cmdIdLayer.Click += new System.EventHandler(this.cmdIdLayer_Click);
            // 
            // cmdCheckAreaLimits
            // 
            this.cmdCheckAreaLimits.BackColor = System.Drawing.SystemColors.Control;
            this.cmdCheckAreaLimits.Cursor = System.Windows.Forms.Cursors.Default;
            this.cmdCheckAreaLimits.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdCheckAreaLimits.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cmdCheckAreaLimits.Location = new System.Drawing.Point(15, 248);
            this.cmdCheckAreaLimits.Name = "cmdCheckAreaLimits";
            this.cmdCheckAreaLimits.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.cmdCheckAreaLimits.Size = new System.Drawing.Size(168, 32);
            this.cmdCheckAreaLimits.TabIndex = 11;
            this.cmdCheckAreaLimits.Text = "CHECK AREA LIMITS";
            this.cmdCheckAreaLimits.UseVisualStyleBackColor = false;
            this.cmdCheckAreaLimits.Click += new System.EventHandler(this.cmdCheckAreaLimits_Click);
            // 
            // cmdTransfer
            // 
            this.cmdTransfer.AutoEllipsis = true;
            this.cmdTransfer.BackColor = System.Drawing.SystemColors.Control;
            this.cmdTransfer.Cursor = System.Windows.Forms.Cursors.Default;
            this.cmdTransfer.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdTransfer.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cmdTransfer.Location = new System.Drawing.Point(15, 10);
            this.cmdTransfer.Name = "cmdTransfer";
            this.cmdTransfer.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.cmdTransfer.Size = new System.Drawing.Size(168, 32);
            this.cmdTransfer.TabIndex = 12;
            this.cmdTransfer.Text = "TRANSFER IN";
            this.cmdTransfer.UseVisualStyleBackColor = false;
            this.cmdTransfer.Click += new System.EventHandler(this.cmdTransfer_Click);
            // 
            // frmBuildAreas
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(194, 288);
            this.Controls.Add(this.cmdTransfer);
            this.Controls.Add(this.cmdSelectLayers2Freeze);
            this.Controls.Add(this.cmdBoundary);
            this.Controls.Add(this.cmdMakePolyline);
            this.Controls.Add(this.cmdSAP);
            this.Controls.Add(this.cmdIdLayer);
            this.Controls.Add(this.cmdCheckAreaLimits);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmBuildAreas";
            this.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.ShowInTaskbar = false;
            this.Text = "BUILD AREAS";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmBuildAreas_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.Button cmdSelectLayers2Freeze;
        public System.Windows.Forms.Button cmdBoundary;
        public System.Windows.Forms.Button cmdMakePolyline;
        public System.Windows.Forms.Button cmdSAP;
        public System.Windows.Forms.Button cmdIdLayer;
        public System.Windows.Forms.Button cmdCheckAreaLimits;
        public System.Windows.Forms.Button cmdTransfer;
    }
}