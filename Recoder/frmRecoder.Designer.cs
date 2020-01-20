namespace Recoder
{
    partial class frmRecoder
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmRecoder));
            this.txtSourceFolder = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lblTarget = new System.Windows.Forms.Label();
            this.txtTargetFolder = new System.Windows.Forms.TextBox();
            this.cmdBrowseSource = new System.Windows.Forms.Button();
            this.cmdBrowseTarget = new System.Windows.Forms.Button();
            this.grpReEncode = new System.Windows.Forms.GroupBox();
            this.lblStatistics = new System.Windows.Forms.Label();
            this.lblEntireTask = new System.Windows.Forms.Label();
            this.lblCurrentFile = new System.Windows.Forms.Label();
            this.pBarTask = new System.Windows.Forms.ProgressBar();
            this.pBarCurrentFile = new System.Windows.Forms.ProgressBar();
            this.cmdBegin = new System.Windows.Forms.Button();
            this.cboBitRate = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtSourceExtensions = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.cboTargetType = new System.Windows.Forms.ComboBox();
            this.grpReEncode.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtSourceFolder
            // 
            this.txtSourceFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSourceFolder.Location = new System.Drawing.Point(74, 14);
            this.txtSourceFolder.Margin = new System.Windows.Forms.Padding(0);
            this.txtSourceFolder.Name = "txtSourceFolder";
            this.txtSourceFolder.Size = new System.Drawing.Size(327, 22);
            this.txtSourceFolder.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 17);
            this.label1.TabIndex = 1;
            this.label1.Text = "&Source:";
            // 
            // lblTarget
            // 
            this.lblTarget.AutoSize = true;
            this.lblTarget.Location = new System.Drawing.Point(13, 45);
            this.lblTarget.Name = "lblTarget";
            this.lblTarget.Size = new System.Drawing.Size(54, 17);
            this.lblTarget.TabIndex = 3;
            this.lblTarget.Text = "&Target:";
            // 
            // txtTargetFolder
            // 
            this.txtTargetFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTargetFolder.Location = new System.Drawing.Point(74, 42);
            this.txtTargetFolder.Margin = new System.Windows.Forms.Padding(0);
            this.txtTargetFolder.Name = "txtTargetFolder";
            this.txtTargetFolder.Size = new System.Drawing.Size(327, 22);
            this.txtTargetFolder.TabIndex = 2;
            // 
            // cmdBrowseSource
            // 
            this.cmdBrowseSource.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdBrowseSource.Location = new System.Drawing.Point(408, 13);
            this.cmdBrowseSource.Margin = new System.Windows.Forms.Padding(0);
            this.cmdBrowseSource.Name = "cmdBrowseSource";
            this.cmdBrowseSource.Size = new System.Drawing.Size(75, 23);
            this.cmdBrowseSource.TabIndex = 4;
            this.cmdBrowseSource.Text = "&Browse...";
            this.cmdBrowseSource.UseVisualStyleBackColor = true;
            this.cmdBrowseSource.Click += new System.EventHandler(this.cmdBrowseSource_Click);
            // 
            // cmdBrowseTarget
            // 
            this.cmdBrowseTarget.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdBrowseTarget.Location = new System.Drawing.Point(408, 43);
            this.cmdBrowseTarget.Margin = new System.Windows.Forms.Padding(0);
            this.cmdBrowseTarget.Name = "cmdBrowseTarget";
            this.cmdBrowseTarget.Size = new System.Drawing.Size(75, 23);
            this.cmdBrowseTarget.TabIndex = 5;
            this.cmdBrowseTarget.Text = "&Browse...";
            this.cmdBrowseTarget.UseVisualStyleBackColor = true;
            this.cmdBrowseTarget.Click += new System.EventHandler(this.cmdBrowseSource_Click);
            // 
            // grpReEncode
            // 
            this.grpReEncode.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpReEncode.Controls.Add(this.lblStatistics);
            this.grpReEncode.Controls.Add(this.lblEntireTask);
            this.grpReEncode.Controls.Add(this.lblCurrentFile);
            this.grpReEncode.Controls.Add(this.pBarTask);
            this.grpReEncode.Controls.Add(this.pBarCurrentFile);
            this.grpReEncode.Location = new System.Drawing.Point(16, 191);
            this.grpReEncode.Margin = new System.Windows.Forms.Padding(0);
            this.grpReEncode.Name = "grpReEncode";
            this.grpReEncode.Padding = new System.Windows.Forms.Padding(0);
            this.grpReEncode.Size = new System.Drawing.Size(476, 247);
            this.grpReEncode.TabIndex = 6;
            this.grpReEncode.TabStop = false;
            this.grpReEncode.Text = "&Progress";
            // 
            // lblStatistics
            // 
            this.lblStatistics.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblStatistics.Location = new System.Drawing.Point(13, 30);
            this.lblStatistics.Name = "lblStatistics";
            this.lblStatistics.Size = new System.Drawing.Size(454, 84);
            this.lblStatistics.TabIndex = 4;
            // 
            // lblEntireTask
            // 
            this.lblEntireTask.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblEntireTask.AutoSize = true;
            this.lblEntireTask.Location = new System.Drawing.Point(10, 184);
            this.lblEntireTask.Name = "lblEntireTask";
            this.lblEntireTask.Size = new System.Drawing.Size(80, 17);
            this.lblEntireTask.TabIndex = 3;
            this.lblEntireTask.Text = "&Entire Task";
            // 
            // lblCurrentFile
            // 
            this.lblCurrentFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblCurrentFile.AutoSize = true;
            this.lblCurrentFile.Location = new System.Drawing.Point(10, 118);
            this.lblCurrentFile.Name = "lblCurrentFile";
            this.lblCurrentFile.Size = new System.Drawing.Size(81, 17);
            this.lblCurrentFile.TabIndex = 2;
            this.lblCurrentFile.Text = "&Current File";
            // 
            // pBarTask
            // 
            this.pBarTask.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pBarTask.Location = new System.Drawing.Point(6, 212);
            this.pBarTask.Margin = new System.Windows.Forms.Padding(0);
            this.pBarTask.Name = "pBarTask";
            this.pBarTask.Size = new System.Drawing.Size(414, 23);
            this.pBarTask.TabIndex = 1;
            // 
            // pBarCurrentFile
            // 
            this.pBarCurrentFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pBarCurrentFile.Location = new System.Drawing.Point(6, 141);
            this.pBarCurrentFile.Margin = new System.Windows.Forms.Padding(0);
            this.pBarCurrentFile.Name = "pBarCurrentFile";
            this.pBarCurrentFile.Size = new System.Drawing.Size(414, 23);
            this.pBarCurrentFile.TabIndex = 0;
            // 
            // cmdBegin
            // 
            this.cmdBegin.Location = new System.Drawing.Point(408, 145);
            this.cmdBegin.Margin = new System.Windows.Forms.Padding(0);
            this.cmdBegin.Name = "cmdBegin";
            this.cmdBegin.Size = new System.Drawing.Size(75, 39);
            this.cmdBegin.TabIndex = 7;
            this.cmdBegin.Text = "&Begin";
            this.cmdBegin.UseVisualStyleBackColor = true;
            this.cmdBegin.Click += new System.EventHandler(this.cmdBegin_Click);
            // 
            // cboBitRate
            // 
            this.cboBitRate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboBitRate.FormattingEnabled = true;
            this.cboBitRate.Location = new System.Drawing.Point(221, 79);
            this.cboBitRate.Margin = new System.Windows.Forms.Padding(0);
            this.cboBitRate.Name = "cboBitRate";
            this.cboBitRate.Size = new System.Drawing.Size(121, 24);
            this.cboBitRate.TabIndex = 8;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(162, 86);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 17);
            this.label2.TabIndex = 9;
            this.label2.Text = "Bitrate:";
            // 
            // txtSourceExtensions
            // 
            this.txtSourceExtensions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSourceExtensions.Location = new System.Drawing.Point(160, 124);
            this.txtSourceExtensions.Name = "txtSourceExtensions";
            this.txtSourceExtensions.Size = new System.Drawing.Size(194, 22);
            this.txtSourceExtensions.TabIndex = 10;
            this.txtSourceExtensions.Text = ".flac,.wma";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(25, 127);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(129, 17);
            this.label3.TabIndex = 11;
            this.label3.Text = "Source Extensions:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(27, 156);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(98, 17);
            this.label4.TabIndex = 12;
            this.label4.Text = "&Target Format";
            // 
            // cboTargetType
            // 
            this.cboTargetType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboTargetType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTargetType.FormattingEnabled = true;
            this.cboTargetType.Location = new System.Drawing.Point(159, 152);
            this.cboTargetType.Name = "cboTargetType";
            this.cboTargetType.Size = new System.Drawing.Size(195, 24);
            this.cboTargetType.TabIndex = 14;
            // 
            // frmRecoder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(497, 476);
            this.Controls.Add(this.cboTargetType);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtSourceExtensions);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cboBitRate);
            this.Controls.Add(this.cmdBegin);
            this.Controls.Add(this.grpReEncode);
            this.Controls.Add(this.cmdBrowseTarget);
            this.Controls.Add(this.cmdBrowseSource);
            this.Controls.Add(this.lblTarget);
            this.Controls.Add(this.txtTargetFolder);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtSourceFolder);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "frmRecoder";
            this.Text = "BASecamp Recoder";
            this.Load += new System.EventHandler(this.frmRecoder_Load);
            this.grpReEncode.ResumeLayout(false);
            this.grpReEncode.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtSourceFolder;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblTarget;
        private System.Windows.Forms.TextBox txtTargetFolder;
        private System.Windows.Forms.Button cmdBrowseSource;
        private System.Windows.Forms.Button cmdBrowseTarget;
        private System.Windows.Forms.GroupBox grpReEncode;
        private System.Windows.Forms.Button cmdBegin;
        private System.Windows.Forms.Label lblStatistics;
        private System.Windows.Forms.Label lblEntireTask;
        private System.Windows.Forms.Label lblCurrentFile;
        private System.Windows.Forms.ProgressBar pBarTask;
        private System.Windows.Forms.ProgressBar pBarCurrentFile;
        private System.Windows.Forms.ComboBox cboBitRate;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtSourceExtensions;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cboTargetType;
    }
}