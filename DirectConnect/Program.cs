using System;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;

namespace DirectConnect
{
    static class Program
    {
        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // testing other languages
            //Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en-US");

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmConnect());
        }
    }
}
