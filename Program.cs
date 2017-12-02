using MapEditor.Dialog;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

namespace MapEditor
{
    static class Program
    {
        /// <summary>
        /// Point d'entrée principal de l'application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
		{

#if !DEBUG

			var uid = Environment.GetEnvironmentVariable("MAPEDITOR_REQUEST");

			if (args == null || args.Length < 1 || args[0] != uid)
			{

				try
				{
					Process.Start("Updater.exe");
					Environment.Exit(1);
				}
				catch { }
				finally
				{
					Environment.Exit(1);
				}
			}

			Environment.SetEnvironmentVariable("LAUNCHER_REQUEST", null);
#endif

			Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new Editor(args.Any(r => r == "upgrade")));
		}
    }
}
