using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

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
            string firstLine;
            string DefaultFontName = "Tahoma";
            string FontConf = Application.StartupPath + System.IO.Path.DirectorySeparatorChar + "font.txt";
            if (!File.Exists(FontConf))
            {
                using (StreamWriter writer = new StreamWriter(FontConf))
                {
                    writer.Write(DefaultFontName);
                }
            }
            if (File.Exists(FontConf))
            {
              using (StreamReader reader = new StreamReader(FontConf))
              {
                  firstLine = reader.ReadLine() ?? "";
              }
              if (firstLine!="")
                  DefaultFontName = firstLine;
            }
            form.Font = new Font(DefaultFontName, 8.25f);
            if (form.Font.Name != DefaultFontName) form.Font = new Font("Arial", 8.25f);
            form.AutoScaleMode = AutoScaleMode.Font;
            form.AutoScaleDimensions = new SizeF(6f, 13f);
            form.ResumeLayout(false);
        }
    }
}