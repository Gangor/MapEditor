using MapEditor.Enum;
using MapEditor.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace MapEditor.Modules
{
    public class NflManager : Nfl
    {
        public event EventHandler<StructLights> Added;
        public event EventHandler<EventArgs> Painting;
        public event EventHandler<EventArgs> Removed;

        /// <summary>
        /// Initialize a new instance
        /// </summary>
        public NflManager() : base() { }

        /// <summary>
        /// Add new regions
        /// </summary>
        /// <param name="location"></param>
        public void Add(StructLights light)
        {
            Lights.Add(light);

            Added?.Invoke(this, light);
        }

        /// <summary>
        /// Reinitilize child objet
        /// </summary>
        public void Blank()
        {
            Specular = new KColor();
            Diffuse = new KColor();
            Ambient = new KColor(0, 255, 0, 255);
            Lights = new List<StructLights>();
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
                    Direction.X = b.ReadSingle();
                    Direction.Y = b.ReadSingle();
                    Direction.Z = b.ReadSingle();
                    Specular = new KColor(b.ReadByte(), b.ReadByte(), b.ReadByte(), b.ReadByte());
                    Diffuse = new KColor(b.ReadByte(), b.ReadByte(), b.ReadByte(), b.ReadByte());
                    Ambient = new KColor(b.ReadByte(), b.ReadByte(), b.ReadByte(), b.ReadByte());

                    var lightCount = b.ReadInt32();

                    for (int i = 0; i < lightCount; i++)
                    {
                        var light = new StructLights();
                        light.Position.X = b.ReadSingle();
                        light.Position.Y = b.ReadSingle();
                        light.Position.Z = b.ReadSingle();
                        light.Height = b.ReadSingle();
                        light.Direction.X = b.ReadSingle();
                        light.Direction.Y = b.ReadSingle();
                        light.Direction.Z = b.ReadSingle();
                        light.Specular = new KColor(b.ReadByte(), b.ReadByte(), b.ReadByte(), b.ReadByte());
                        light.Diffuse = new KColor(b.ReadByte(), b.ReadByte(), b.ReadByte(), b.ReadByte());
                        light.Ambient = new KColor(b.ReadByte(), b.ReadByte(), b.ReadByte(), b.ReadByte());
                        light.LightType = (LightsType)b.ReadInt32();
                        Lights.Add(light);
                    }                    
                }
            }
            catch (Exception exception)
            {
                Blank();
                XLog.WriteLine(Levels.Error, "NflManager::Load<Exception> -> {0}", exception);
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
        /// Remove light
        /// </summary>
        /// <param name="index"></param>
        public void Remove(int index)
        {
            Lights.RemoveAt(index);

            Removed?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// Load existing collision file
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
                    b.Write(Direction.X);
                    b.Write(Direction.Y);
                    b.Write(Direction.Z);
                    b.Write(Specular.Color.B);
                    b.Write(Specular.Color.G);
                    b.Write(Specular.Color.R);
                    b.Write(Specular.Color.A);
                    b.Write(Diffuse.Color.B);
                    b.Write(Diffuse.Color.G);
                    b.Write(Diffuse.Color.R);
                    b.Write(Diffuse.Color.A);
                    b.Write(Ambient.Color.B);
                    b.Write(Ambient.Color.G);
                    b.Write(Ambient.Color.R);
                    b.Write(Ambient.Color.A);
                    b.Write(Lights.Count);

                    for (int i = 0; i < Lights.Count; i++)
                    {
                        b.Write(Lights[i].Position.X);
                        b.Write(Lights[i].Position.Y);
                        b.Write(Lights[i].Position.Z);
                        b.Write(Lights[i].Height);
                        b.Write(Lights[i].Direction.X);
                        b.Write(Lights[i].Direction.Y);
                        b.Write(Lights[i].Direction.Z);
                        b.Write(Lights[i].Specular.Color.B);
                        b.Write(Lights[i].Specular.Color.G);
                        b.Write(Lights[i].Specular.Color.R);
                        b.Write(Lights[i].Specular.Color.A);
                        b.Write(Lights[i].Diffuse.Color.B);
                        b.Write(Lights[i].Diffuse.Color.G);
                        b.Write(Lights[i].Diffuse.Color.R);
                        b.Write(Lights[i].Diffuse.Color.A);
                        b.Write(Lights[i].Ambient.Color.B);
                        b.Write(Lights[i].Ambient.Color.G);
                        b.Write(Lights[i].Ambient.Color.R);
                        b.Write(Lights[i].Ambient.Color.A);
                        b.Write((int)Lights[i].LightType);
                    }
                }
            }
            catch (Exception exception)
            {
                XLog.WriteLine(Levels.Error, "NflManager::Save<Exception> -> {0}", exception);
            }
        }

        /// <summary>
        /// Get class name to string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "nFlavor Light";
        }
    }
}
