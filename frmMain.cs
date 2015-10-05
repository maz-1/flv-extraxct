using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace JDP
{
    /// <summary>
    /// 
    /// </summary>
    public partial class frmMain : Form
    {
        public static bool AudioMuxing = true;
        public static bool VideoMuxing = true;
        public static string Fps;
        public static string Mode;
        public static string Ratio;
        public static bool Remove;
        public static string MkvmergePath;
        public static string Mp4BoxPath;
        public static char Slash = System.IO.Path.DirectorySeparatorChar;
        private static readonly List<string> Paths = new List<string>();
        private Thread _statusThread;


        public frmMain()
        {
            InitializeComponent();
            Program.SetFontAndScaling(this);
        }

        /// <summary>
        /// Handles the Click event of the addToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                using (var of = new OpenFileDialog())
                {
                    of.Multiselect = true;
                    of.ShowReadOnly = true;

                    if (of.ShowDialog() != DialogResult.OK) return;
                    string[] path = of.FileNames;

                    for (var i = 0; i < path.Length; i++)
                    {
                        var file = new FileInfo(path[i]);

                        if ("".Equals(file.Extension))
                        {
                            string[] files = Directory.GetFiles(path[i], "*.flv", SearchOption.TopDirectoryOnly);
                            foreach (var s in files)
                            {
                                var inputFile = new ListViewItem();
                                inputFile.SubItems.Add(s);
                                inputFile.SubItems.Add(((new FileInfo(s).Length)/1024/1024).ToString(CultureInfo.InvariantCulture) + " MB");
                                lvInput.Items.Add(inputFile);
                                inputFile.Checked = true;
                            }
                        }
                        else if (file.Extension.Equals(".flv", StringComparison.CurrentCultureIgnoreCase))
                        {
                            var inputFile = new ListViewItem();
                            inputFile.SubItems.Add(path[i]);

                            //inputFile.SubItems.Add(res);

                            inputFile.SubItems.Add(((new FileInfo(path[i]).Length)/1024/1024).ToString(CultureInfo.InvariantCulture) + " MB");

                            //MI.Open(path[i]);
                            //MessageBox.Show(MI.Inform().ToString());

                            lvInput.Items.Add(inputFile);
                            inputFile.Checked = true;
                        }
                    } // for
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAbout_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                String.Format("FLV Extract v{1}{0} Copyright 2006-2012 J.D. Purcell{0}" +
                              "http://www.moitah.net/" +
                              "\n\n Add features and update by jofori89" +
                              "\njofori89@gmail.com" +
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
                              "\n -Add right click menu, now user can add files via it or \"Open with...\" in Windows Explorer."+
                              "\n\n v2.2.0.2: " +
                              "\n -Add only-video muxing option.",
                              Environment.NewLine, General.Version), "About", MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            try
            {
                lvInput.Items.Clear();
                Paths.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            Fps = cbFps.Text;
            Ratio = cbRatio.Text;
            Remove = chkRemove.Checked;

            try
            {
                if (rbtFLV.Checked)
                {
                    Mode = "FLV";
                }
                else if (rbtMp4.Checked)
                {
                    Mode = "MP4";
                    Mp4BoxPath = string.Format("{0}"+Slash+"MP4Box.exe", Application.StartupPath);
                    if (!File.Exists(Mp4BoxPath))
                    {
                        MessageBox.Show(
                            string.Format("{0} is not found, copy it to the same folder of FLVExtract, please.", Mp4BoxPath), "Error");
                        return;
                    }
                }
                else if (rbtMkv.Checked)
                {
                    Mode = "MKV";
                    Ratio = cbRatio.Text;
                    MkvmergePath = string.Format("{0}"+Slash+"mkvmerge.exe", Application.StartupPath);
                    if (!File.Exists(MkvmergePath))
                    {
                        MessageBox.Show(
			string.Format("{0} is not found, copy it to the same folder of FLVExtract, please.",         MkvmergePath), "Error");
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
                    Paths.Clear();
                    foreach (ListViewItem checkedItem in lvInput.CheckedItems)
                    {
                        // All these ListViewItems are checked, do something...
                        Paths.Add(checkedItem.SubItems[1].Text);
                    }

                    string[] inputName = Paths.ToArray();

                    var statusForm = new frmStatus(inputName,
                                                   chkVideo.Checked, chkAudio.Checked, chkTimeCodes.Checked);
                    _statusThread = new Thread(() => Invoke((MethodInvoker) delegate
                                                                                {
                                                                                    bool topMost = TopMost;
                                                                                    TopMost = false;
                                                                                    statusForm.ShowDialog();
                                                                                    TopMost = topMost;
                                                                                }));
                    _statusThread.Start();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }


        private void chkOnTop_CheckedChanged(object sender, EventArgs e)
        {
            TopMost = chkOnTop.Checked;
        }

        private void chkVideo_CheckedChanged(object sender, EventArgs e)
        {
            VideoMuxing = chkVideo.Checked;
        }

        private void clearAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btnClear_Click(sender, e);
        }

        /// <summary>
        /// Handles the DragDrop event of the frmMain control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="DragEventArgs"/> instance containing the event data.</param>
        private void frmMain_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                if ((_statusThread != null) && _statusThread.IsAlive) return;

                e.Data.GetData(DataFormats.FileDrop);

                if (!e.Data.GetDataPresent(DataFormats.FileDrop)) return;
                var path = (string[]) e.Data.GetData(DataFormats.FileDrop);
				for (int i = 0; i < path.Length; i++)
                {
                    var file = new FileInfo(path[i]);

                    if (file.Extension.Equals(""))
                    {
                        string[] files = Directory.GetFiles(path[i], "*.flv", SearchOption.TopDirectoryOnly);
                        foreach (string s in files)
                        {
                            var inputFile = new ListViewItem {Checked = true};
                            inputFile.SubItems.Add(s);
                            inputFile.SubItems.Add((new FileInfo(s).Length/1024/1024).ToString(CultureInfo.InvariantCulture) + " MB");

                            lvInput.Items.Add(inputFile);
                        }
                    }
                    else if (file.Extension.Equals(".flv", StringComparison.CurrentCultureIgnoreCase))
                    {
                        var inputFile = new ListViewItem {Checked = true};
                        inputFile.SubItems.Add(path[i]);
                        inputFile.SubItems.Add((new FileInfo(path[i]).Length/1024/1024).ToString(CultureInfo.InvariantCulture) + " MB");

                        lvInput.Items.Add(inputFile);
                    }
                } // for
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("{0}\n{1}\n{2}", ex.Message, ex.StackTrace, ex.HelpLink), "Error " + ex.Source);
            }
        }

        private void frmMain_DragEnter(object sender, DragEventArgs e)
        {
            try
            {
                if ((_statusThread != null) && _statusThread.IsAlive) return;

                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    e.Effect = DragDropEffects.Copy;
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void frmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            SaveSettings();
        }

        /// <summary>
        /// Handles the Load event of the frmMain control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
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

                    var file = new FileInfo(f);

                    if (file.Extension.Equals(""))
                    {
                        string[] files = Directory.GetFiles(f, "*.flv", SearchOption.TopDirectoryOnly);
                        foreach (var s in files)
                        {
                            var inputFile = new ListViewItem();
                            inputFile.SubItems.Add(s);
                            inputFile.SubItems.Add(
                                ((new FileInfo(s).Length)/1024/1024).ToString(CultureInfo.InvariantCulture) + " MB");

                            lvInput.Items.Add(inputFile);
                            inputFile.Checked = true;
                        }
                    }
                    else if (file.Extension.Equals(".flv", StringComparison.CurrentCultureIgnoreCase))
                    {
                        var inputFile = new ListViewItem();
                        inputFile.SubItems.Add(f);
                        inputFile.SubItems.Add(
                            ((new FileInfo(f).Length)/1024/1024).ToString(CultureInfo.InvariantCulture) + " MB");

                        lvInput.Items.Add(inputFile);
                        inputFile.Checked = true;
                    }
                }

                toolTip1.SetToolTip(grpRemux, "Select output type to remux file, no recoding file will be processed.");
                toolTip1.SetToolTip(grpExtract,
                                    "Check which parts you want to keep in output files, timecodes can be uncheck.");
                toolTip1.SetToolTip(this, "Drag & drop FLV file(s) or folder here to add. \nRight click for menu.");

                toolTip1.SetToolTip(cbRatio,
                                    "Changing ratio of video should be currently used for Mkv output files only.");
                toolTip1.SetToolTip(lvInput, "Uncheck to skip file(s)");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Loads the settings.
        /// </summary>
        private void LoadSettings()
        {
            try
            {
                var sr = new SettingsReader("FLV Extract", "settings.txt");
                string val;

                if ((val = sr.Load("ExtractVideo")) != null)
                {
                    chkVideo.Checked = (val != "0");
                }
                if ((val = sr.Load("ExtractTimeCodes")) != null)
                {
                    chkTimeCodes.Checked = (val != "0");
                }
                if ((val = sr.Load("ExtractAudio")) != null)
                {
                    chkAudio.Checked = (val != "0");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void lvInput_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
        }

        /// <summary>
        /// Handles the MouseUp event of the lvInput control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
        private void lvInput_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Right)
                {
                    contextMenuStrip1.Show(lvInput, e.Location);
                }
                
            } catch (Exception exName) {
                Console.WriteLine(exName);
            }
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                //lvInput.SelectedItems.
                if (lvInput.SelectedItems.Count <= 0) return;
                foreach (ListViewItem checkedItem in lvInput.SelectedItems)
                {
                    // All these ListViewItems are Selected, do something...
                    checkedItem.Remove();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        /// <summary>
        /// Saves the settings.
        /// </summary>
        private void SaveSettings()
        {
            try
            {
                var sw = new SettingsWriter("FLV Extract", "settings.txt");

                sw.Save("ExtractVideo", chkVideo.Checked ? "1" : "0");
                sw.Save("ExtractTimeCodes", chkTimeCodes.Checked ? "1" : "0");
                sw.Save("ExtractAudio", chkAudio.Checked ? "1" : "0");

                sw.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// Handles the Click event of the skipTheseFilesToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void skipTheseFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (lvInput.SelectedItems.Count <= 0) return;
                foreach (ListViewItem checkedItem in lvInput.SelectedItems)
                {
                    // All these ListViewItems are Selected, do something...
                    checkedItem.Checked = false;
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void chkAudio_CheckedChanged(object sender, EventArgs e)
        {
            AudioMuxing = chkAudio.Checked;
        }
    }
}