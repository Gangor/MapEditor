using MapEditor.Attributes;
using MapEditor.Events;
using MapEditor.Extends;
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

        private int x = 0;
        private int y = 0;

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
        
        [DisplayName("Map X")]
        [Category("Location")]
        public int X
        {
            get { return x; }
            set
            {
                Pvs.MapStartPosX = (byte)value;
                x = value;
            }
        }

        [DisplayName("Map Y")]
        [Category("Location")]
        public int Y
        {
            get { return y; }
            set
            {
                Pvs.MapStartPosY = (byte)value;
                y = value;
            }
        }
        
        /// <summary>
        /// Listing file of all directory
        /// </summary>
        public List<string> Mapping = new List<string>();

        /// <summary>
        /// Event Delegate
        /// </summary>
        public event EventHandler<PaintingArgs> Painting;

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
            Nfa.Blank();
            Nfc.Blank();
            Nfe.Blank();
            Nfl.Blank();
            Nfm.Blank();
            Nfp.Blank();
            Nfs.Blank();
            Nfw.Blank();
            Pvs.Blank((byte)X, (byte)Y);
            Qpf.Blank();

            Refresh();
        }

        /// <summary>
        /// Make new project
        /// </summary>
        public void New(string folder, string file)
        {
            XLog.WriteLine(Levels.Info, "New project map {0} on path {1}", file, folder);

            resolveName(file);
            Dispose();

            XLog.WriteLine(Levels.Info, "Map generated completed");
        }

        /// <summary>
        /// Load a existing project map
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="file"></param>
        public void Load(string folder, string file)
        {
            resolveName(file);
            
            Cfg.Load(folder + @"\cfg");
            Nfa.Load(folder + @"\nfa", $"{file}.nfa");
            Nfc.Load(folder + @"\nfc", $"{file}.nfc");
            Nfe.Load(folder + @"\nfe", $"{file}.nfe");
            Nfl.Load(folder + @"\nfl", $"{file}.nfl");
            Nfm.Load(folder + @"\nfm", $"{file}.nfm");
            Nfp.Load(folder + @"\nfp", $"{file}.nfp");
            Nfs.Load(folder + @"\nfs", $"{file}.nfs");
            Nfw.Load(folder + @"\nfw", $"{file}.nfw");
            Pvs.Load(folder + @"\pvs", $"{file}.pvs", (byte)X, (byte)Y);
            Qpf.Load(folder + @"\qpf", $"{file}.qpf");

            XLog.WriteLine(Levels.Info, "Map loading completed");

            Refresh();
        }

        /// <summary>
        /// Resolve current location
        /// </summary>
        /// <param name="name"></param>
        private void resolveName(string name)
        {
            var match = Regex.Matches(name, "[0-9]+");
            if (match.Count == 2)
            {
                X = int.Parse(match[0].Value);
                Y = int.Parse(match[1].Value);
            }
            else
            {
                X = 0;
                Y = 0;
            }
        }

        /// <summary>
        /// Save all file
        /// </summary>
        /// <param name="folder"></param>
        public void Save(string folder)
        {
            var filename = $"m{X.ToString("000")}_{Y.ToString("000")}";

            XLog.WriteLine(Levels.Info, "Saving map {0} on path {1}", filename, folder);

            Nfa.Save(folder + @"\nfa\", $"{filename}.nfa");
            Nfc.Save(folder + @"\nfc\", $"{filename}.nfc");
            Nfe.Save(folder + @"\nfe\", $"{filename}.nfe");
            Nfl.Save(folder + @"\nfl\", $"{filename}.nfl");
            Nfm.Save(folder + @"\nfm\", $"{filename}.nfm");
            Nfp.Save(folder + @"\nfp\", $"{filename}.nfp");
            Nfs.Save(folder + @"\nfs\", $"{filename}.nfs");
            Nfw.Save(folder + @"\nfw\", $"{filename}.nfw");
            Pvs.Save(folder + @"\pvs\", $"{filename}.pvs");
            Qpf.Save(folder + @"\qpf\", $"{filename}.qpf");

            XLog.WriteLine(Levels.Info, "Map saving completed");
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
        public void Watch(string path)
        {
            Mapping = new List<string>();
            WatchDirectory(path + @"\nfa\");
            WatchDirectory(path + @"\nfc\");
            WatchDirectory(path + @"\nfe\");
            WatchDirectory(path + @"\nfl\");
            WatchDirectory(path + @"\nfm\");
            WatchDirectory(path + @"\nfp\");
            WatchDirectory(path + @"\nfs\");
            WatchDirectory(path + @"\nfw\");
            WatchDirectory(path + @"\pvs\");
            WatchDirectory(path + @"\qpf\");
        }

        /// <summary>
        /// Watch directory for retrieves all file
        /// </summary>
        /// <param name="path"></param>
        private void WatchDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                var files = Directory.GetFiles(path, "m*_*", SearchOption.AllDirectories);
                foreach (var file in files)
                {
                    var onlyname = file.Split(new char[] { '\\' }).Last();
                    var name = onlyname.Split(new char[] { '.' })[0];
                    var match = Regex.Match(name, @"\bm([0-9]+)_([0-9]+)");
                    if (match.Success)
                    {
                        if (!Mapping.Contains(name)) Mapping.Add(name);
                    }
                }
            }
        }
    }
}
