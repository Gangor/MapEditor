using MapEditor.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MapEditor.Modules
{
    public class QpfManager : Qpf
    {
        public uint[] SupportedVersion = { 1, 3 };

        public event EventHandler<Prop> Added;
        public event EventHandler<EventArgs> Painting;
        public event EventHandler<EventArgs> Removed;

        /// <summary>
        /// Add new prop
        /// </summary>
        /// <param name="location"></param>
        public void AddLocation(Prop prop)
        {
            Props.Add(prop);

            Added?.Invoke(this, prop);
        }

        /// <summary>
        /// Reinitilize child objet
        /// </summary>
        public void Blank()
        {
            Props = new List<Prop>();
        }

        /// <summary>
        /// Load existing nflavor script file
        /// </summary>
        /// <param name="filename"></param>
        public void Load(string path, string filename)
        {
            Blank();

            if (!File.Exists(Path.Combine(path, filename)))
            {
                XLog.WriteLine(Levels.Warning, $"Missing file {filename}.");
                return;
            }

            XLog.WriteLine(Levels.Info, $"Loading {filename}...");
            try
            {
                using (BinaryReader b = new BinaryReader(File.Open(Path.Combine(path, filename), FileMode.Open)))
                {
                    Sign = Encoding.Default.GetString(b.ReadBytes(18));
                    Version = b.ReadUInt32();

                    XLog.WriteLine(Levels.Debug, $"QPF version {Version}.");

#if DEBUG == false
                    if (!SupportedVersion.Contains(Version))
                    {
                        XLog.WriteLine(Levels.Fatal, "QpfManager::Load<Version> -> Incompatible version {0} is not supported", Version);
                        return;
                    }
#endif

                    var PropCount = b.ReadInt32();

                    for (int i = 0; i < PropCount; i++)
                    {
                        var prop = new Prop();
                        prop.QuestPropID = b.ReadInt32();
                        prop.X = b.ReadSingle();
                        prop.Y = b.ReadSingle();
                        prop.OffSet = b.ReadSingle();
                        prop.RotateX = b.ReadSingle();
                        prop.RotateY = b.ReadSingle();
                        prop.RotateZ = b.ReadSingle();
                        prop.ScaleX = b.ReadSingle();
                        prop.ScaleY = (Version >= 3) ? b.ReadSingle() : prop.ScaleX;
                        prop.ScaleZ = (Version >= 3) ? b.ReadSingle() : prop.ScaleX;
                        prop.PropNum = b.ReadUInt16();
                        prop.LockedHeight = (Version >= 3) ? b.ReadBoolean() : false;
                        prop.LockHeight = (Version >= 3) ? b.ReadSingle() : 0f;
                        prop.TextureGroupIndex = (Version >= 3) ? b.ReadInt16() : (short)-1;
                        Props.Add(prop);
                    }
                }
            }
            catch (Exception exception)
            {
                Blank();
                XLog.WriteLine(Levels.Error, "QpfManager::Load<Exception> -> {0}", exception);
            }
        }

        /// <summary>
        /// Refresh the current info for painting
        /// </summary>
        public void Refresh()
        {
            Painting?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// Remove entry by index
        /// </summary>
        /// <param name="index"></param>
        public void Remove(int index)
        {
            Props.RemoveAt(index);

            Removed?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// Load existing nflavor script file
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="filename"></param>
        public void Save(string directory, string filename)
        {
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

            XLog.WriteLine(Levels.Info, $"Saving {filename}...");
            try
            {
                using (BinaryWriter b = new BinaryWriter(File.Open(Path.Combine(directory, filename), FileMode.Create)))
                {
                    b.Write(Encoding.Default.GetBytes(Sign));
                    b.Write(Version);
                    b.Write(Props.Count);

                    for (int i = 0; i < Props.Count; i++)
                    {
                        b.Write(Props[i].QuestPropID);
                        b.Write(Props[i].X);
                        b.Write(Props[i].Y);
                        b.Write(Props[i].OffSet);
                        b.Write(Props[i].RotateX);
                        b.Write(Props[i].RotateY);
                        b.Write(Props[i].RotateZ);
                        b.Write(Props[i].ScaleX);
                        b.Write(Props[i].ScaleY);
                        b.Write(Props[i].ScaleZ);
                        b.Write(Props[i].PropNum);
                        b.Write(Props[i].LockedHeight);
                        b.Write(Props[i].LockHeight);
                        b.Write(Props[i].TextureGroupIndex);
                    }
                }
            }
            catch (Exception exception)
            {
                XLog.WriteLine(Levels.Error, "QpfManager::Save<Exception> -> {0}", exception);
            }
        }

        public override string ToString()
        {
            return "nFlavor QuestProp";
        }
    }
}
