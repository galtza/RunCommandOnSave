using System;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.Shell;
using Newtonsoft.Json.Linq;
using Microsoft.VisualStudio.OLE.Interop;

namespace RunCommandOnSave
{
    public class Settings
    {
        public bool Debug;
        public class Preset
        {
            public string[] Commands { get; set; }
            public string[] ExcludeExtensions { get; set; }
            public string[] ExcludePaths { get; set; }
        }
        public Dictionary<SaveEventType, Dictionary<string, Preset>> EventsConfig = new Dictionary<SaveEventType, Dictionary<string, Preset>>();

        public Settings(string settingsFilename)
        {
            _iniReader = new IniFileReader(settingsFilename);

            // Check if the debug is active

            Debug = _iniReader.Sections.ContainsKey("Debug") ? (_iniReader.Sections["Debug"].ContainsKey("On") ? Boolean.Parse(_iniReader.Sections["Debug"]["On"]) : false) : false;

            // Generate our dictionary

            foreach (var section in _iniReader.Sections)
            {
                var eventType = SaveEventType.Unknown;
                var isPure = section.Key == "PreSave" || section.Key == "PostSave";

                if (section.Key.StartsWith("PreSave"))
                {
                    eventType = SaveEventType.PreSave;
                }
                else if (section.Key.StartsWith("PostSave"))
                {
                    eventType = SaveEventType.PostSave;
                }

                if (eventType == SaveEventType.Unknown)
                {
                    continue;
                }

                Preset preset = new Preset();
                if (section.Value.ContainsKey("Commands"))
                {
                    preset.Commands = section.Value["Commands"].Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                }
                if (section.Value.ContainsKey("ExcludeExtensions"))
                {
                    preset.ExcludeExtensions = section.Value["ExcludeExtensions"].Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                }
                if (section.Value.ContainsKey("ExcludePaths"))
                {
                    preset.ExcludePaths = section.Value["ExcludePaths"].Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                }

                if (!EventsConfig.ContainsKey(eventType))
                {
                    EventsConfig[eventType] = new Dictionary<string, Preset>();
                }

                if (isPure)
                {
                    EventsConfig[eventType]["*"] = preset;
                }
                else
                {
                    var dotIndex = section.Key.IndexOf('.');
                    string extensions = section.Key.Substring(dotIndex + 1);
                    foreach (var ext in extensions.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        EventsConfig[eventType][ext] = preset;
                    }
                }
            }

        }

        /*
            ===============
            Private Stuff
            ===============
        */

        private IniFileReader _iniReader;

    }
}
