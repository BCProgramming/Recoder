using System;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Reflection;
using Un4seen.Bass;
using Un4seen.Bass.Misc;
using System.Windows.Forms;

namespace Recoder
{
    internal class Program
    {
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        ///For DPI Awareness.
        private static extern bool SetProcessDPIAware();

        [STAThread]
        public static void Main(String[] args)
        {
            SetProcessDPIAware();
            bool BassResult = Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);
            String CurrentFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Bass.BASS_PluginLoadDirectory(CurrentFolder);
            Application.SetCompatibleTextRenderingDefault(true);
            Application.EnableVisualStyles();
            Application.Run(new frmRecoder());
        }
    }
}