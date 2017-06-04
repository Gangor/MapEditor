﻿using MapEditor.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MapEditor.Modules
{
    public class NfsManager : Nfs
    {
        public int[] SupportedVersion = { 2 };

        public event EventHandler<Location> AddedLocation;
        public event EventHandler<PropScriptInfo> AddedPropScript;
        public event EventHandler<EventArgs> Painting;
        public event EventHandler<EventArgs> Removed;

        /// <summary>
        /// Initialize a new instance
        /// </summary>
        public NfsManager() : base() { }

        /// <summary>
        /// Add new regions
        /// </summary>
        /// <param name="location"></param>
        public void AddLocation(Location location)
        {
            Respawns.Add(location);

            AddedLocation?.Invoke(this, location);
        }

        /// <summary>
        /// Add prop script
        /// </summary>
        /// <param name="propScript"></param>
        public void AddPropScript(PropScriptInfo propScript)
        {
            Props.Add(propScript);

            AddedPropScript?.Invoke(this, propScript);
        }

        /// <summary>
        /// Reinitilize child objet
        /// </summary>
        public void Blank()
        {
            Respawns = new List<Location>();
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
                    Sign = Encoding.Default.GetString(b.ReadBytes(16));
                    Version = b.ReadInt32();

                    XLog.WriteLine(Levels.Debug, $"NFS version {Version}.");

#if DEBUG == false
                    if (!SupportedVersion.Contains(Version))
                    {
                        XLog.WriteLine(Levels.Fatal, "NfsManager::Load<Version> -> Incompatible version {0} is not supported", Version);
                        return;
                    }
#endif

                    /* nfs.dwEventLocationOffset = */ b.ReadInt32();
                    /* nfs.dwEventScriptOffset = */ b.ReadInt32();
                    /* nfs.dwPropScriptOffset = */ b.ReadInt32();

                    var nLocationCount = b.ReadInt32();
                    Respawns = new List<Location>();

                    for (int i = 0; i < nLocationCount; i++)
                    {
                        var location = new Location();
                        location.Left = b.ReadInt32();
                        location.Top = b.ReadInt32();
                        location.Right = b.ReadInt32();
                        location.Bottom = b.ReadInt32();
                        var stringSize = b.ReadInt32();
                        location.Description = Encoding.Default.GetString(b.ReadBytes(stringSize));
                        Respawns.Add(location);
                    }

                    var nScriptCount = b.ReadInt32();

                    for (int i = 0; i < nScriptCount; i++)
                    {
                        var index = b.ReadInt32();
                        var nFunctionCount = b.ReadInt32();

                        for (int f = 0; f < nFunctionCount; f++)
                        {
                            var function = new Function();
                            /* function.nTrigger = */ b.ReadInt32();
                            var nStringSize = b.ReadInt32();
                            function.FunctionString = Encoding.Default.GetString(b.ReadBytes(nStringSize));
                            Respawns[index].Scripts.Add(function);
                        }
                    }

                    var nPropCount = b.ReadInt32();
                    Props = new List<PropScriptInfo>();

                    for (int i = 0; i < nPropCount; i++)
                    {
                        var propScript = new PropScriptInfo();
                        propScript.PropId = b.ReadInt32();
                        propScript.X = b.ReadSingle();
                        propScript.Y = b.ReadSingle();
                        propScript.ModelId = b.ReadInt16();
                        var nFunctionCount = b.ReadInt32();

                        for (int f = 0; f < nFunctionCount; f++)
                        {
                            var function = new Function();
                            /* function.nTrigger = */ b.ReadInt32();
                            var nStringSize = b.ReadInt32();
                            function.FunctionString = Encoding.Default.GetString(b.ReadBytes(nStringSize));
                            propScript.Scripts.Add(function);
                        }

                        Props.Add(propScript);
                    }
                }
            }
            catch (Exception exception)
            {
                Blank();
                XLog.WriteLine(Levels.Error, "NfsManager::Load<Exception> -> {0}", exception);
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
        /// Save nflavor script file
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

                    b.Write(0); /* dwEventLocationOffset = */
                    b.Write(0); /* dwEventScriptOffset = */
                    b.Write(0); /* dwPropScriptOffset = */

                    int dwEventLocationOffset = (int)b.BaseStream.Position;
                    b.Write(Respawns.Count);

                    for (int i = 0; i < Respawns.Count; i++)
                    {
                        b.Write(Respawns[i].Left);
                        b.Write(Respawns[i].Top);
                        b.Write(Respawns[i].Right);
                        b.Write(Respawns[i].Bottom);
                        b.Write(Respawns[i].Description.Length);
                        b.Write(Encoding.Default.GetBytes(Respawns[i].Description));
                    }

                    int dwEventScriptOffset = (int)b.BaseStream.Position;
                    b.Write(GetScriptCount());

                    for (int i = 0; i < Respawns.Count; i++)
                    {
                        if (Respawns[i].Scripts.Count > 0)
                        {
                            b.Write(i);
                            b.Write(Respawns[i].Scripts.Count);

                            for (int f = 0; f < Respawns[i].Scripts.Count; f++)
                            {
                                b.Write(f);
                                b.Write(Respawns[i].Scripts[f].FunctionString.Length);
                                b.Write(Encoding.Default.GetBytes(Respawns[i].Scripts[f].FunctionString));
                            }
                        }
                    }

                    int dwPropScriptOffset = (int)b.BaseStream.Position;
                    b.Write(Props.Count);

                    for (int i = 0; i < Props.Count; i++)
                    {
                        b.Write(Props[i].PropId);
                        b.Write(Props[i].X);
                        b.Write(Props[i].Y);
                        b.Write(Props[i].ModelId);
                        b.Write(Props[i].Scripts.Count);

                        for (int f = 0; f < Props[i].Scripts.Count; f++)
                        {
                            b.Write(f);
                            b.Write(Props[i].Scripts[f].FunctionString.Length);
                            b.Write(Encoding.Default.GetBytes(Props[i].Scripts[f].FunctionString));
                        }
                    }

                    b.Seek(20, SeekOrigin.Begin);
                    b.Write(dwEventLocationOffset);
                    b.Write(dwEventScriptOffset);
                    b.Write(dwPropScriptOffset);
                }
            }
            catch (Exception exception)
            {
                XLog.WriteLine(Levels.Error, "NfsManager::Save<Exception> -> {0}", exception);
            }
        }

        /// <summary>
        /// Remove nflavor script
        /// </summary>
        /// <param name="index"></param>
        public void Remove(int index)
        {
            Respawns.RemoveAt(index);

            Removed?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// Get class name to string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "nFlavor Script";
        }
    }
}