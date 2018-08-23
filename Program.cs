using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace JDP
{
    internal static class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            string fileName;
            if (args.Length > 0)
                fileName = args[0];
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmMain());
        }

        public static void SetFontAndScaling(Form form)
        {
            form.SuspendLayout();
            int platformId = (int) Environment.OSVersion.Platform;
            //Detect Unix
            if ((platformId == 4) || (platformId == 128)) {
                Regex UnixDefaultFontRegexp = new Regex(@"^'(.*) \d*'$", RegexOptions.Compiled);
                string DefaultFont = UnixDefaultFontRegexp.Match(ExecuteBashCommand("gsettings get org.gnome.desktop.interface font-name")).Groups[1].ToString();
                //MessageBox.Show(DefaultFont);
                if (DefaultFont != "")
                    form.Font = new Font(DefaultFont, 8.25f);
                else
                    form.Font = new Font("Tahoma", 8.25f);
            }
            /*
            //macos tobedone
            else if (platformId == 6)
            {
                
            }
            */
            else
            {
                form.Font = new Font("Tahoma", 8.25f);
                if (form.Font.Name != "Tahoma") form.Font = new Font("Arial", 8.25f);
            }
            form.AutoScaleMode = AutoScaleMode.Font;
            form.AutoScaleDimensions = new SizeF(6f, 13f);
            form.ResumeLayout(false);
        }
        
        
        private static string ExecuteBashCommand(string command)
        {
            // according to: https://stackoverflow.com/a/15262019/637142
            // thans to this we will pass everything as one command
            command = command.Replace("\"","\"\"");
            
            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = "-c \""+ command + "\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };
        
            proc.Start();
            proc.WaitForExit();

            return proc.StandardOutput.ReadToEnd();
        }
    }
}
