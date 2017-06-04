using MapEditor.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace MapEditor.Modules
{
    public class NfaManager : Nfa
    {
        public event EventHandler<Polygon2> Added;
        public event EventHandler<EventArgs> Painting;
        public event EventHandler<EventArgs> Removed;

        /// <summary>
        /// Initialize a new instance
        /// </summary>
        public NfaManager() : base() { }

        /// <summary>
        /// Add new collision
        /// </summary>
        /// <param name="location"></param>
        public void AddCollision(Polygon2 polygon)
        {
            Polygons.Add(polygon);

            Added?.Invoke(this, polygon);
        }

        /// <summary>
        /// Reinitilize child objet
        /// </summary>
        public void Blank()
        {
            Polygons = new List<Polygon2>();
        }

        /// <summary>
        /// Load existing collision file
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
                    var polygonCount = b.ReadInt32();

                    for (int i = 0; i < polygonCount; i++)
                    {
                        var polygon = new Polygon2();
                        var pointNum = b.ReadInt32();

                        for (int p = 0; p < pointNum; p++)
                        {
                            var point = new K2DVector();
                            point.X = b.ReadInt32();
                            point.Y = b.ReadInt32();
                            polygon.Points.Add(point);
                        }

                        Polygons.Add(polygon);
                    }                    
                }
            }
            catch (Exception exception)
            {
                Blank();
                XLog.WriteLine(Levels.Error, "NfaManager::Load<Exception> -> {0}", exception);
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
        /// Remove a region
        /// </summary>
        /// <param name="index"></param>
        public void Remove(int index)
        {
            Polygons.RemoveAt(index);

            Removed?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// Save collision file
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
                    b.Write(Polygons.Count);

                    for (int i = 0; i < Polygons.Count; i++)
                    {
                        b.Write(Polygons[i].Points.Count);

                        for (int p = 0; p < Polygons[i].Points.Count; p++)
                        {
                            b.Write(Polygons[i].Points[p].X);
                            b.Write(Polygons[i].Points[p].Y);
                        }                        
                    }
                }
            }
            catch (Exception exception)
            {
                XLog.WriteLine(Levels.Error, "NfaManager::Save<Exception> -> {0}", exception);
            }
        }

        /// <summary>
        /// Get class name to string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "nFlavor Collision";
        }
    }
}
