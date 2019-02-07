using DataCore;
using MapEditor.Attributes;
using MapEditor.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace MapEditor.Modules
{
	/// <summary>
	/// Global manager for all type of the file
	/// </summary>
	public class MapWorker
	{
		public static MapWorker Instance = null;

		private byte x = 0;
		private byte y = 0;

		/// <summary>
		/// Variable for manage file
		/// </summary>
		#region Manager map file
        
		[DisplayName("Cfg")]
		[PropertyGridBrowsable(true)]
		[TypeConverter(typeof(ExpandableObjectConverter))]
		public CfgManager Cfg { get; set; } = new CfgManager();

		[DisplayName("Nfa")]
		[PropertyGridBrowsable(true)]
		[TypeConverter(typeof(ExpandableObjectConverter))]
		public NfaManager Nfa { get; set; } = new NfaManager();

		[DisplayName("Nfc")]
		[PropertyGridBrowsable(true)]
		[TypeConverter(typeof(ExpandableObjectConverter))]
		public NfcManager Nfc { get; set; } = new NfcManager();

		[DisplayName("Nfe")]
		[PropertyGridBrowsable(true)]
		[TypeConverter(typeof(ExpandableObjectConverter))]
		public NfeManager Nfe { get; set; } = new NfeManager();

		[DisplayName("Nfl")]
		[PropertyGridBrowsable(true)]
		[TypeConverter(typeof(ExpandableObjectConverter))]
		public NflManager Nfl { get; set; } = new NflManager();

		[DisplayName("Nfm")]
		[PropertyGridBrowsable(true)]
		[TypeConverter(typeof(ExpandableObjectConverter))]
		public NfmManager Nfm { get; set; } = new NfmManager();

		[DisplayName("Nfp")]
		[PropertyGridBrowsable(true)]
		[TypeConverter(typeof(ExpandableObjectConverter))]
		public NfpManager Nfp { get; set; } = new NfpManager();

		[DisplayName("Nfs")]
		[PropertyGridBrowsable(true)]
		[TypeConverter(typeof(ExpandableObjectConverter))]
		public NfsManager Nfs { get; set; } = new NfsManager();

		[DisplayName("Nfw")]
		[PropertyGridBrowsable(true)]
		[TypeConverter(typeof(ExpandableObjectConverter))]
		public NfwManager Nfw { get; set; } = new NfwManager();

		[DisplayName("Qpf")]
		[PropertyGridBrowsable(true)]
		[TypeConverter(typeof(ExpandableObjectConverter))]
		public QpfManager Qpf { get; set; } = new QpfManager();

		[DisplayName("Pvs")]
		[PropertyGridBrowsable(true)]
		[TypeConverter(typeof(ExpandableObjectConverter))]
		public PvsManager Pvs { get; set; } = new PvsManager();

		#endregion
        
		[DisplayName("X")]
		[Category("Map")]
		[PropertyGridBrowsable(true)]
		public byte X
		{
			get { return x; }
			set
			{
				Pvs.MapStartPosX = (byte)value;
				x = value;
			}
		}

		[DisplayName("Y")]
		[Category("Map")]
		[PropertyGridBrowsable(true)]
		public byte Y
		{
			get { return y; }
			set
			{
				Pvs.MapStartPosY = (byte)value;
				y = value;
			}
		}

		/// <summary>
		/// Event when painting
		/// </summary>
		public event EventHandler<PaintingArgs> Painting;

		/// <summary>
		/// Event when load
		/// </summary>
		public event EventHandler<EventArgs> Reset;

		#region Constructor

		/// <summary>
		/// Initialize a new instance
		/// </summary>
		protected MapWorker(string folder)
		{
			///Collision file
			Nfa.Added += (o, e) => Refresh();
			Nfa.Painting += (o, e) => Refresh(false);
			Nfa.Removed += (o, e) => Refresh();
            
			///Region file
			Nfc.Added += (o, e) => Refresh();
			Nfc.Painting += (o, e) => Refresh(false);
			Nfc.Removed += (o, e) => Refresh();

			///Area file
			Nfe.Added += (o, e) => Refresh();
			Nfe.Painting += (o, e) => Refresh(false);
			Nfe.Removed += (o, e) => Refresh();

			///Light file
			Nfl.Added += (o, e) => Refresh();
			Nfl.Painting += (o, e) => Refresh(false);
			Nfl.Removed += (o, e) => Refresh();

			///Unknown file
			Nfp.Added += (o, e) => Refresh();
			Nfp.Painting += (o, e) => Refresh(false);
			Nfp.Removed += (o, e) => Refresh();

			///Script file
			Nfs.AddedLocation += (o, e) => Refresh();
			Nfs.AddedPropScript += (o, e) => Refresh();
			Nfs.Painting += (o, e) => Refresh(false);
			Nfs.Removed += (o, e) => Refresh();

			///Water file
			Nfw.Added += (o, e) => Refresh();
			Nfw.Painting += (o, e) => Refresh(false);
			Nfw.Removed += (o, e) => Refresh();

			///Quest prop file
			Qpf.Added += (o, e) => Refresh();
			Qpf.Painting += (o, e) => Refresh(false);
			Qpf.Removed += (o, e) => Refresh();
		}

		/// <summary>
		/// Initialize singleton instance
		/// </summary>
		/// <returns></returns>
		public static bool Init(string folder)
		{
			if (Instance == null)
			{
				Instance = new MapWorker(folder);
				return true;
			}

			return false;
		}

		/// <summary>
		/// Destroy singleton instance
		/// </summary>
		/// <returns></returns>
		public static bool DeInit()
		{
			if (Instance != null)
			{
				Instance = null;
				return true;
			}

			return false;
		}

		#endregion

		/// <summary>
		/// Dispose mapping
		/// </summary>
		public void Dispose()
		{
			Reset?.Invoke(this, EventArgs.Empty);

			Nfa.Blank();
			Nfc.Blank();
			Nfe.Blank();
			Nfl.Blank();
			Nfm.Blank();
			Nfp.Blank();
			Nfs.Blank();
			Nfw.Blank();
			Pvs.Blank();
			Qpf.Blank();
			
			Refresh();
		}

		/// <summary>
		/// Saving all file from data
		/// </summary>
		/// <param name="core"></param>
		/// <param name="folder"></param>
		public void Export(Core core, string folder)
		{
			var filename = $"m{X.ToString("000")}_{Y.ToString("000")}";

			XLog.WriteLine(Levels.Info, "Saving map {0} from DataCore v4...", filename);

			Export(core, $"{filename}.nfa", new Func<byte[]>(Nfa.GetBuffer));
			Export(core, $"{filename}.nfc", new Func<byte[]>(Nfc.GetBuffer));
			Export(core, $"{filename}.nfe", new Func<byte[]>(Nfe.GetBuffer));
			Export(core, $"{filename}.nfl", new Func<byte[]>(Nfl.GetBuffer));
			Export(core, $"{filename}.nfm", new Func<byte[]>(Nfm.GetBuffer));
			Export(core, $"{filename}.nfp", new Func<byte[]>(Nfp.GetBuffer));
			Export(core, $"{filename}.nfs", new Func<byte[]>(Nfs.GetBuffer));
			Export(core, $"{filename}.nfw", new Func<byte[]>(Nfw.GetBuffer));
			Export(core, $"{filename}.pvs", new Func<byte[]>(Pvs.GetBuffer));
			Export(core, $"{filename}.qpf", new Func<byte[]>(Qpf.GetBuffer));
			core.Save();

			XLog.WriteLine(Levels.Info, "Map saving completed.");
		}

		/// <summary>
		/// Export file one by one from data
		/// </summary>
		/// <param name="core"></param>
		/// <param name="filename"></param>
		/// <param name="action"></param>
		private void Export(Core core, string filename, Func<byte[]> action)
		{
			XLog.Write(Levels.Info, $"Saving {filename}...\t");

			core.ImportFileEntry(filename, action());
		}

		/// <summary>
		/// Load a existing project map by data
		/// </summary>
		/// <param name="core"></param>
		/// <param name="folder"></param>
		/// <param name="file"></param>
		public void Import(Core core, string file, string encoding)
		{
			Dispose();
			ResolveName(file);

			XLog.WriteLine(Levels.Info, "Loading map {0} from DataCore v4...", file);

			Import(core, $"terrainpropinfo{encoding}.cfg", new Action<byte[]>(Cfg.LoadProp));
			Import(core, $"terraintextureinfo{encoding}.cfg", new Action<byte[]>(Cfg.LoadTexture));
			Import(core, $"{file}{encoding}.nfa", new Action<byte[]>(Nfa.Load));
			Import(core, $"{file}{encoding}.nfc", new Action<byte[]>(Nfc.Load));
			Import(core, $"{file}{encoding}.nfe", new Action<byte[]>(Nfe.Load));
			Import(core, $"{file}{encoding}.nfl", new Action<byte[]>(Nfl.Load));
			Import(core, $"{file}{encoding}.nfm", new Action<byte[]>(Nfm.Load));
			Import(core, $"{file}{encoding}.nfp", new Action<byte[]>(Nfp.Load));
			Import(core, $"{file}{encoding}.nfs", new Action<byte[]>(Nfs.Load));
			Import(core, $"{file}{encoding}.nfw", new Action<byte[]>(Nfw.Load));
			Import(core, $"{file}{encoding}.pvs", new Action<byte[]>(Pvs.Load));
			Import(core, $"{file}{encoding}.qpf", new Action<byte[]>(Qpf.Load));

			XLog.WriteLine(Levels.Info, "Map loading completed.");
		}

		/// <summary>
		/// Load file one by one by data
		/// </summary>
		/// <param name="core"></param>
		/// <param name="filename"></param>
		/// <param name="action"></param>
		private void Import(Core core, string filename, Action<byte[]> action)
		{
			XLog.Write(Levels.Info, $"Loading {filename}...\t");

			if (!core.GetEntryExists(filename))
			{
				XLog.WriteLine(Levels.Warning, "not found");
				return;
			}

			action(core.GetFileBytes(filename));
		}

		/// <summary>
		/// Load a existing project map by file
		/// </summary>
		/// <param name="folder"></param>
		/// <param name="file"></param>
		public void Load(string folder, string file, string encoding)
		{
			Dispose();
			ResolveName(file);

			XLog.WriteLine(Levels.Info, "Loading map {0} from path {1}...", file, folder);
            
			Cfg.Open(folder + @"\cfg");
			Load(folder + @"\nfa\", $"{file}{encoding}.nfa", new Action<byte[]>(Nfa.Load));
			Load(folder + @"\nfc\", $"{file}{encoding}.nfc", new Action<byte[]>(Nfc.Load));
			Load(folder + @"\nfe\", $"{file}{encoding}.nfe", new Action<byte[]>(Nfe.Load));
			Load(folder + @"\nfl\", $"{file}{encoding}.nfl", new Action<byte[]>(Nfl.Load));
			Load(folder + @"\nfm\", $"{file}{encoding}.nfm", new Action<byte[]>(Nfm.Load));
			Load(folder + @"\nfp\", $"{file}{encoding}.nfp", new Action<byte[]>(Nfp.Load));
			Load(folder + @"\nfs\", $"{file}{encoding}.nfs", new Action<byte[]>(Nfs.Load));
			Load(folder + @"\nfw\", $"{file}{encoding}.nfw", new Action<byte[]>(Nfw.Load));
			Load(folder + @"\pvs\", $"{file}{encoding}.pvs", new Action<byte[]>(Pvs.Load));
			Load(folder + @"\qpf\", $"{file}{encoding}.qpf", new Action<byte[]>(Qpf.Load));
			
			XLog.WriteLine(Levels.Info, "Map loading completed.");
		}

		/// <summary>
		/// Load file one by one
		/// </summary>
		/// <param name="path"></param>
		/// <param name="filename"></param>
		/// <param name="action"></param>
		private void Load(string path, string filename, Action<byte[]> action)
		{
			XLog.Write(Levels.Info, $"Loading {filename}...\t");

			var fullname = Path.Combine(path, filename);

			if (!File.Exists(fullname))
			{
				XLog.WriteLine(Levels.Warning, "not found");
				return;
			}

			action(File.ReadAllBytes(fullname));
		}

		/// <summary>
		/// Make new project
		/// </summary>
		public void New(string folder, string file)
		{
			Dispose();
			ResolveName(file);

			XLog.WriteLine(Levels.Info, "New project map {0} from path {1}.", file, folder);
			XLog.WriteLine(Levels.Info, "Map create completed.");
		}

		/// <summary>
		/// Resolve current location
		/// </summary>
		/// <param name="name"></param>
		private void ResolveName(string name)
		{
			var match = Regex.Matches(name, "[0-9]+");
			if (match.Count == 2)
			{
				X = byte.Parse(match[0].Value);
				Y = byte.Parse(match[1].Value);
			}
			else
			{
				X = 0;
				Y = 0;
			}
		}

		/// <summary>
		/// Resolve encoding file 
		/// </summary>
		/// 
		/// <example>
		/// m012_000(ascii)
		/// </example>
		/// <param name="name"></param>
		/// <returns></returns>
		public static (string, string) ResolveEncode(string name)
		{
			var realname = name;
			var codepage = string.Empty;

			var match = Regex.Match(name, @"(\(.*?\))");
			if (match.Success)
			{
				codepage = match.Groups[1].Value;
				realname = name.Replace(codepage, string.Empty);
			}
			return (realname, codepage);
		}

		/// <summary>
		/// Save all file
		/// </summary>
		/// <param name="folder"></param>
		public void Save(string folder)
		{
			var filename = $"m{X.ToString("000")}_{Y.ToString("000")}";

			XLog.WriteLine(Levels.Info, "Saving map {0} on path {1}...", filename, folder);

			Save(folder + @"\nfa\", $"{filename}.nfa", new Func<byte[]>(Nfa.GetBuffer));
			Save(folder + @"\nfc\", $"{filename}.nfc", new Func<byte[]>(Nfc.GetBuffer));
			Save(folder + @"\nfe\", $"{filename}.nfe", new Func<byte[]>(Nfe.GetBuffer));
			Save(folder + @"\nfl\", $"{filename}.nfl", new Func<byte[]>(Nfl.GetBuffer));
			Save(folder + @"\nfm\", $"{filename}.nfm", new Func<byte[]>(Nfm.GetBuffer));
			Save(folder + @"\nfp\", $"{filename}.nfp", new Func<byte[]>(Nfp.GetBuffer));
			Save(folder + @"\nfs\", $"{filename}.nfs", new Func<byte[]>(Nfs.GetBuffer));
			Save(folder + @"\nfw\", $"{filename}.nfw", new Func<byte[]>(Nfw.GetBuffer));
			Save(folder + @"\pvs\", $"{filename}.pvs", new Func<byte[]>(Pvs.GetBuffer));
			Save(folder + @"\qpf\", $"{filename}.qpf", new Func<byte[]>(Qpf.GetBuffer));

			XLog.WriteLine(Levels.Info, "Map saving completed.");
		}

		/// <summary>
		/// Save file one by one
		/// </summary>
		/// <param name="path"></param>
		/// <param name="filename"></param>
		/// <param name="action"></param>
		private void Save(string path, string filename, Func<byte[]> action)
		{
			var fullname = Path.Combine(path, filename);

			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}

			XLog.Write(Levels.Info, $"Saving {filename}...\t");
			File.WriteAllBytes(fullname, action());
		}

		/// <summary>
		/// Refresh all element in the map
		/// </summary>
		/// <param name="changed"></param>
		public void Refresh(bool changed = true)
		{
			Painting?.Invoke(this, new PaintingArgs(
				nfa: Nfa,
				nfc: Nfc,
				nfe: Nfe,
				nfl: Nfl,
				nfp: Nfp,
				nfs: Nfs,
				nfw: Nfw,
				pvs: Pvs,
				qpf: Qpf,
				changed: changed
			));
		}

		/// <summary>
		/// Watch directory for retrieves all file
		/// </summary>
		/// <param name="path"></param>
		public static List<string> Watch(string path)
		{
			var mapping = new List<string>();
			var directory = Path.Combine(path, "nfm");

			if (Directory.Exists(directory))
			{
				var files = Directory.GetFiles(directory, "m*_*", SearchOption.AllDirectories);
				foreach (var file in files)
				{
					var name = Path.GetFileNameWithoutExtension(file);
					var match = Regex.Match(name, @"\bm([0-9]+)_([0-9]+)");
					if (match.Success)
					{
						if (!mapping.Contains(name)) mapping.Add(name);
					}
				}
			}
			return mapping.OrderBy(r => r).ToList();
		}

		/// <summary>
		/// Watch data for retieves all file
		/// </summary>
		/// <param name="core"></param>
		/// <returns></returns>
		public static List<string> Watch(Core core)
		{
			var mapping = new List<string>();
			var entries = core.GetEntriesByExtension("nfm");

			foreach (var file in entries)
			{
				var name = Path.GetFileNameWithoutExtension(file.Name);
				var match = Regex.Match(name, @"\bm([0-9]+)_([0-9]+)");
				if (match.Success)
				{
					if (!mapping.Contains(name)) mapping.Add(name);
				}
			}
			return mapping.OrderBy(r => r).ToList();
		}
	}
}
