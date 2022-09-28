using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace DirectConnect
{
    public partial class frmConnect : Form
    {

        IniFile iniFile;

        public frmConnect()
        {
            InitializeComponent();
            this.AcceptButton = btnConnect;
            this.Text += " " + Assembly.GetEntryAssembly().GetName().Version.ToString();
            btnConnect.Text = Properties.strings.connect;
            lblAddress.Text = Properties.strings.address+":";
            lblProfile.Text = Properties.strings.profile+":";

            try
            {
                iniFile = new IniFile();
                string[] sections = iniFile.GetSectionNames();
                if(sections.Length == 0)
                {
                    throw new Exception(Properties.strings.no_profiles_found);
                }
                foreach (string s in sections)
                {
                    cbProfile.Items.Add(s);
                }
                cbProfile.SelectedIndex = 0;
            } catch(Exception e)
            {
                MessageBox.Show(this, e.Message, Properties.strings.error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Load += (s1, e1) => Close();
                return;
            }
        }

        private string escapeCommandLineArg(string arg)
        {
            return "\"" + Regex.Replace(arg, @"(\\+)$", @"$1$1") + "\"";
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            frmAbout about = new frmAbout();
            about.ShowDialog(this);
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            string address = txtAddress.Text;
            string exePath = iniFile.Read("executable", cbProfile.Text);
            string arguments = iniFile.Read("arguments", cbProfile.Text).Replace("%HOSTNAME%", escapeCommandLineArg(address));

            if (address.Trim() == "")
            {
                MessageBox.Show(this, Properties.strings.missing_target_address, Properties.strings.error, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!File.Exists(exePath))
            {
                MessageBox.Show(this, Properties.strings.executable_could_not_be_found + "\n" + exePath, Properties.strings.error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Process process = new Process();
            process.StartInfo.FileName = exePath;
            process.StartInfo.Arguments = arguments;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
            process.Start();
#if DEBUG
            Trace.WriteLine("Starting command line: " + exePath + " " + arguments);
#endif

            btnConnect.Text = Properties.strings.connecting;
            btnConnect.Enabled = false;
            process.WaitForExit();

            btnConnect.Text = Properties.strings.connect;
            btnConnect.Enabled = true;
        }
    }
}
