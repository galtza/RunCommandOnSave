using System;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using System.Linq;
using Microsoft.VisualStudio.Shell;

namespace RunOnSave
{
    public class Settings
    {
        public Settings(string docFullName)
        {
            SettingsFilename = LocateSettings(Path.GetDirectoryName(docFullName));
            if (SettingsFilename == null)
            {
                throw new ArgumentException("No Settings found for this file", docFullName);
            }
        }

        public string ReadKey(string section, string key)
        {
            var Temp = new StringBuilder(256);
            var NumberOfChars = GetPrivateProfileString(section, key, null, Temp, Temp.Capacity, SettingsFilename);
            return NumberOfChars == 0 ? null : Temp.ToString();
        }

        public string[] GetCommand(string docFullName, string action)
        {
            var CommandsRaw = ReadKey(action, "Commands");
            if (CommandsRaw == null)
            {
                return null;
            }
            var Commands = CommandsRaw.Split(new char[] { '|' }, System.StringSplitOptions.RemoveEmptyEntries);
            if (Commands.Length == 0)
            {
                return null;
            }

            // Check excluded paths (They can be relative or absolute separated by |)

            var ExcludedPathsRaw = ReadKey(action, "ExcludePaths");
            if (ExcludedPathsRaw != null)
            {
                var BasePath = Path.GetDirectoryName(SettingsFilename);
                var ExcludedPathsRawList = ExcludedPathsRaw.Split(new char[] { '|' }, System.StringSplitOptions.RemoveEmptyEntries);
                var ExcludedPaths = ExcludedPathsRawList.Select(s => new FileInfo(Path.Combine(BasePath, s.Replace("/", "\\").Replace("\"", ""))).FullName);
                try
                {
                    foreach (string Path in ExcludedPaths)
                    {
                        if (docFullName.StartsWith(Path))
                        {
                            return null;
                        }
                    }
                }
                catch (Exception)
                {
                }
            }

            // Check excluded extensions (Separated by |)

            var ExcludedExtensionsRaw = ReadKey(action, "ExcludeExtensions");
            if (ExcludedExtensionsRaw != null)
            {
                var ExcludedExtensions = ExcludedExtensionsRaw.Split(new char[] { '|' }, System.StringSplitOptions.RemoveEmptyEntries).Select(s => s.TrimSuffix("*"));
                try
                {
                    foreach (string Ext in ExcludedExtensions)
                    {
                        if (docFullName.EndsWith(Ext))
                        {
                            return null;
                        }
                    }
                }
                catch (Exception)
                {
                }
            }

            return Commands;
        }

        /*
            ===============
            Private Stuff
            ===============
        */

        private string SettingsFilename = null;

        private string LocateSettings(string startingPath)
        {
            var cfgFile = new FileInfo(Path.Combine(startingPath, ".runonsave"));
            var dir = new DirectoryInfo(startingPath);
            while (!cfgFile.Exists && dir.Parent != null)
            {
                var configs = dir.GetFiles(".runonsave");
                if (configs.Length > 0)
                {
                    cfgFile = configs[0];
                    break;
                }
                dir = dir.Parent;
            }
            return cfgFile.Exists ? cfgFile.FullName : null;
        }

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);
    }
}
