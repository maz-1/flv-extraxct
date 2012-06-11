namespace JDP {
	partial class frmStatus {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmStatus));
            this.lvStatus = new System.Windows.Forms.ListView();
            this.chStatus = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chFile = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chTrueFrameRate = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chAverageFrameRate = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chDetails = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCopyFrameRates = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.txtStatus = new System.Windows.Forms.Label();
            this.cbxCommand = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // lvStatus
            // 
            this.lvStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvStatus.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chStatus,
            this.chFile,
            this.chTrueFrameRate,
            this.chAverageFrameRate,
            this.chDetails});
            this.lvStatus.FullRowSelect = true;
            this.lvStatus.HideSelection = false;
            this.lvStatus.Location = new System.Drawing.Point(8, 8);
            this.lvStatus.Name = "lvStatus";
            this.lvStatus.Size = new System.Drawing.Size(592, 286);
            this.lvStatus.TabIndex = 0;
            this.lvStatus.UseCompatibleStateImageBehavior = false;
            this.lvStatus.View = System.Windows.Forms.View.Details;
            this.lvStatus.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lvStatus_MouseDoubleClick);
            // 
            // chStatus
            // 
            this.chStatus.Text = "";
            this.chStatus.Width = 24;
            // 
            // chFile
            // 
            this.chFile.Text = "File";
            this.chFile.Width = 220;
            // 
            // chTrueFrameRate
            // 
            this.chTrueFrameRate.Text = "True Frame Rate";
            this.chTrueFrameRate.Width = 104;
            // 
            // chAverageFrameRate
            // 
            this.chAverageFrameRate.Text = "Avg Frame Rate";
            this.chAverageFrameRate.Width = 104;
            // 
            // chDetails
            // 
            this.chDetails.Text = "Warning/Error";
            this.chDetails.Width = 560;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Enabled = false;
            this.btnOK.Location = new System.Drawing.Point(540, 302);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(60, 23);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "&OK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // btnCopyFrameRates
            // 
            this.btnCopyFrameRates.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCopyFrameRates.Enabled = false;
            this.btnCopyFrameRates.Location = new System.Drawing.Point(362, 300);
            this.btnCopyFrameRates.Name = "btnCopyFrameRates";
            this.btnCopyFrameRates.Size = new System.Drawing.Size(104, 23);
            this.btnCopyFrameRates.TabIndex = 3;
            this.btnCopyFrameRates.Text = "&Copy Frame Rates";
            this.btnCopyFrameRates.UseVisualStyleBackColor = true;
            this.btnCopyFrameRates.Click += new System.EventHandler(this.btnCopyFrameRates_Click);
            // 
            // btnStop
            // 
            this.btnStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStop.Location = new System.Drawing.Point(472, 302);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(60, 23);
            this.btnStop.TabIndex = 2;
            this.btnStop.Text = "&Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // txtStatus
            // 
            this.txtStatus.AutoSize = true;
            this.txtStatus.Location = new System.Drawing.Point(5, 312);
            this.txtStatus.Name = "txtStatus";
            this.txtStatus.Size = new System.Drawing.Size(43, 13);
            this.txtStatus.TabIndex = 4;
            this.txtStatus.Text = "Waiting";
            // 
            // cbxCommand
            // 
            this.cbxCommand.AutoSize = true;
            this.cbxCommand.Location = new System.Drawing.Point(216, 306);
            this.cbxCommand.Name = "cbxCommand";
            this.cbxCommand.Size = new System.Drawing.Size(140, 17);
            this.cbxCommand.TabIndex = 5;
            this.cbxCommand.Text = "Show command window";
            this.cbxCommand.UseVisualStyleBackColor = true;
            // 
            // frmStatus
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(608, 333);
            this.Controls.Add(this.cbxCommand);
            this.Controls.Add(this.txtStatus);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnCopyFrameRates);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.lvStatus);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(280, 180);
            this.Name = "frmStatus";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Status";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmStatus_FormClosing);
            this.Shown += new System.EventHandler(this.frmStatus_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ListView lvStatus;
		private System.Windows.Forms.ColumnHeader chFile;
		private System.Windows.Forms.ColumnHeader chStatus;
		private System.Windows.Forms.ColumnHeader chTrueFrameRate;
		private System.Windows.Forms.ColumnHeader chAverageFrameRate;
		private System.Windows.Forms.ColumnHeader chDetails;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCopyFrameRates;
		private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Label txtStatus;
        private System.Windows.Forms.CheckBox cbxCommand;
	}
}