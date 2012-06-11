using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;

namespace JDP {
	public partial class frmStatus : Form {
		Thread _workThread;
		volatile string[] _paths;
		volatile bool _stop;
		volatile bool _extractVideo;
		volatile bool _extractAudio;
		volatile bool _extractTimeCodes;
		bool _overwriteAll;
		bool _overwriteNone;        
        
        string video_source;
        string audio_source;
        string target;
        string fps;
        string arg;
        

		public frmStatus(string[] paths, bool extractVideo, bool extractAudio, bool extractTimeCodes) {
			InitializeComponent();
			int initialWidth = ClientSize.Width;
			Program.SetFontAndScaling(this);
			float scaleFactorX = (float)ClientSize.Width / initialWidth;
			foreach (ColumnHeader columnHeader in lvStatus.Columns) {
				columnHeader.Width = Convert.ToInt32(columnHeader.Width * scaleFactorX);
			}

			_paths = paths;
			_extractVideo = extractVideo;
			_extractAudio = extractAudio;
			_extractTimeCodes = extractTimeCodes;

			ImageList imageList = new ImageList();
			imageList.ColorDepth = ColorDepth.Depth32Bit;
			AddToImageListFromResource(imageList, Properties.Resources.OK);
			AddToImageListFromResource(imageList, Properties.Resources.Warning);
			AddToImageListFromResource(imageList, Properties.Resources.Error);
			lvStatus.SmallImageList = imageList;
		}

		private void frmStatus_Shown(object sender, EventArgs e) {
			Activate();

			_workThread = new Thread(new ThreadStart(ExtractFilesThread));
			_workThread.Start();
		}

		private void frmStatus_FormClosing(object sender, FormClosingEventArgs e) {
			if ((_workThread != null) && _workThread.IsAlive) {
				e.Cancel = true;
			}
		}

