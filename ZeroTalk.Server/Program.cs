using System;
using System.Windows.Forms;

namespace ZeroTalk.Server
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            bool autoStart = args != null && args.Length > 0 && 
                Array.Exists(args, a => a.Equals("autostart", StringComparison.OrdinalIgnoreCase));

            Application.Run(new Forms.frmServer(autoStart));
        }
    }
}
