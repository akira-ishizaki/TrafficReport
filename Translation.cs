﻿using ColossalFramework.Globalization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace TrafficReport
{
    public class Translation
    {
        const string PREFIX_FILENAME = "TrafficReport.Resources.";

        private static Dictionary<string, string> textDictionary;

        public static string GetString(string key)
        {
            if (textDictionary == null)
            {
                string filename = PREFIX_FILENAME + GetTranslatedFileName("text.txt");
                string[] lines;
                using (Stream st = Assembly.GetExecutingAssembly().GetManifestResourceStream(filename))
                {
                    using (StreamReader sr = new StreamReader(st))
                    {
                        lines = sr.ReadToEnd().Split(new string[] { "\n", "\r\n" }, StringSplitOptions.None);
                    }
                }
                textDictionary = new Dictionary<string, string>();
                foreach (string line in lines)
                {
                    if (line == null || line.Trim().Length == 0)
                    {
                        continue;
                    }
                    int delimiterIndex = line.Trim().IndexOf('\t');
                    if (delimiterIndex > 0)
                    {
                        textDictionary.Add(line.Substring(0, delimiterIndex), line.Substring(delimiterIndex + 1).Trim().Replace("\\n", "\n"));
                    }
                }
            }
            if (textDictionary.ContainsKey(key))
            {
                return textDictionary[key];
            }
            else
            {
                return key;
            }
        }

        public static string GetTranslatedFileName(string filename)
        {
            string language = LocaleManager.instance.language;
            switch (language)
            {
                case null:
                    language = "en";
                    break;
                case "jaex":
                    language = "ja";
                    break;
            }
            int delimiterIndex = filename.Trim().LastIndexOf('.');
            string translated_filename = filename.Substring(0, delimiterIndex) + "_" + language.Trim().ToLower() + filename.Substring(delimiterIndex);
            if (Assembly.GetExecutingAssembly().GetManifestResourceNames().Contains(PREFIX_FILENAME + translated_filename))
            {
                return translated_filename;
            }
            else
            {
                return filename;
            }
        }

        public static void Release()
        {
            textDictionary = null;
        }
    }
}
