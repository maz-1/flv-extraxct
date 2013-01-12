using System;
using System.Threading;
using System.Windows.Forms;
using System.Collections.Generic;
using System.IO;

namespace JDP {
	public partial class frmMain : Form {
		Thread _statusThread;
        static List<string> paths = new List<string>();
        public static string mp4box_path;
        public static string mkvmerge_path;
        public static string _fps;
        public static string _ratio;
        public static string _mode;
        public static bool _remove;
        public static bool _audio_muxing;
        

		public frmMain() {
			InitializeComponent();
			Program.SetFontAndScaling(this);
		}

		private void LoadSettings() {
			SettingsReader sr = new SettingsReader("FLV Extract", "settings.txt");
			string val;

			if ((val = sr.Load("ExtractVideo")) != null) {
				chkVideo.Checked = (val != "0");
			}
			if ((val = sr.Load("ExtractTimeCodes")) != null) {
				chkTimeCodes.Checked = (val != "0");
			}
			if ((val = sr.Load("ExtractAudio")) != null) {
				chkAudio.Checked = (val != "0");
			}
		}

		private void SaveSettings() {
			SettingsWriter sw = new SettingsWriter("FLV Extract", "settings.txt");

			sw.Save("ExtractVideo", chkVideo.Checked ? "1" : "0");
			sw.Save("ExtractTimeCodes", chkTimeCodes.Checked ? "1" : "0");
			sw.Save("ExtractAudio", chkAudio.Checked ? "1" : "0");

			sw.Close();
		}

		private void btnAbout_Click(object sender, EventArgs e) {
			MessageBox.Show(
                String.Format("FLV Extract v{1}{0} Copyright 2006-2012 J.D. Purcell{0}" +
				"http://www.moitah.net/" +

                "\n\n Add features and update by jofori89"+
                "\njofori89@gmail.com"+
                "\n 09 - June - 2012" +

                "\n\n Change log:" +

                "\n\n v1.7.0: " +
                "\n -Add list file with checkbox." +
                "\n -Change a bit on GUI." +

                "\n\n v1.7.1: " +
                "\n -Allow people drap and drop folders, files and both in the same time into files list." +
                "\n -Fix drap&drop multi files bug that not allow to add all of them." +

                "\n\n v2.0.1: " +
                "\n -Add immediately remuxing to MP4/ MKV file feature." +
                "\n -Fix some glitches." +

                "\n\n v2.1.0.2: " +
                "\n -Fix incorrect framerate for mp4 output file at auto option." +
                "\n -Add right click menu, now user can add files via it or \"Open with...\" in Windows Explorer." 
                ,
                 Environment.NewLine, General.Version),"About", MessageBoxButtons.OK, MessageBoxIcon.Information);

		}

		private void frmMain_DragEnter(object sender, DragEventArgs e) {
			if ((_statusThread != null) && _statusThread.IsAlive) return;

			if (e.Data.GetDataPresent(DataFormats.FileDrop)) {
				e.Effect = DragDropEffects.Copy;
			}
		}

