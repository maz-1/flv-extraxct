using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using JDP.Properties;

namespace JDP
{
    public partial class frmStatus : Form
    {
        private volatile bool _extractAudio;
        private volatile bool _extractTimeCodes;
        private volatile bool _extractVideo;
        private bool _overwriteAll;
        private bool _overwriteNone;
        private volatile string[] _paths;
        private volatile bool _stop;
        private Thread _workThread;

        public frmStatus(string[] paths, bool extractVideo, bool extractAudio, bool extractTimeCodes)
        {
            InitializeComponent();
            int initialWidth = ClientSize.Width;
            Program.SetFontAndScaling(this);
            float scaleFactorX = (float)ClientSize.Width / initialWidth;
            foreach (ColumnHeader columnHeader in lvStatus.Columns)
            {
                columnHeader.Width = Convert.ToInt32(columnHeader.Width * scaleFactorX);
            }

            _paths = paths;
            _extractVideo = extractVideo;
            _extractAudio = extractAudio;
            _extractTimeCodes = extractTimeCodes;

            var imageList = new ImageList { ColorDepth = ColorDepth.Depth32Bit };
            AddToImageListFromResource(imageList, Resources.OK);
            AddToImageListFromResource(imageList, Resources.Warning);
            AddToImageListFromResource(imageList, Resources.Error);
            lvStatus.SmallImageList = imageList;
        }

        private void frmStatus_Shown(object sender, EventArgs e)
        {
            Activate();

            _workThread = new Thread(ExtractFilesThread);
            _workThread.Start();
        }

        private void frmStatus_FormClosing(object sender, FormClosingEventArgs e)
        {
            if ((_workThread != null) && _workThread.IsAlive)
            {
                e.Cancel = true;
            }
        }

