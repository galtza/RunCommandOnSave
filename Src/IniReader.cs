using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;

namespace RunCommandOnSave
{
    public class IniFileReader
    {
        public Dictionary<string, Dictionary<string, string>> Sections = new Dictionary<string, Dictionary<string, string>>();
        public IniFileReader(string filePath)
        {
            string currentSection = null;
            foreach (var line in File.ReadAllLines(filePath))
            {
                var sectionMatch = _sectionRegex.Match(line);
                if (sectionMatch.Success)
                {
                    currentSection = sectionMatch.Groups[1].Value;
                    if (!Sections.ContainsKey(currentSection))
                    {
                        Sections[currentSection] = new Dictionary<string, string>();
                    }
                    continue;
                }

                var keyValueMatch = _keyValueRegex.Match(line);
                if (keyValueMatch.Success && currentSection != null)
                {
                    Sections[currentSection][keyValueMatch.Groups[1].Value] = keyValueMatch.Groups[2].Value;
                }
            }
        }

        private static readonly Regex _sectionRegex = new Regex(@"^\s*\[\s*([^]]+)\s*\]\s*$");
        private static readonly Regex _keyValueRegex = new Regex(@"^\s*([^=]+?)\s*=\s*(.*?)\s*$");

    }
}
