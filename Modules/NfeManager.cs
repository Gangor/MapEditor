using MapEditor.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace MapEditor.Modules
{
    public class NfeManager : Nfe
    {
        public event EventHandler<EventAreaScript> Added;
        public event EventHandler<EventArgs> Painting;
        public event EventHandler<EventArgs> Removed;

        /// <summary>
        /// Initialize a new instance
        /// </summary>
        public NfeManager() : base(true) { }

        /// <summary>
        /// Add new regions
        /// </summary>
        /// <param name="location"></param>
        public void AddEventArea(EventAreaScript eventArea)
        {
            Events.Add(eventArea);

            Added?.Invoke(this, eventArea);
        }

        /// <summary>
        /// Reinitilize child objet
        /// </summary>
        public void Blank()
        {
            Events = new List<EventAreaScript>();
        }

        /// <summary>
        /// Load existing event area file
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
                    var areaCount = b.ReadInt32();

                    for (int i = 0; i < areaCount; i++)
                    {
                        var area = new EventAreaScript();
                        area.AreaId = b.ReadInt32();
                        var polygonCount = b.ReadInt32();

                        for (int p = 0; p < polygonCount; p++)
                        {
                            var polygon = new Polygon2();
                            var pointNum = b.ReadInt32();

                            for (int n = 0; n < pointNum; n++)
                            {
                                var point = new K2DVector();
                                point.X = b.ReadInt32();
                                point.Y = b.ReadInt32();
                                polygon.Points.Add(point);
                            }

                            area.Polygons.Add(polygon);
                        }

                        Events.Add(area);
                    }                    
                }
            }
            catch (Exception exception)
            {
                Blank();
                XLog.WriteLine(Levels.Error, "NfeManager::Load<Exception> -> {0}", exception);
            }
        }

        /// <summary>
        /// Refresh the current info for painting
        /// </summary>
        public void Refresh()
        {
            Painting?.Invoke(this, new EventArgs());
        }

        public void Remove(int index)
        {
            Events.RemoveAt(index);

            Removed?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// Save event area file
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
                    b.Write(Events.Count);

                    for (int i = 0; i < Events.Count; i++)
                    {
                        b.Write(Events[i].AreaId);
                        b.Write(Events[i].Polygons.Count);

                        for (int p = 0; p < Events[i].Polygons.Count; p++)
                        {
                            b.Write(Events[i].Polygons[p].Points.Count);

                            for (int n = 0; n < Events[i].Polygons[p].Points.Count; n++)
                            {
                                b.Write(Events[i].Polygons[p].Points[n].X);
                                b.Write(Events[i].Polygons[p].Points[n].Y);
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                XLog.WriteLine(Levels.Error, "NfeManager::Save<Exception> -> {0}", exception);
            }
        }

        /// <summary>
        /// Get class name to string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "nFlavor EventArea";
        }
    }
}
