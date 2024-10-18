using System;
using System.Collections.Generic;
using System.IO;
using EnvDTE;
using Microsoft.VisualStudio.Shell;

namespace RunCommandOnSave
{
    public class SettingsCache
    {
        private Dictionary<string, (Settings settings, DateTime lastReadTime, long lastFileSize)> _cache = new Dictionary<string, (Settings, DateTime, long)>();

        public Settings GetSettingsForDocument(Document document)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            FileInfo settingsFile = LocateSettingsFor(document.FullName);
            if (settingsFile == null)
            {
                return null;
            }

            DateTime lastWriteTime = settingsFile.LastWriteTime;
            long fileSize = settingsFile.Length;

            if (_cache.TryGetValue(settingsFile.FullName, out var cacheEntry))
            {
                if (cacheEntry.lastReadTime != lastWriteTime || cacheEntry.lastFileSize != fileSize)
                {
                    var settings = new Settings(settingsFile.FullName);
                    _cache[settingsFile.FullName] = (settings, lastWriteTime, fileSize);
                    return settings;
                }
                return cacheEntry.settings;  // Return cached settings if up-to-date
            }
            else
            {
                var settings = new Settings(settingsFile.FullName);
                _cache[settingsFile.FullName] = (settings, lastWriteTime, fileSize);
                return settings;
            }
        }

        private FileInfo LocateSettingsFor(string documentFullName)
        {
            DirectoryInfo dir = new DirectoryInfo(Path.GetDirectoryName(documentFullName));
            while (dir != null)
            {
                FileInfo[] configs = dir.GetFiles(".runcommandonsave");
                if (configs.Length > 0)
                {
                    return configs[0];
                }
                dir = dir.Parent;
            }
            return null;
        }
    }
}
