using MapEditor.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MapEditor.Modules
{
    public class PvsManager : Pvs
    {
        public int[] SupportedVersion = { 1 };

        /// <summary>
        /// Initialize a new instance
        /// </summary>
        public PvsManager() : base() { }

        /// <summary>
        /// Reinitilize child objet
        /// </summary>
        public void Blank(byte x, byte y)
        {
            SegmentCountPerMap = 64;
            MapStartPosX = x;
            MapStartPosY = y;
            SegmentLeft = 0;
            SegmentTop = 0;
            SegmentRight = 0;
            SegmentBottom = 0;
            Segments = new List<PVS_SEGMENT_V1>();
            Props = new List<PVS_PROP_V1>();
        }

        /// <summary>
        /// Load existing collision file
        /// </summary>
        /// <param name="filename"></param>
        public void Load(string path, string filename, byte X, byte Y)
        {
            Blank(X, Y);

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
                    Version = b.ReadUInt16();

                    XLog.WriteLine(Levels.Debug, $"PVS version {Version}.");

#if DEBUG == false
                    if (!SupportedVersion.Contains(Version))
                    {
                        XLog.WriteLine(Levels.Fatal, "PvsManager::Load<Version> -> Incompatible version {0} is not supported", Version);
                        return;
                    }
#endif

                    SegmentCountPerMap = b.ReadByte();
                    MapStartPosX = b.ReadByte();
                    MapStartPosY = b.ReadByte();

                    var segmentCount = b.ReadUInt16();
                    var propCount = b.ReadInt32();

                    SegmentLeft = b.ReadByte();
                    SegmentTop = b.ReadByte();
                    SegmentRight = b.ReadByte();
                    SegmentBottom = b.ReadByte();

                    for (int i = 0; i < segmentCount; i++)
                    {
                        var segment = new PVS_SEGMENT_V1();
                        segment.SegmentX = b.ReadByte();
                        segment.SegmentY = b.ReadByte();
                        var includeSegmentCount = b.ReadUInt16();

                        for (int f = 0; f < includeSegmentCount; f++)
                        {
                            var includeSegment = new SEGMENT_DATA_V1();
                            includeSegment.SegmentX = b.ReadByte();
                            includeSegment.SegmentY = b.ReadByte();
                            segment.IncludeSegments.Add(includeSegment);
                        }

                        Segments.Add(segment);
                    }

                    for (int i = 0; i < propCount; i++)
                    {
                        var prop = new PVS_PROP_V1();
                        prop.SegmentX = b.ReadByte();
                        prop.SegmentY = b.ReadByte();
                        var includePropCount = b.ReadInt32();

                        for (int f = 0; f < includePropCount; f++)
                        {
                            var includeProp = new PROP_DATA_V1();
                            includeProp.PropIdx = b.ReadUInt32();
                            includeProp.SegmentIdx = b.ReadUInt32();
                            prop.IncludeProps.Add(includeProp);
                        }

                        Props.Add(prop);
                    }
                }
            }
            catch (Exception exception)
            {
                Blank(X, Y);
                XLog.WriteLine(Levels.Error, "PvsManager::Load<Exception> -> {0}", exception);
            }
        }

        /// <summary>
        /// Save potencial file
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
                    b.Write(SegmentCountPerMap);
                    b.Write(MapStartPosX);
                    b.Write(MapStartPosY);
                    b.Write((ushort)Segments.Count);
                    b.Write(Props.Count);
                    b.Write(SegmentLeft);
                    b.Write(SegmentTop);
                    b.Write(SegmentRight);
                    b.Write(SegmentBottom);

                    for (int i = 0; i < Segments.Count; i++)
                    {
                        b.Write(Segments[i].SegmentX);
                        b.Write(Segments[i].SegmentY);
                        b.Write((ushort)Segments[i].IncludeSegments.Count);

                        for (int f = 0; f < Segments[i].IncludeSegments.Count; f++)
                        {
                            b.Write(Segments[i].IncludeSegments[f].SegmentX);
                            b.Write(Segments[i].IncludeSegments[f].SegmentY);
                        }
                    }

                    for (int i = 0; i < Props.Count; i++)
                    {
                        b.Write(Props[i].SegmentX);
                        b.Write(Props[i].SegmentY);
                        b.Write(Props[i].IncludeProps.Count);

                        for (int f = 0; f < Props[i].IncludeProps.Count; f++)
                        {
                            b.Write(Props[i].IncludeProps[f].PropIdx);
                            b.Write(Props[i].IncludeProps[f].SegmentIdx);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                XLog.WriteLine(Levels.Error, "PvsManager::Save<Exception> -> {0}", exception);
            }
        }

        /// <summary>
        /// Get class name to string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "nFlavor PVS";
        }
    }
}