        private void lvStatus_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            var item = lvStatus.GetItemAt(e.X, e.Y);
            if ((item != null) && (item.SubItems[4].Tag != null))
            {
                MessageBox.Show((string)item.SubItems[4].Tag, "Stack Trace", MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            _stop = true;
            btnStop.Enabled = false;
        }

        private void btnCopyFrameRates_Click(object sender, EventArgs e)
        {
            var sb = new StringBuilder();

            foreach (ListViewItem item in lvStatus.Items)
            {
                if ((item.SubItems[2].Tag == null) && (item.SubItems[3].Tag == null))
                {
                    continue;
                }

                sb.Append("File:  ");
                sb.AppendLine(item.SubItems[1].Text);

                if (item.SubItems[2].Tag != null)
                {
                    sb.Append("Estimated True Frame Rate:  ");
                    sb.AppendLine(((FractionUInt32)item.SubItems[2].Tag).ToString(true));
                }

                if (item.SubItems[3].Tag != null)
                {
                    sb.Append("Average Frame Rate:  ");
                    sb.AppendLine(((FractionUInt32)item.SubItems[3].Tag).ToString(true));
                }

                sb.AppendLine();
            }

            if (sb.Length == 0) return;
            Clipboard.Clear();
            Clipboard.SetText(sb.ToString(0, sb.Length - Environment.NewLine.Length));
        }

        private void AddToImageListFromResource(ImageList imageList, Bitmap resource)
        {
            Icon icon = Icon.FromHandle(resource.GetHicon());
            imageList.Images.Add(icon);
            icon.Dispose();
            resource.Dispose();
        }

        private bool PromptOverwrite(string path)
        {
            if (_overwriteAll) return true;
            if (_overwriteNone) return false;

            bool overwrite = false;

            Invoke((MethodInvoker)delegate
                                       {
                                           var dialog = new frmOverwrite(path);
                                           var res = dialog.ShowDialog();

                                           if (res == DialogResult.Yes)
                                           {
                                               overwrite = true;
                                               if (dialog.ToAll)
                                               {
                                                   _overwriteAll = true;
                                               }
                                           }
                                           else if (res == DialogResult.Cancel)
                                           {
                                               btnStop_Click(null, null);
                                           }
                                           else
                                           {
                                               if (dialog.ToAll)
                                               {
                                                   _overwriteNone = true;
                                               }
                                           }
                                       });

            return overwrite;
        }

        /// <summary>
        /// Extracts the files thread.
        /// </summary>
        private void ExtractFilesThread()
        {
            ListViewItem item = null;

            //string mode = frmMain.rbtFLV;

            for (var i = 0; (i < _paths.Length) && !_stop; i++)
            {
                var i1 = i;
                Invoke((MethodInvoker)delegate
                                           {
                                               item = lvStatus.Items.Add(new ListViewItem(new[]
																							  {
																								  String.Empty,
																								  Path.GetFileName(
																									  _paths[i1]),
																								  String.Empty,
																								  String.Empty,
																								  String.Empty
																							  }));
                                               item.EnsureVisible();
                                           });

                string videoSource = "";
                string videoCmd = "";
                string audioSource = "";
                string audioCmd = "";
                string target = "";
                string fps = "";
                string ratio = "";
                string arg = "";

                try
                {
                    switch (frmMain.Mode)
                    {
                        #region "FLV"

                        case "FLV":
                            using (var flvFile = new FLVFile(_paths[i]))
                            {
                                flvFile.ExtractStreams(_extractAudio, _extractVideo, _extractTimeCodes,
                                                       PromptOverwrite);

                                Invoke((MethodInvoker)(delegate
                                                            {
                                                                //txtStatus.Text = "Extracting...";
                                                                if (flvFile.TrueFrameRate != null)
                                                                {
                                                                    item.SubItems[2].Text =
                                                                        flvFile.TrueFrameRate.Value.ToString(false);
                                                                    item.SubItems[2].Tag = flvFile.TrueFrameRate;
                                                                }
                                                                if (flvFile.AverageFrameRate != null)
                                                                {
                                                                    item.SubItems[3].Text =
                                                                        flvFile.AverageFrameRate.Value.ToString(
                                                                            false);
                                                                    item.SubItems[3].Tag = flvFile.AverageFrameRate;
                                                                }
                                                                if (flvFile.Warnings.Length == 0)
                                                                {
                                                                    item.ImageIndex = (int)IconIndex.OK;
                                                                }
                                                                else
                                                                {
                                                                    item.ImageIndex = (int)IconIndex.Warning;
                                                                    item.SubItems[4].Text = String.Join("  ",
                                                                                                        flvFile.
                                                                                                            Warnings);
                                                                }

                                                                //txtStatus.Text = "Done extracting.";
                                                            }));
                            }
                            break;

                        // Case FLV

                        #endregion "FLV"

                        #region "MP4"
                        case "MP4":
                            using (var flvFile = new FLVFile(_paths[i]))
                            {
                                flvFile.ExtractStreams(_extractAudio, _extractVideo, _extractTimeCodes, PromptOverwrite);

                                Invoke((MethodInvoker)delegate
                                                           {
                                                               txtStatus.Text = "Extracting...";
                                                               if (flvFile != null && flvFile.TrueFrameRate != null)
                                                               {
                                                                   item.SubItems[2].Text =
                                                                       flvFile.TrueFrameRate.Value.ToString(false);
                                                                   item.SubItems[2].Tag = flvFile.TrueFrameRate;
                                                               }
                                                               if (flvFile.AverageFrameRate != null)
                                                               {
                                                                   item.SubItems[3].Text =
                                                                       flvFile.AverageFrameRate.Value.ToString(false);
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

                                                               // command for video
                                                               if (frmMain.AudioMuxing && frmMain.VideoMuxing == false)
                                                               {
                                                                   fps = "";
                                                                   videoCmd = "";
                                                                   // no video muxing

                                                                   // command for audio
                                                                   audioSource = Path.ChangeExtension(_paths[i], ".aac");
                                                                   audioCmd = "-add \"" + audioSource + "#audio" + "\" ";
                                                                   if (!File.Exists(audioSource))
                                                                   {
                                                                       audioSource = Path.ChangeExtension(_paths[i], ".mp3");
                                                                       audioCmd = "-add \"" + audioSource + "#audio" + "\" ";
                                                                       if (!File.Exists(audioSource))
                                                                       {
                                                                           item.SubItems[4].Text = audioSource + " does not exits, please check the video directory/extract setting, or output video will have not audio.";
                                                                           audioCmd = "";
                                                                       }
                                                                   }
                                                               }

                                                               else if (frmMain.AudioMuxing == false && frmMain.VideoMuxing)
                                                               {
                                                                   audioCmd = "";

                                                                   fps = (frmMain.Fps.Equals("") || frmMain.Fps.Equals("Auto")) ? ":fps=" + flvFile.TrueFrameRate.ToString() : ":fps=" + frmMain.Fps;

                                                                   if ("Original".Equals(frmMain.Ratio))
                                                                   {
                                                                       ratio = "";
                                                                   }
                                                                   else if ("4:3".Equals(frmMain.Ratio))
                                                                   {
                                                                       ratio = ":par=1:1";
                                                                   }
                                                                   else if ("16:9".Equals(frmMain.Ratio))
                                                                   {
                                                                       ratio = ":par=1.78:1.33";
                                                                   }
                                                                   videoSource = Path.ChangeExtension(_paths[i], ".264");
                                                                   videoCmd = "-add \"" + videoSource + "#video" + fps + ratio + "\" ";

                                                                   if (!File.Exists(videoSource))
                                                                   {
                                                                       videoSource = Path.ChangeExtension(_paths[i], ".avi");
                                                                       videoCmd = "-add \"" + videoSource + "#video" + fps + ratio + "\" ";
                                                                       if (!File.Exists(videoSource))
                                                                       {
                                                                           item.SubItems[4].Text = "Video does not exits, please check the video directory or extract setting.";
                                                                       }
                                                                   }
                                                               }

                                                               else if (frmMain.AudioMuxing && frmMain.VideoMuxing)
                                                               {
                                                                   // command for audio
                                                                   audioSource = Path.ChangeExtension(_paths[i], ".aac");
                                                                   audioCmd = "-add \"" + audioSource + "#audio" + "\" ";

                                                                   if (!File.Exists(audioSource))
                                                                   {
                                                                       audioSource = Path.ChangeExtension(_paths[i], ".mp3");
                                                                       audioCmd = "-add \"" + audioSource + "#audio" + "\" ";
                                                                       if (!File.Exists(audioSource))
                                                                       {
                                                                          item.SubItems[4].Text = audioSource + " does not exits, please check the video directory/extract setting, or output video will have not audio.";
                                                                           audioCmd = "";
                                                                       }
                                                                   }

                                                                   // end command for audio
                                                                   if (frmMain.Fps.Equals("") ||
                                                                       frmMain.Fps.Equals("Auto"))
                                                                   {
                                                                       //fps = ":fps=" + Math.Round(flvFile.TrueFrameRate.Value.ToDouble(), 3).ToString();
                                                                       fps = ":fps=" + flvFile.TrueFrameRate.ToString();
                                                                   }
                                                                   else
                                                                   {
                                                                       fps = ":fps=" + frmMain.Fps;
                                                                   }
                                                                   if (frmMain.Ratio.Equals("Original"))
                                                                   {
                                                                       ratio = "";
                                                                   }
                                                                   else if (frmMain.Ratio.Equals("4:3"))
                                                                   {
                                                                       ratio = ":par=1:1";
                                                                   }
                                                                   else if (frmMain.Ratio.Equals("16:9"))
                                                                   {
                                                                       ratio = ":par=1.78:1.33";
                                                                   }

                                                                   videoSource = Path.ChangeExtension(_paths[i], ".264");
                                                                   videoCmd = "-add \"" + videoSource + "#video" + fps + ratio + "\" ";

                                                                   if (!File.Exists(videoSource))
                                                                   {
                                                                       videoSource = Path.ChangeExtension(_paths[i], ".avi");
                                                                       videoCmd = "-add \"" + videoSource + "#video" + fps + ratio + "\" ";
                                                                       if (!File.Exists(videoSource))
                                                                       {
                                                                           item.SubItems[4].Text = "Video does not exits, please check the video directory or extract setting.";
                                                                       }
                                                                   }
                                                               } // else if has video / audio
                                                               else
                                                               {
                                                                   return;
                                                               }

                                                               // command for output
                                                               target = Path.ChangeExtension(_paths[i], frmMain.VideoMuxing ? ".mp4" : ".m4a");

                                                               if (File.Exists(target))
                                                               {
                                                                   var mes =
                                                                       MessageBox.Show(
                                                                           string.Format("{0} has already existed, rewrite or save with new name?", target),
                                                                           "Warning", MessageBoxButtons.YesNoCancel,
                                                                           MessageBoxIcon.Question);
                                                                   if (mes == DialogResult.Yes)
                                                                   {
                                                                       File.Delete(target);

                                                                       // build final command and mux files
                                                                       arg = videoCmd + audioCmd + "\"" + target + "\"";
                                                                       Mp4Muxing(arg);
                                                                   } // Yes rewrite

                                                                   switch (mes)
                                                                   {
                                                                       case DialogResult.No:
                                                                           target = target.Insert(target.LastIndexOf(".", StringComparison.Ordinal),
                                                                                                  "_new");
                                                                           File.Delete(target);
                                                                           arg = videoCmd + audioCmd + "\"" + target + "\"";
                                                                           Mp4Muxing(arg);
                                                                           break;

                                                                       case DialogResult.Cancel:
                                                                           item.SubItems[4].Text = "Skipped";
                                                                           break;
                                                                   }
                                                               }
                                                               else // no exist
                                                               {
                                                                   arg = videoCmd + audioCmd + "\"" + target + "\"";
                                                                   Mp4Muxing(arg);
                                                               }

                                                               // end command for output

                                                               if (frmMain.Remove)
                                                               {
                                                                   try
                                                                   {
                                                                       if (frmMain.AudioMuxing) File.Delete(audioSource);
                                                                       if (frmMain.VideoMuxing) File.Delete(videoSource);
                                                                       File.Delete(Path.ChangeExtension(_paths[i],".txt"));
                                                                   }
                                                                   catch (Exception ex)
                                                                   {
                                                                       Console.WriteLine(ex);
                                                                   }
                                                               }

                                                               txtStatus.Text = "Done.";

                                                               /** End here */
                                                           });
                            } // using
                            break; // case MP4

                        #endregion "MP4"

                        #region "MKV"

                        case "MKV":

                            //MessageBox.Show("Not now", "Information");
                            using (var flvFile = new FLVFile(_paths[i]))
                            {
                                flvFile.ExtractStreams(_extractAudio, _extractVideo, _extractTimeCodes, PromptOverwrite);

                                Invoke((MethodInvoker)delegate
                                                           {
                                                               txtStatus.Text = "Extracting...";
                                                               if (flvFile.TrueFrameRate != null)
                                                               {
                                                                   item.SubItems[2].Text =
                                                                       flvFile.TrueFrameRate.Value.ToString(false);
                                                                   item.SubItems[2].Tag = flvFile.TrueFrameRate;
                                                               }
                                                               if (flvFile.AverageFrameRate != null)
                                                               {
                                                                   item.SubItems[3].Text =
                                                                       flvFile.AverageFrameRate.Value.ToString(false);
                                                                   item.SubItems[3].Tag = flvFile.AverageFrameRate;
                                                               }
                                                               if (flvFile.Warnings.Length == 0)
                                                               {
                                                                   item.ImageIndex = (int)IconIndex.OK;
                                                               }
                                                               else
                                                               {
                                                                   item.ImageIndex = (int)IconIndex.Warning;
                                                                   item.SubItems[4].Text = String.Join("  ",
                                                                                                       flvFile.Warnings);
                                                               }
                                                               txtStatus.Visible = true;
                                                               txtStatus.Text = "Remuxing to mkv file...";
                                                               /** Extracting process ends here */

                                                               /** Start muxing process here*/

                                                               // mkvmerge.exe -o "F:\\Anime\\Full Metal Panic! Fumoffu\\[A4VF]Full_Metal_Panic_Fumoffu-01.mkv"  "--default-duration" "0:23.976fps" " "--aspect-ratio" "0:4/3" "-d" "(" "F:\\Anime\\Full Metal Panic! Fumoffu\\[A4VF]Full_Metal_Panic_Fumoffu-01.264" ")" "(" "F:\\Anime\\Full Metal Panic! Fumoffu\\[A4VF]Full_Metal_Panic_Fumoffu-01.aac" ")"

                                                               /** Checking codes go here**/

                                                               // command for video
                                                               if (frmMain.AudioMuxing && frmMain.VideoMuxing == false)
                                                               {
                                                                   fps = "";
                                                                   ratio = "";
                                                                   videoCmd = "";

                                                                   // command for audio
                                                                   audioSource = Path.ChangeExtension(_paths[i], ".aac");
                                                                   audioCmd = "\"" + audioSource + "\"";
                                                                   if (!File.Exists(audioSource))
                                                                   {
                                                                       audioSource = Path.ChangeExtension(_paths[i], ".mp3");
                                                                       audioCmd = "\"" + audioSource + "\"";
                                                                       if (!File.Exists(audioSource))
                                                                       {
                                                                           item.SubItems[4].Text = string.Format("{0} does not exist, please check the video directory/extract setting, or output video will have not audio.", audioSource);
                                                                           audioCmd = "";
                                                                       }
                                                                   }
                                                               }

                                                               else if (frmMain.AudioMuxing == false && frmMain.VideoMuxing)
                                                               {
                                                                   audioCmd = "";

                                                                   fps = ("".Equals(frmMain.Fps) || "Auto".Equals(frmMain.Fps)) ? "" : "\"--default-duration\" \"0:" + frmMain.Fps + "fps\" ";
                                                                   ratio = ("".Equals(frmMain.Ratio) || "Original".Equals(frmMain.Ratio)) ? "" : "\"--aspect-ratio\" \"0:" + frmMain.Ratio.Replace(":", "/") + "\" ";

                                                                   videoSource = Path.ChangeExtension(_paths[i], ".264");
                                                                   videoCmd = fps + ratio + "\"(\" " + videoSource + "\" \")\" ";
                                                                   if (!File.Exists(videoSource))
                                                                   {
                                                                       videoSource = Path.ChangeExtension(_paths[i], ".avi");
                                                                       videoCmd = fps + ratio + "\"(\" \"" + videoSource + "\" \")\" ";
                                                                       if (!File.Exists(videoSource))
                                                                       {
                                                                           item.SubItems[4].Text = "Video does not exist, please check the video directory or extract setting.";

                                                                           //return;
                                                                       }
                                                                   }
                                                               }
                                                               else if (frmMain.AudioMuxing && frmMain.VideoMuxing)
                                                               {
                                                                   // command for audio
                                                                   audioSource = Path.ChangeExtension(_paths[i], ".aac");
                                                                   audioCmd = "\"" + audioSource + "\"";

                                                                   if (!File.Exists(audioSource))
                                                                   {
                                                                       audioSource = Path.ChangeExtension(_paths[i], ".mp3");
                                                                       audioCmd = "\"" + audioSource + "\"";
                                                                       if (!File.Exists(audioSource))
                                                                       {
                                                                           item.SubItems[4].Text =
                                                                               string.Format(
                                                                                   "{0} does not exist, please check the video directory/extract setting, or output video will have not audio.",
                                                                                   audioSource);
                                                                           audioCmd = "";
                                                                       }
                                                                   }

                                                                   fps = ("".Equals(frmMain.Fps) || "Auto".Equals(frmMain.Fps)) ? "" : "\"--default-duration\" \"0:" + frmMain.Fps + "fps\" ";
                                                                   ratio = ("".Equals(frmMain.Ratio) || "Original".Equals(frmMain.Ratio)) ? "" : "\"--aspect-ratio\" \"0:" + frmMain.Ratio.Replace(":", "/") + "\" ";

                                                                   videoSource = Path.ChangeExtension(_paths[i], ".264");
                                                                   videoCmd = fps + ratio + "\"" + videoSource + "\" ";
                                                                   if (!File.Exists(videoSource))
                                                                   {
                                                                       videoSource = Path.ChangeExtension(_paths[i],".avi");
                                                                       videoCmd = fps + ratio + "\"(\" \"" + videoSource + "\" \")\" ";

                                                                       if (!File.Exists(videoSource))
                                                                       {
                                                                           MessageBox.Show("Video does not exist, please check the video directory or extract setting.");
                                                                       }
                                                                   }
                                                               } // end video / audio command

                                                               // command for output
                                                               target = Path.ChangeExtension(_paths[i], frmMain.VideoMuxing ? ".mkv" : ".mka");

                                                               if (File.Exists(target))
                                                               {
                                                                   var mes =
                                                                       MessageBox.Show(
                                                                           string.Format("{0} has already existed, rewrite or save with new name?", target),
                                                                           "Warning", MessageBoxButtons.YesNoCancel,
                                                                           MessageBoxIcon.Question);
                                                                   switch (mes)
                                                                   {
                                                                       case DialogResult.Yes:
                                                                           File.Delete(target);
                                                                           arg =  " -o \"" + target + "\" " +  videoCmd + audioCmd;
                                                                           MkvMuxing(arg);
                                                                           break;
                                                                       case DialogResult.No:
                                                                           target = target.Insert(target.LastIndexOf(".", System.StringComparison.Ordinal), "_new");
                                                                           File.Delete(target);
                                                                           arg = " -o \"" + target + "\" " + videoCmd + audioCmd;
                                                                           MkvMuxing(arg);
                                                                           break;

                                                                       case DialogResult.Cancel:
                                                                           item.SubItems[4].Text = "Skipped";
                                                                           break;
                                                                   }
                                                               }
                                                               else
                                                               {
                                                                   arg = " -o \"" + target + "\" " + videoCmd + audioCmd;
                                                                   MkvMuxing(arg);
                                                               }

                                                               if (frmMain.Remove)
                                                               {
                                                                   try
                                                                   {
                                                                       if (frmMain.AudioMuxing) File.Delete(audioSource);
                                                                       if (frmMain.VideoMuxing) File.Delete(videoSource);
                                                                       File.Delete(Path.ChangeExtension(_paths[i], ".txt"));
                                                                   }
                                                                   catch (Exception ex)
                                                                   {
                                                                       Console.WriteLine(ex);
                                                                   }
                                                               }

                                                               txtStatus.Text = "Done.";

                                                               /** End here */
                                                           });
                            } // using
                            break;

                        // case Mkv

                        #endregion "MKV"

                        default:
                            MessageBox.Show("Not now", "Information");
                            break;
                    }
                } // try
                catch (Exception ex)
                {
                    Invoke((MethodInvoker)(() =>
                                               {
                                                   item.ImageIndex = (int)IconIndex.Error;
                                                   item.SubItems[4].Text = ex.Message;
                                                   item.SubItems[4].Tag = ex.StackTrace;
                                               }));
                }
            }

            Invoke((MethodInvoker)delegate
                                       {
                                           btnStop.Visible = false;
                                           btnCopyFrameRates.Enabled = true;
                                           btnOK.Enabled = true;
                                       });
        }

        /// <summary>
        /// MP4s the muxing.
        /// </summary>
        /// <param name="arg">The arg.</param>
        private void Mp4Muxing(string arg)
        {
            try
            {
//arg = video_cmd + audio_cmd + "\"" + target + "\"";

                var startInfo = new ProcessStartInfo
                                    {
                                        CreateNoWindow = !chkCommand.Checked,
                                        UseShellExecute = false,
                                        FileName = frmMain.Mp4BoxPath,
                                        WindowStyle = ProcessWindowStyle.Hidden,
                                        Arguments = arg
                                    };

                using (var exeProcess = Process.Start(startInfo))
                {
                    exeProcess.WaitForExit();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        /// <summary>
        /// MKVs the muxing.
        /// </summary>
        /// <param name="arg">The arg.</param>
        private void MkvMuxing(string arg)
        {
            try
            {
// arg = video_cmd + audio_cmd + " -o \"" + target + "\"";
                //MessageBox.Show(arg);
                var startInfo = new ProcessStartInfo
                                    {
                                        CreateNoWindow = !chkCommand.Checked,
                                        UseShellExecute = false,
                                        FileName = frmMain.MkvmergePath,
                                        WindowStyle = ProcessWindowStyle.Hidden,
                                        Arguments = arg
                                    };

                using (var exeProcess = Process.Start(startInfo))
                {
                    exeProcess.WaitForExit();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
    }

    internal enum IconIndex
    {
        OK,
        Warning,
        Error
    }
}