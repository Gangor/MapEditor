using MapEditor.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace MapEditor.Modules
{
    public class NfwManager : Nfw
    {
        public event EventHandler<Water> Added;
        public event EventHandler<EventArgs> Painting;
        public event EventHandler<EventArgs> Removed;

        /// <summary>
        /// Initialize a new instance
        /// </summary>
        public NfwManager() : base(true) { }

        /// <summary>
        /// Add new prop
        /// </summary>
        /// <param name="location"></param>
        public void AddWater(Water water)
        {
            Waters.Add(water);

            Added?.Invoke(this, water);
        }

        /// <summary>
        /// Reinitilize child objet
        /// </summary>
        public void Blank()
        {
            Waters = new List<Water>();
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
                    var WaterCount = b.ReadInt32();

                    for (int i = 0; i < WaterCount; i++)
                    {
                        var water = new Water();
                        water.PointA.X = b.ReadSingle();
                        water.PointA.Y = b.ReadSingle();
                        water.PointA.Z = b.ReadSingle();
                        water.PointB.X = b.ReadSingle();
                        water.PointB.Y = b.ReadSingle();
                        water.PointB.Z = b.ReadSingle();
                        water.Center.X = b.ReadSingle();
                        water.Center.Y = b.ReadSingle();
                        water.Center.Z = b.ReadSingle();
                        water.UseReflect = b.ReadInt32();
                        water.WaterId = b.ReadInt32();
                        Waters.Add(water);
                    }
                }
            }
            catch (Exception exception)
            {
                Blank();
                XLog.WriteLine(Levels.Error, "NfwManager::Load<Exception> -> {0}", exception);
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
            Waters.RemoveAt(index);

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
                    b.Write(Waters.Count);

                    for (int i = 0; i < Waters.Count; i++)
                    {
                        b.Write(Waters[i].PointA.X);
                        b.Write(Waters[i].PointA.Y);
                        b.Write(Waters[i].PointA.Z);
                        b.Write(Waters[i].PointB.X);
                        b.Write(Waters[i].PointB.Y);
                        b.Write(Waters[i].PointB.Z);
                        b.Write(Waters[i].Center.X);
                        b.Write(Waters[i].Center.Y);
                        b.Write(Waters[i].Center.Z);
                        b.Write(Waters[i].UseReflect);
                        b.Write(Waters[i].WaterId);
                    }
                }
            }
            catch (Exception exception)
            {
                XLog.WriteLine(Levels.Error, "NfwManager::Save<Exception> -> {0}", exception);
            }
        }

        /// <summary>
        /// Get class name to string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "nFlavor Water";
        }
    }
}
