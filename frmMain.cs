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
                "\n -Fix drap&drop multi files bug that not allow to add all of them.",
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
                                string[] files = Directory.GetFiles(path[i].ToString(), "*.flv", SearchOption.TopDirectoryOnly);
                                foreach (string s in files)
                                {
                                    ListViewItem input_file = new ListViewItem(s);
                                    lvInput.Items.Add(input_file);
                                    input_file.Checked = true;
                                }
                            
                        }
                        else if (file.Extension.Equals(".flv"))
                        {

                                ListViewItem input_file = new ListViewItem(path[i].ToString());
                                lvInput.Items.Add(input_file);
                                input_file.Checked = true;

                        }
                    }// for
                } // if
                
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "Error");
            }
			
		}

		private void frmMain_Load(object sender, EventArgs e) {
			LoadSettings();
            mp4box_path = Application.StartupPath.ToString() + "\\MP4Box.exe";
		}

		private void frmMain_FormClosed(object sender, FormClosedEventArgs e) {
			SaveSettings();
		}

        private void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                paths.Clear();
                foreach (ListViewItem checkedItem in lvInput.CheckedItems)
                {
                    // All these ListViewItems are checked, do something...
                    paths.Add(checkedItem.SubItems[0].Text);
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
	}
}
