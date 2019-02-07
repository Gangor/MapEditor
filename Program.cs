using MapEditor.Dialog;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
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
            AppDomain.CurrentDomain.AssemblyResolve += (sender, arg) =>
            {

                Assembly thisAssembly = Assembly.GetExecutingAssembly();

                //Load form Embedded Resources - This Function is not called if the Assembly is in the Application Folder
                var resources = thisAssembly.GetManifestResourceNames().Where(s => s.EndsWith(".dll"));
                if (resources.Count() > 0)
                {
                    var resourceName = resources.First();
                    using (Stream stream = thisAssembly.GetManifestResourceStream(resourceName))
                    {
                        if (stream == null) return null;
                        var block = new byte[stream.Length];
                        stream.Read(block, 0, block.Length);
                        return Assembly.Load(block);
                    }
                }
                return null;
            };

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new Editor(args.Any(r => r == "upgrade")));
		}
    }
}
