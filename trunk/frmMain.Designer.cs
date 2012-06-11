namespace JDP {
	partial class frmMain {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.btnAbout = new System.Windows.Forms.Button();
            this.lblInstructions = new System.Windows.Forms.Label();
            this.grpExtract = new System.Windows.Forms.GroupBox();
            this.chkAudio = new System.Windows.Forms.CheckBox();
            this.chkTimeCodes = new System.Windows.Forms.CheckBox();
            this.chkVideo = new System.Windows.Forms.CheckBox();
            this.lvInput = new System.Windows.Forms.ListView();
            this.chFilename = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnStart = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.grpRemux = new System.Windows.Forms.GroupBox();
            this.cbxRemove = new System.Windows.Forms.CheckBox();
            this.cbRatio = new System.Windows.Forms.ComboBox();
            this.lblRatio = new System.Windows.Forms.Label();
            this.lblFramerate = new System.Windows.Forms.Label();
            this.cbFps = new System.Windows.Forms.ComboBox();
            this.rbtMkv = new System.Windows.Forms.RadioButton();
            this.rbtMp4 = new System.Windows.Forms.RadioButton();
            this.rbtFLV = new System.Windows.Forms.RadioButton();
            this.grpExtract.SuspendLayout();
            this.grpRemux.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnAbout
            // 
            this.btnAbout.Location = new System.Drawing.Point(195, 150);
            this.btnAbout.Name = "btnAbout";
            this.btnAbout.Size = new System.Drawing.Size(63, 24);
            this.btnAbout.TabIndex = 2;
            this.btnAbout.Text = "A&bout";
            this.btnAbout.UseVisualStyleBackColor = true;
            this.btnAbout.Click += new System.EventHandler(this.btnAbout_Click);
            // 
            // lblInstructions
            // 
            this.lblInstructions.Location = new System.Drawing.Point(9, 13);
            this.lblInstructions.Name = "lblInstructions";
            this.lblInstructions.Size = new System.Drawing.Size(168, 56);
            this.lblInstructions.TabIndex = 0;
            this.lblInstructions.Text = "Drop folders or files here.\r\n\r\nOutput files are written in the same folder as the" +
    " FLVs.";
            this.lblInstructions.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // grpExtract
            // 
            this.grpExtract.Controls.Add(this.chkAudio);
            this.grpExtract.Controls.Add(this.chkTimeCodes);
            this.grpExtract.Controls.Add(this.chkVideo);
            this.grpExtract.Location = new System.Drawing.Point(183, 12);
            this.grpExtract.Name = "grpExtract";
            this.grpExtract.Size = new System.Drawing.Size(96, 84);
            this.grpExtract.TabIndex = 1;
            this.grpExtract.TabStop = false;
            this.grpExtract.Text = "Extract:";
            // 
            // chkAudio
            // 
            this.chkAudio.Checked = true;
            this.chkAudio.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAudio.Location = new System.Drawing.Point(12, 60);
            this.chkAudio.Name = "chkAudio";
            this.chkAudio.Size = new System.Drawing.Size(80, 17);
            this.chkAudio.TabIndex = 2;
            this.chkAudio.Text = "&Audio";
            this.chkAudio.UseVisualStyleBackColor = true;
            // 
            // chkTimeCodes
            // 
            this.chkTimeCodes.Checked = true;
            this.chkTimeCodes.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkTimeCodes.Location = new System.Drawing.Point(12, 40);
            this.chkTimeCodes.Name = "chkTimeCodes";
            this.chkTimeCodes.Size = new System.Drawing.Size(80, 17);
            this.chkTimeCodes.TabIndex = 1;
            this.chkTimeCodes.Text = "&Timecodes";
            this.chkTimeCodes.UseVisualStyleBackColor = true;
            // 
            // chkVideo
            // 
            this.chkVideo.Checked = true;
            this.chkVideo.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkVideo.Location = new System.Drawing.Point(12, 20);
            this.chkVideo.Name = "chkVideo";
            this.chkVideo.Size = new System.Drawing.Size(80, 17);
            this.chkVideo.TabIndex = 0;
            this.chkVideo.Text = "&Video";
            this.chkVideo.UseVisualStyleBackColor = true;
            // 
            // lvInput
            // 
            this.lvInput.CheckBoxes = true;
            this.lvInput.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chFilename});
            this.lvInput.Location = new System.Drawing.Point(11, 217);
            this.lvInput.Name = "lvInput";
            this.lvInput.Size = new System.Drawing.Size(571, 364);
            this.lvInput.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.lvInput.TabIndex = 3;
            this.lvInput.UseCompatibleStateImageBehavior = false;
            this.lvInput.View = System.Windows.Forms.View.Details;
            // 
            // chFilename
            // 
            this.chFilename.Text = "Filename";
            this.chFilename.Width = 400;
            // 
            // btnStart
            // 
            this.btnStart.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnStart.ForeColor = System.Drawing.Color.Green;
            this.btnStart.Location = new System.Drawing.Point(113, 150);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(63, 23);
            this.btnStart.TabIndex = 4;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnClear
            // 
            this.btnClear.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnClear.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.btnClear.Location = new System.Drawing.Point(12, 150);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 23);
            this.btnClear.TabIndex = 5;
            this.btnClear.Text = "Clear all";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // grpRemux
            // 
            this.grpRemux.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpRemux.Controls.Add(this.cbxRemove);
            this.grpRemux.Controls.Add(this.cbRatio);
            this.grpRemux.Controls.Add(this.lblRatio);
            this.grpRemux.Controls.Add(this.lblFramerate);
            this.grpRemux.Controls.Add(this.cbFps);
            this.grpRemux.Controls.Add(this.rbtMkv);
            this.grpRemux.Controls.Add(this.rbtMp4);
            this.grpRemux.Controls.Add(this.rbtFLV);
            this.grpRemux.Location = new System.Drawing.Point(302, 13);
            this.grpRemux.Name = "grpRemux";
            this.grpRemux.Size = new System.Drawing.Size(280, 161);
            this.grpRemux.TabIndex = 6;
            this.grpRemux.TabStop = false;
            this.grpRemux.Text = "Remuxing";
            // 
            // cbxRemove
            // 
            this.cbxRemove.AutoSize = true;
            this.cbxRemove.Checked = true;
            this.cbxRemove.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbxRemove.Location = new System.Drawing.Point(34, 99);
            this.cbxRemove.Name = "cbxRemove";
            this.cbxRemove.Size = new System.Drawing.Size(178, 17);
            this.cbxRemove.TabIndex = 6;
            this.cbxRemove.Text = "Remove temp files (.264, .acc,..)";
            this.cbxRemove.UseVisualStyleBackColor = true;
            // 
            // cbRatio
            // 
            this.cbRatio.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cbRatio.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbRatio.FormattingEnabled = true;
            this.cbRatio.Items.AddRange(new object[] {
            "Original",
            "4:3",
            "16:9",
            "5:4",
            "12:5"});
            this.cbRatio.Location = new System.Drawing.Point(182, 61);
            this.cbRatio.Name = "cbRatio";
            this.cbRatio.Size = new System.Drawing.Size(92, 21);
            this.cbRatio.TabIndex = 5;
            // 
            // lblRatio
            // 
            this.lblRatio.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblRatio.AutoSize = true;
            this.lblRatio.Location = new System.Drawing.Point(113, 67);
            this.lblRatio.Name = "lblRatio";
            this.lblRatio.Size = new System.Drawing.Size(63, 13);
            this.lblRatio.TabIndex = 4;
            this.lblRatio.Text = "Aspect ratio";
            // 
            // lblFramerate
            // 
            this.lblFramerate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblFramerate.AutoSize = true;
            this.lblFramerate.Location = new System.Drawing.Point(152, 22);
            this.lblFramerate.Name = "lblFramerate";
            this.lblFramerate.Size = new System.Drawing.Size(24, 13);
            this.lblFramerate.TabIndex = 4;
            this.lblFramerate.Text = "Fps";
            // 
            // cbFps
            // 
            this.cbFps.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cbFps.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbFps.FormattingEnabled = true;
            this.cbFps.Items.AddRange(new object[] {
            "Original",
            "23.976",
            "12.5",
            "24",
            "25",
            "29.976",
            "30"});
            this.cbFps.Location = new System.Drawing.Point(182, 19);
            this.cbFps.Name = "cbFps";
            this.cbFps.Size = new System.Drawing.Size(92, 21);
            this.cbFps.TabIndex = 3;
            // 
            // rbtMkv
            // 
            this.rbtMkv.AutoSize = true;
            this.rbtMkv.Location = new System.Drawing.Point(34, 65);
            this.rbtMkv.Name = "rbtMkv";
            this.rbtMkv.Size = new System.Drawing.Size(63, 17);
            this.rbtMkv.TabIndex = 2;
            this.rbtMkv.Text = "To MKV";
            this.rbtMkv.UseVisualStyleBackColor = true;
            // 
            // rbtMp4
            // 
            this.rbtMp4.AutoSize = true;
            this.rbtMp4.Checked = true;
            this.rbtMp4.Location = new System.Drawing.Point(34, 43);
            this.rbtMp4.Name = "rbtMp4";
            this.rbtMp4.Size = new System.Drawing.Size(62, 17);
            this.rbtMp4.TabIndex = 1;
            this.rbtMp4.TabStop = true;
            this.rbtMp4.Text = "To MP4";
            this.rbtMp4.UseVisualStyleBackColor = true;
            // 
            // rbtFLV
            // 
            this.rbtFLV.AutoSize = true;
            this.rbtFLV.Location = new System.Drawing.Point(34, 20);
            this.rbtFLV.Name = "rbtFLV";
            this.rbtFLV.Size = new System.Drawing.Size(69, 17);
            this.rbtFLV.TabIndex = 0;
            this.rbtFLV.Text = "No remux";
            this.rbtFLV.UseVisualStyleBackColor = true;
            // 
            // frmMain
            // 
            this.AcceptButton = this.btnStart;
            this.AllowDrop = true;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(594, 593);
            this.Controls.Add(this.grpRemux);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.lvInput);
            this.Controls.Add(this.grpExtract);
            this.Controls.Add(this.lblInstructions);
            this.Controls.Add(this.btnAbout);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FLV Extract v2.0";
            this.TopMost = true;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmMain_FormClosed);
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.frmMain_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.frmMain_DragEnter);
            this.grpExtract.ResumeLayout(false);
            this.grpRemux.ResumeLayout(false);
            this.grpRemux.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button btnAbout;
		private System.Windows.Forms.Label lblInstructions;
		private System.Windows.Forms.GroupBox grpExtract;
		private System.Windows.Forms.CheckBox chkAudio;
		private System.Windows.Forms.CheckBox chkTimeCodes;
        private System.Windows.Forms.CheckBox chkVideo;
        private System.Windows.Forms.ListView lvInput;
        private System.Windows.Forms.ColumnHeader chFilename;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Label lblFramerate;
        private System.Windows.Forms.Label lblRatio;
        private System.Windows.Forms.ComboBox cbRatio;
        private System.Windows.Forms.RadioButton rbtMkv;
        private System.Windows.Forms.RadioButton rbtMp4;
        private System.Windows.Forms.RadioButton rbtFLV;
        private System.Windows.Forms.ComboBox cbFps;
        private System.Windows.Forms.GroupBox grpRemux;
        private System.Windows.Forms.CheckBox cbxRemove;
	}
}