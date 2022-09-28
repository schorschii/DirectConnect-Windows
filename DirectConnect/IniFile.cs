using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace DirectConnect
{
    class IniFile
    {
        string Path;
        string EXE = Assembly.GetExecutingAssembly().GetName().Name;

        [DllImport("kernel32")]
        static extern long WritePrivateProfileString(string Section, string Key, string Value, string FilePath);

        [DllImport("kernel32")]
        static extern int GetPrivateProfileString(string Section, string Key, string Default, StringBuilder RetVal, int Size, string FilePath);

        [DllImport("kernel32", EntryPoint = "GetPrivateProfileSectionNamesA")]
        static extern int GetPrivateProfileSectionNames(byte[] lpszReturnBuffer, int nSize, string lpFileName);

        public IniFile(string IniPath = null)
        {
            Path = new FileInfo(IniPath ?? EXE + ".ini").FullName;
            Trace.WriteLine("Using config file " + Path);
            if (!File.Exists(Path))
            {
                throw new Exception("Config file does not exist!\n" + Path);
            }
        }

        public string[] GetSectionNames()
        {
            List<String> sections = new List<String>();
            byte[] buffer = new byte[1024];
            GetPrivateProfileSectionNames(buffer, buffer.Length, Path);
            string allSections = Encoding.UTF8.GetString(buffer);
            string[] sectionNames = allSections.Split('\0');
            foreach (string sectionName in sectionNames)
            {
                if (sectionName != string.Empty)
                    sections.Add(sectionName);
            }
            return sections.ToArray();
        }

        public string Read(string Key, string Section = null)
        {
            var RetVal = new StringBuilder(255);
            GetPrivateProfileString(sectionEncoding(Section) ?? EXE, Key, "", RetVal, 255, Path);
            return RetVal.ToString();
        }

        public void Write(string Key, string Value, string Section = null)
        {
            WritePrivateProfileString(sectionEncoding(Section) ?? EXE, Key, Value, Path);
        }

        public void DeleteKey(string Key, string Section = null)
        {
            Write(Key, null, sectionEncoding(Section) ?? EXE);
        }

        public void DeleteSection(string Section = null)
        {
            Write(null, null, sectionEncoding(Section) ?? EXE);
        }

        public bool KeyExists(string Key, string Section = null)
        {
            return Read(Key, sectionEncoding(Section)).Length > 0;
        }

        private string sectionEncoding(string text)
        {
            if (text != null)
            {
                byte[] bytes = Encoding.UTF8.GetBytes(text);
                text = Encoding.Default.GetString(bytes);
            }
            return text;
        }
    }
}