		private void lvStatus_MouseDoubleClick(object sender, MouseEventArgs e) {
			ListViewItem item = lvStatus.GetItemAt(e.X, e.Y);
			if ((item != null) && (item.SubItems[4].Tag != null)) {
				MessageBox.Show((string)item.SubItems[4].Tag, "Stack Trace", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
		}

		private void btnStop_Click(object sender, EventArgs e) {
			_stop = true;
			btnStop.Enabled = false;
		}

		private void btnCopyFrameRates_Click(object sender, EventArgs e) {
			StringBuilder sb = new StringBuilder();

			foreach (ListViewItem item in lvStatus.Items) {
				if ((item.SubItems[2].Tag == null) && (item.SubItems[3].Tag == null)) {
					continue;
				}

				sb.Append("File:  ");
				sb.AppendLine(item.SubItems[1].Text);

				if (item.SubItems[2].Tag != null) {
					sb.Append("Estimated True Frame Rate:  ");
					sb.AppendLine(((FractionUInt32)item.SubItems[2].Tag).ToString(true));
				}

				if (item.SubItems[3].Tag != null) {
					sb.Append("Average Frame Rate:  ");
					sb.AppendLine(((FractionUInt32)item.SubItems[3].Tag).ToString(true));
				}

				sb.AppendLine();
			}

			if (sb.Length != 0) {
				Clipboard.Clear();
				Clipboard.SetText(sb.ToString(0, sb.Length - Environment.NewLine.Length));
			}
		}

		private void AddToImageListFromResource(ImageList imageList, Bitmap resource) {
			Icon icon = Icon.FromHandle(resource.GetHicon());
			imageList.Images.Add(icon);
			icon.Dispose();
			resource.Dispose();
		}

		private bool PromptOverwrite(string path) {
			if (_overwriteAll) return true;
			if (_overwriteNone) return false;

			bool overwrite = false;

			Invoke((MethodInvoker)delegate() {
				frmOverwrite dialog = new frmOverwrite(path);
				DialogResult res = dialog.ShowDialog();

				if (res == DialogResult.Yes) {
					overwrite = true;
					if (dialog.ToAll) {
						_overwriteAll = true;
					}
				}
				else if (res == DialogResult.Cancel) {
					btnStop_Click(null, null);
				}
				else {
					if (dialog.ToAll) {
						_overwriteNone = true;
					}
				}
			});

			return overwrite;
		}

		private void ExtractFilesThread() {
			ListViewItem item = null;
            string mode = "mp4";

			for (int i = 0; (i < _paths.Length) && !_stop; i++) {
				Invoke((MethodInvoker)delegate() {
					item = lvStatus.Items.Add(new ListViewItem(new string[] { String.Empty,
						Path.GetFileName(_paths[i]), String.Empty, String.Empty, String.Empty }));
					item.EnsureVisible();
				});

				try {
                    switch(mode)
                    {
                        case "flv":
                            
					        using (FLVFile flvFile = new FLVFile(_paths[i])) {
                                
						        flvFile.ExtractStreams(_extractAudio, _extractVideo, _extractTimeCodes, PromptOverwrite);

						        Invoke((MethodInvoker)delegate() {
                                    //txtStatus.Text = "Extracting...";
							        if (flvFile.TrueFrameRate != null) {
								        item.SubItems[2].Text = flvFile.TrueFrameRate.Value.ToString(false);
								        item.SubItems[2].Tag = flvFile.TrueFrameRate;
							        }
							        if (flvFile.AverageFrameRate != null) {
								        item.SubItems[3].Text = flvFile.AverageFrameRate.Value.ToString(false);
								        item.SubItems[3].Tag = flvFile.AverageFrameRate;
							        }
							        if (flvFile.Warnings.Length == 0) {
								        item.ImageIndex = (int)IconIndex.OK;
							        }
							        else {
								        item.ImageIndex = (int)IconIndex.Warning;
								        item.SubItems[4].Text = String.Join("  ", flvFile.Warnings);
							        }
                                    //txtStatus.Text = "Done extracting.";
						        });                        
					        }
                            
                            break;

                        case "mp4":

                            using (FLVFile flvFile = new FLVFile(_paths[i]))
                            {
                                flvFile.ExtractStreams(_extractAudio, _extractVideo, _extractTimeCodes, PromptOverwrite);

                                Invoke((MethodInvoker)delegate()
                                {
                                    //txtStatus.Text = "Extracting...";
                                    if (flvFile.TrueFrameRate != null)
                                    {
                                        item.SubItems[2].Text = flvFile.TrueFrameRate.Value.ToString(false);
                                        item.SubItems[2].Tag = flvFile.TrueFrameRate;
                                    }
                                    if (flvFile.AverageFrameRate != null)
                                    {
                                        item.SubItems[3].Text = flvFile.AverageFrameRate.Value.ToString(false);
                                        item.SubItems[3].Tag = flvFile.AverageFrameRate;
                                    }
                                    if (flvFile.Warnings.Length == 0)
                                    {
                                        item.ImageIndex = (int)IconIndex.OK;
                                    }
                                    else
                                    {
                                        item.ImageIndex = (int)IconIndex.Warning;
                                        item.SubItems[4].Text = String.Join("  ", flvFile.Warnings);
                                    }
                                    txtStatus.Visible = true;
                                    txtStatus.Text = "Remuxing to mp4 file...";
                                    /** Extracting process ends here */

                                    /** Start muxing process here*/
                                    //MessageBox.Show("Done each of file");

                                    // -add "F:\Anime\Full Metal Panic! Fumoffu\[A4VF]Full_Metal_Panic_Fumoffu-01.264:fps=23.976" -add "F:\Anime\Full Metal Panic! Fumoffu\[A4VF]Full_Metal_Panic_Fumoffu-01.aac" "F:\Anime\Full Metal Panic! Fumoffu\[A4VF]Full_Metal_Panic_Fumoffu-01.mp4"

                                    video_source = Path.ChangeExtension(_paths[i], ".264");
                                    audio_source = Path.ChangeExtension(_paths[i], ".aac");
                                    target = Path.ChangeExtension(_paths[i], ".mp4");

                                    fps = "23.976";
                                    arg = "-add \"" + video_source + ":fps=" + fps + "\" -add \"" + audio_source + "\" \"" + target + "\"";

                                    /** Checking codes go here**/
                                    arg.Remove(arg.IndexOf("\\"), 1);
                                    //MessageBox.Show(arg);

                                    ProcessStartInfo startInfo = new ProcessStartInfo();
                                    startInfo.CreateNoWindow = true;
                                    startInfo.UseShellExecute = false;
                                    startInfo.FileName = frmMain.mp4box_path;
                                    startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                                    startInfo.Arguments = arg;

                                    using (Process exeProcess = Process.Start(startInfo))
                                    {
                                        exeProcess.WaitForExit();
                                        
                                    }
                                    txtStatus.Text = "Done.";
                                    /** End here */
                                });
                                
                            }
                        break;
				    }
                }
				catch (Exception ex) {
					Invoke((MethodInvoker)delegate() {
						item.ImageIndex = (int)IconIndex.Error;
						item.SubItems[4].Text = ex.Message;
						item.SubItems[4].Tag = ex.StackTrace;
					});
				}
			}

			Invoke((MethodInvoker)delegate() {
				btnStop.Visible = false;
				btnCopyFrameRates.Enabled = true;
				btnOK.Enabled = true;
			});
        
		}
	}

	enum IconIndex {
		OK,
		Warning,
		Error
	}
}
