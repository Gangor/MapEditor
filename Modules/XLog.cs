using System.Drawing;

namespace MapEditor.Modules
{
    public class XLog
    {
        public static Levels Level = Levels.Debug;

		/// <summary>
		/// Write text in console
		/// </summary>
		/// <param name="level"></param>
		/// <param name="l"></param>
		/// <param name="args"></param>
		public static void Write(Levels level, string l, params object[] args)
		{
			if (Level <= level)
			{
				var color = new Color();
				var text = string.Format("{0}", string.Format(l, args));

				switch (level)
				{
					case Levels.Debug:
						color = Color.LightGray;
						break;
					case Levels.Info:
						color = Color.White;
						break;
					case Levels.Good:
						color = Color.DarkGreen;
						break;
					case Levels.Trace:
						color = Color.White;
						break;
					case Levels.Warning:
						color = Color.Yellow;
						break;
					case Levels.Fatal:
						color = Color.Red;
						break;
					case Levels.Error:
						color = Color.DarkRed;
						break;
				}

				Editor.Instance.InsertLog(level, text, color);
			}
		}

		/// <summary>
		/// Write line in console
		/// </summary>
		/// <param name="level"></param>
		/// <param name="l"></param>
		/// <param name="args"></param>
		public static void WriteLine(Levels level, string l, params object[] args) { Write(level, string.Format("{0}\n", new object[] { string.Format(l, args) })); }
	}

    public enum Levels
    {
        Trace,
        Debug,
		Good,
		Info,
        Warning,
        Error,
        Fatal,
    }
}
