// ****************************************************************************
//
// FLV Extract
// Copyright (C) 2006-2011  J.D. Purcell (moitah@yahoo.com)
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//
// ****************************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JDP
{
    internal static class SettingsShared
    {
        public static string GetMyAppDataDir(string appName)
        {
            string appDataDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string myAppDataDir = Path.Combine(appDataDir, appName);

            if (Directory.Exists(myAppDataDir) == false)
            {
                Directory.CreateDirectory(myAppDataDir);
            }

            return myAppDataDir;
        }
    }

    internal class SettingsReader
    {
        private Dictionary<string, string> _settings;

        public SettingsReader(string appName, string fileName)
        {
            _settings = new Dictionary<string, string>();

            string path = Path.Combine(SettingsShared.GetMyAppDataDir(appName), fileName);
            if (!File.Exists(path))
            {
                return;
            }

            using (StreamReader sr = new StreamReader(path, Encoding.UTF8))
            {
                string line;
                string val;

                while ((line = sr.ReadLine()) != null)
                {
                    int pos = line.IndexOf('=');
                    if (pos != -1)
                    {
                        string name = line.Substring(0, pos);
                        val = line.Substring(pos + 1);

                        if (!_settings.ContainsKey(name))
                        {
                            _settings.Add(name, val);
                        }
                    }
                }
            }
        }

        public string Load(string name)
        {
            return _settings.ContainsKey(name) ? _settings[name] : null;
        }
    }

    internal class SettingsWriter
    {
        private StreamWriter _sw;

        public SettingsWriter(string appName, string fileName)
        {
            string path = Path.Combine(SettingsShared.GetMyAppDataDir(appName), fileName);

            _sw = new StreamWriter(path, false, Encoding.UTF8);
        }

        public void Save(string name, string value)
        {
            _sw.WriteLine(name + "=" + value);
        }

        public void Close()
        {
            _sw.Close();
        }
    }
}