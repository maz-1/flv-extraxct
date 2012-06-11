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
        string video_cmd;
        string audio_source;
        string audio_cmd;
        string target;
        string fps;
        string ratio;
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
            
            //string mode = frmMain.rbtFLV;

			for (int i = 0; (i < _paths.Length) && !_stop; i++) {
				Invoke((MethodInvoker)delegate() {
					item = lvStatus.Items.Add(new ListViewItem(new string[] { String.Empty,
						Path.GetFileName(_paths[i]), String.Empty, String.Empty, String.Empty }));
					item.EnsureVisible();
				});

                string video_source = null;
                string video_cmd = null;
                string audio_source = null;
                string audio_cmd = null;
                string target = null;
                string fps = null;
                string ratio = null;
                string arg = null;

				try {
                    switch(frmMain._mode){
                        case "FLV":
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
                            // Case FLV

                        case "MP4":
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
                                    // MP4Box.exe" -par 1=16:11 -add "F:\Anime\Full Metal Panic! Fumoffu\[A4VF]Full_Metal_Panic_Fumoffu-01.264:fps=29.976" -add "F:\Anime\Full Metal Panic! Fumoffu\[A4VF]Full_Metal_Panic_Fumoffu-01.aac" -itags tool="Yamb 2.1.0.0 [http://yamb.unite-video.com]" -new "F:\Anime\Full Metal Panic! Fumoffu\[A4VF]Full_Metal_Panic_Fumoffu-01.mp4"

                                    /** Checking codes go here**/
                                    // command for video
                                    if (frmMain._fps.Equals("") || frmMain._fps.Equals("Original"))
                                    {
                                        //fps = ":fps=" + Math.Round(flvFile.TrueFrameRate.Value.ToDouble(), 3).ToString();
                                        fps = "";
                                    }
                                    else
                                    {
                                        fps = ":fps=" + frmMain._fps;
                                    }

                                    video_source = Path.ChangeExtension(_paths[i], ".264");
                                    video_cmd = "-add \"" + video_source + fps + "\"";

                                    if (!File.Exists(video_source))
                                    {
                                        video_source = Path.ChangeExtension(_paths[i], ".avi");
                                        video_cmd = "-add \"" + video_source + fps + "\"";
                                        if (!File.Exists(video_source))
                                        {
                                            MessageBox.Show("Video does not exits, please check the video directory or extract setting.", "Error");
                                            //return;
                                        }
                                    }

                                    // command for audio
                                    audio_source = Path.ChangeExtension(_paths[i], ".aac");
                                    audio_cmd = "-add \"" + audio_source + "\"";
                                    if (!File.Exists(audio_source))
                                    {
                                        audio_source = Path.ChangeExtension(_paths[i], ".mp3");
                                        audio_cmd = "-add \"" + audio_source + "\"";
                                        if (!File.Exists(audio_source))
                                        {
                                            MessageBox.Show(audio_source + " does not exits, please check the video directory/extract setting, or output video will have not audio.", "Warring");
                                            audio_cmd = "";
                                        }
                                    }

                                    // command for output
                                    target = Path.ChangeExtension(_paths[i], ".mp4");
                                    if (File.Exists(target))
                                    {
                                        var mes = MessageBox.Show(target + " has already existed, rewrite or save with new name?", "Warning", MessageBoxButtons.YesNoCancel);
                                        if (mes == DialogResult.Yes)
                                        {
                                            File.Delete(target);
                                            // build final command
                                            arg = video_cmd + " " + audio_cmd + " " + "\"" + target + "\"";

                                            ProcessStartInfo startInfo = new ProcessStartInfo();
                                            startInfo.CreateNoWindow = cbxCommand.Checked ? false : true;
                                            startInfo.UseShellExecute = false;
                                            startInfo.FileName = frmMain.mp4box_path;
                                            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                                            startInfo.Arguments = arg;

                                            using (Process exeProcess = Process.Start(startInfo))
                                            {
                                                exeProcess.WaitForExit();

                                            }
                                        } // Yes rewrite

                                        if (mes == System.Windows.Forms.DialogResult.No)
                                        {
                                            target = Path.ChangeExtension(_paths[i], "_new.mp4");
                                            File.Delete(target);
                                            // build final command
                                            arg = video_cmd + " " + audio_cmd + " " + "\"" + target + "\"";

                                            ProcessStartInfo startInfo = new ProcessStartInfo();
                                            startInfo.CreateNoWindow = cbxCommand.Checked ? false : true;
                                            startInfo.UseShellExecute = false;
                                            startInfo.FileName = frmMain.mp4box_path;
                                            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                                            startInfo.Arguments = arg;

                                            using (Process exeProcess = Process.Start(startInfo))
                                            {
                                                exeProcess.WaitForExit();

                                            }
                                        } // No rewrite

                                        else if (mes == System.Windows.Forms.DialogResult.Cancel)
                                        {
                                            item.SubItems[4].Text = "Skipped";
                                        } // Skip
                                    }
                                    else
                                    {
                                        arg = video_cmd + " " + audio_cmd + " " + "\"" + target + "\"";

                                        ProcessStartInfo startInfo = new ProcessStartInfo();
                                        startInfo.CreateNoWindow = cbxCommand.Checked ? false : true;
                                        startInfo.UseShellExecute = false;
                                        startInfo.FileName = frmMain.mp4box_path;
                                        startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                                        startInfo.Arguments = arg;

                                        using (Process exeProcess = Process.Start(startInfo))
                                        {
                                            exeProcess.WaitForExit();

                                        }
                                    }

                                    if (frmMain._remove)
                                    {
                                        File.Delete(video_source);
                                        File.Delete(audio_source);
                                    }

                                    txtStatus.Text = "Done.";

                                    /** End here */
                                });
                                
                            } // using
                            break;
                            // case MP4

                        case "MKV":
                            //MessageBox.Show("Not now", "Information");
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
                                    txtStatus.Text = "Remuxing to mkv file...";
                                    /** Extracting process ends here */

                                    /** Start muxing process here*/
                                    //MessageBox.Show("Done each of file");

                                    // mkvmerge.exe -o "F:\\Anime\\Full Metal Panic! Fumoffu\\[A4VF]Full_Metal_Panic_Fumoffu-01.mkv"  "--default-duration" "0:23.976fps" " "--aspect-ratio" "0:4/3" "-d" "(" "F:\\Anime\\Full Metal Panic! Fumoffu\\[A4VF]Full_Metal_Panic_Fumoffu-01.264" ")" "(" "F:\\Anime\\Full Metal Panic! Fumoffu\\[A4VF]Full_Metal_Panic_Fumoffu-01.aac" ")"

                                    /** Checking codes go here**/
                                    // command for video
                                    if (frmMain._fps.Equals("") || frmMain._fps.Equals("Original"))
                                    {
                                        //fps = Math.Round(flvFile.TrueFrameRate.Value.ToDouble(), 3).ToString();
                                        fps = "";
                                    }
                                    else
                                    {
                                        fps = "\"--default-duration\" \"0:" + frmMain._fps + "fps\" ";
                                    } // fps

                                    if (frmMain._ratio.Equals("") || frmMain._ratio.Equals("Original"))
                                    {
                                        ratio = "";
                                    }
                                    else
                                    {
                                        ratio = "\"--aspect-ratio\" \"0:" + frmMain._ratio.Replace(":", "/") + "\" ";
                                    } // ratio

                                    video_source = Path.ChangeExtension(_paths[i], ".264");
                                    video_cmd = fps + ratio + "\"" + video_source + "\"";
                                    if (!File.Exists(video_source))
                                    {
                                        video_source = Path.ChangeExtension(_paths[i], ".avi");
                                        video_cmd = "\"--default-duration\" \"0:" + fps + "fps\" " + ratio + "\"" + video_source + "\"";
                                        if (!File.Exists(video_source))
                                        {
                                            MessageBox.Show("Video does not exist, please check the video directory or extract setting.", "Error");
                                            //return;
                                        }
                                    }

                                    // command for audio
                                    audio_source = Path.ChangeExtension(_paths[i], ".aac");
                                    audio_cmd = "\"" + audio_source + "\"";
                                    if (!File.Exists(audio_source))
                                    {
                                        audio_source = Path.ChangeExtension(_paths[i], ".mp3");
                                        audio_cmd = "\"" + audio_source + "\"";
                                        if (!File.Exists(audio_source))
                                        {
                                            MessageBox.Show(audio_source + " does not exist, please check the video directory/extract setting, or output video will have not audio.", "Warring");
                                            audio_cmd = "";
                                        }
                                    }

                                    // command for output
                                    target = Path.ChangeExtension(_paths[i], ".mkv");
                                    if (File.Exists(target))
                                    {
                                        var mes = MessageBox.Show(target + " has already existed, rewrite or save with new name?", "Warning", MessageBoxButtons.YesNoCancel);
                                        if (mes == DialogResult.Yes)
                                        {
                                            File.Delete(target);
                                            // build final command
                                            arg = video_cmd + " " + audio_cmd + " -o \"" + target + "\"";

                                            ProcessStartInfo startInfo = new ProcessStartInfo();
                                            startInfo.CreateNoWindow = cbxCommand.Checked ? false : true;
                                            startInfo.UseShellExecute = false;
                                            startInfo.FileName = frmMain.mkvmerge_path;
                                            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                                            startInfo.Arguments = arg;

                                            using (Process exeProcess = Process.Start(startInfo))
                                            {
                                                exeProcess.WaitForExit();

                                            }
                                        } // Yes rewrite

                                        if (mes == DialogResult.No)
                                        {

                                            target = Path.ChangeExtension(_paths[i], "_new.mkv");
                                            File.Delete(target);
                                            // build final command
                                            arg = video_cmd + " " + audio_cmd + " -o \"" + target + "\"";

                                            ProcessStartInfo startInfo = new ProcessStartInfo();
                                            startInfo.CreateNoWindow = cbxCommand.Checked ? false : true;
                                            startInfo.UseShellExecute = false;
                                            startInfo.FileName = frmMain.mkvmerge_path;
                                            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                                            startInfo.Arguments = arg;

                                            using (Process exeProcess = Process.Start(startInfo))
                                            {
                                                exeProcess.WaitForExit();

                                            }
                                        } // No rewrite

                                        else if (mes == System.Windows.Forms.DialogResult.Cancel)
                                        {
                                            item.SubItems[4].Text = "Skipped";
                                        }
                                    }
                                    else
                                    {
                                        arg = video_cmd + " " + audio_cmd + " -o \"" + target + "\"";

                                        ProcessStartInfo startInfo = new ProcessStartInfo();
                                        startInfo.CreateNoWindow = cbxCommand.Checked ? false : true;
                                        startInfo.UseShellExecute = false;
                                        startInfo.FileName = frmMain.mkvmerge_path;
                                        startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                                        startInfo.Arguments = arg;

                                        using (Process exeProcess = Process.Start(startInfo))
                                        {
                                            exeProcess.WaitForExit();

                                        }
                                    }

                                    if (frmMain._remove)
                                    {
                                        File.Delete(video_source);
                                        File.Delete(audio_source);
                                    }

                                    txtStatus.Text = "Done.";

                                    /** End here */
                                });
                                
                            } // using
                            break;
                            // case Mkv
                            
                        default:
                            MessageBox.Show("Not now", "Information");
                            break;
				    }
                } // try
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