		private void frmMain_DragDrop(object sender, DragEventArgs e) {
            try
            {
                if ((_statusThread != null) && _statusThread.IsAlive) return;

                var file_drops = e.Data.GetData(DataFormats.FileDrop);

                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    
                        string[] path = (string[])e.Data.GetData(DataFormats.FileDrop);

                        for (int i = 0; i < path.Length; i++)
                        {
                            FileInfo file = new FileInfo(path[i]);

                            if (file.Extension.Equals(""))
                            {
                                string[] files = Directory.GetFiles(path[i], "*.flv", SearchOption.TopDirectoryOnly);
                                foreach (string s in files)
                                {
                                
                    
                                    ListViewItem input_file = new ListViewItem();
                                    input_file.SubItems.Add(s);
                                    input_file.SubItems.Add(((new FileInfo(s).Length) / 1024 / 1024).ToString() + " MB");
                                    
                                    lvInput.Items.Add(input_file);
                                    input_file.Checked = true;
                                }

                            }
                            else if (file.Extension.Equals(".flv", StringComparison.CurrentCultureIgnoreCase))
                            {

                                ListViewItem input_file = new ListViewItem();                                
                                input_file.SubItems.Add(path[i]);
                                input_file.SubItems.Add(((new FileInfo(path[i]).Length) / 1024 / 1024).ToString() + " MB");
                                
                                lvInput.Items.Add(input_file);
                                input_file.Checked = true;

                            }
                        }// for
                    } // if
                
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message + "\n" + ex.StackTrace + "\n" + ex.HelpLink, "Error " + ex.Source);
            }
			
		}

        private void frmMain_Load(object sender, EventArgs e)
        {
            try
            {

                LoadSettings();                
                cbRatio.SelectedIndex = 0;
                cbFps.SelectedIndex = 0;
                //ContextMenuStrip mn = new ContextMenuStrip();

                if (Environment.GetCommandLineArgs().Length > 1)
                {
                    string f = Environment.GetCommandLineArgs()[1];

                    FileInfo file = new FileInfo(f);

                    if (file.Extension.Equals(""))
                    {
                        string[] files = Directory.GetFiles(f, "*.flv", SearchOption.TopDirectoryOnly);
                        foreach (string s in files)
                        {
                            ListViewItem input_file = new ListViewItem();
                            input_file.SubItems.Add(s);
                            input_file.SubItems.Add(((new FileInfo(s).Length) / 1024 / 1024).ToString() + " MB");
                            
                            lvInput.Items.Add(input_file);
                            input_file.Checked = true;
                        }

                    }
                    else if (file.Extension.Equals(".flv", StringComparison.CurrentCultureIgnoreCase))
                    {
                        ListViewItem input_file = new ListViewItem();
                        input_file.SubItems.Add(f.ToString());
                        input_file.SubItems.Add(((new FileInfo(f).Length) / 1024 / 1024).ToString() + " MB");
                        
                        lvInput.Items.Add(input_file);
                        input_file.Checked = true;

                    }
                }

                toolTip1.SetToolTip(grpRemux, "Select output type to remux file, no recoding file will be processed.");
                toolTip1.SetToolTip(grpExtract, "Check which parts you want to keep in output files, timecodes can be uncheck.");
                toolTip1.SetToolTip(this, "Drag & drop FLV file(s) or folder here to add. \nRight click for menu.");

                toolTip1.SetToolTip(cbRatio, "Changing ratio of video should be currently used for Mkv output files only.");
                toolTip1.SetToolTip(lvInput, "Uncheck to skip file(s)");
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

		}

		private void frmMain_FormClosed(object sender, FormClosedEventArgs e) {
			SaveSettings();
		}

        private void btnStart_Click(object sender, EventArgs e)
        {
            _fps = cbFps.Text;
            _ratio = cbRatio.Text;
            _remove = chkRemove.Checked ? true : false;

            try
            {
                if (rbtFLV.Checked)
                {
                    _mode = "FLV";
                }
                else if (rbtMp4.Checked)
                {
                    _mode = "MP4";
                    mp4box_path = Application.StartupPath.ToString() + "\\MP4Box.exe";
                    if (!File.Exists(mp4box_path))
                    {
                        MessageBox.Show(mp4box_path + " is not found, copy it to the same folder of FLVExtract, please.", "Error");
                        return;
                    }
                    else if (!File.Exists(Application.StartupPath.ToString() + "\\js32.dll"))
                    {
                        MessageBox.Show("js32.dll is missing, this might cause some problem for MP4 muxing process.", "Warrning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else if (rbtMkv.Checked)
                {
                    _mode = "MKV";
                    _ratio = cbRatio.Text;
                    mkvmerge_path = Application.StartupPath.ToString() + "\\mkvmerge.exe";
                    if (!File.Exists(mkvmerge_path))
                    {
                        MessageBox.Show(mkvmerge_path + " is not found, copy it to the same folder of FLVExtract, please.", "Error");
                        return;
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
            
            try
            {
                if (lvInput.CheckedItems.Count > 0)
                {
                    paths.Clear();
                    foreach (ListViewItem checkedItem in lvInput.CheckedItems)
                    {
                        // All these ListViewItems are checked, do something...
                        paths.Add(checkedItem.SubItems[1].Text);
                    }

                    string[] input_name = paths.ToArray();

                    frmStatus statusForm = new frmStatus(input_name,
                            chkVideo.Checked, chkAudio.Checked, chkTimeCodes.Checked);
                    _statusThread = new Thread((ThreadStart)delegate()
                    {
                        Invoke((MethodInvoker)delegate()
                        {
                            bool topMost = TopMost;
                            TopMost = false;
                            statusForm.ShowDialog();
                            TopMost = topMost;
                        });
                    });
                    _statusThread.Start();
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "Error");
            }

        }

        void lvInput_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            try
            {
                lvInput.Items.Clear();
                paths.Clear();
            }
            catch (Exception ex)
            {
                
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void chkAudio_Muxing_CheckedChanged(object sender, EventArgs e)
        {
            if (chkAudio_Muxing.Checked)
            {
                chkVideo.Checked = false;
                _audio_muxing = true;
            }

            else if (!chkAudio_Muxing.Checked)
            {
                chkVideo.Checked = true;
                _audio_muxing = false;
            }
        }

        private void chkVideo_CheckedChanged(object sender, EventArgs e)
        {
            if (chkVideo.Checked)
            {
                chkAudio_Muxing.Checked = false;
            }

            else if (!chkVideo.Checked)
            {
                chkAudio_Muxing.Checked = true;
            }
        }

        private void chkOnTop_CheckedChanged(object sender, EventArgs e)
        {
            if (chkOnTop.Checked)
            {
                this.TopMost = true;
            }
            else
                this.TopMost = false;
        }

        private void lvInput_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                if (e.Button == MouseButtons.Right)
                {
                    //var item = lvInput.IndexFromPoint(e.Location);

                    contextMenuStrip1.Show(lvInput, e.Location);
                }
                
            }
        }

        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog of = new OpenFileDialog())
                {
                    of.Multiselect = true;
                    of.ShowReadOnly = true;

                    if (of.ShowDialog() == DialogResult.OK)
                    {
                        string[] path = (string[])of.FileNames;

                        for (int i = 0; i < path.Length; i++)
                        {
                            FileInfo file = new FileInfo(path[i]);

                            if (file.Extension.Equals(""))
                            {
                                string[] files = Directory.GetFiles(path[i].ToString(), "*.flv", SearchOption.TopDirectoryOnly);
                                foreach (string s in files)
                                {
                                    ListViewItem input_file = new ListViewItem();
                                    input_file.SubItems.Add(s);
                                    input_file.SubItems.Add(((new FileInfo(s).Length) / 1024 / 1024).ToString() + " MB");
                                    lvInput.Items.Add(input_file);
                                    input_file.Checked = true;
                                }

                            }
                            else if (file.Extension.Equals(".flv", StringComparison.CurrentCultureIgnoreCase))
                            {
                                ListViewItem input_file = new ListViewItem();
                                input_file.SubItems.Add(path[i].ToString());
                                input_file.SubItems.Add(((new FileInfo(path[i]).Length) / 1024 / 1024).ToString() + " MB");
                                lvInput.Items.Add(input_file);
                                input_file.Checked = true;

                            }
                        }// for
                    } // if

                };
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void clearAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btnClear_Click(sender, e);
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //lvInput.SelectedItems.
            if (lvInput.SelectedItems.Count > 0)
            {
                foreach (ListViewItem checkedItem in lvInput.SelectedItems)
                {
                    // All these ListViewItems are Selected, do something...
                    checkedItem.Remove();
                }
                
            }
        }

        private void skipTheseFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lvInput.SelectedItems.Count > 0)
            {
                foreach (ListViewItem checkedItem in lvInput.SelectedItems)
                {
                    // All these ListViewItems are Selected, do something...
                    checkedItem.Checked = false;
                }

            }
        }
	}
}
