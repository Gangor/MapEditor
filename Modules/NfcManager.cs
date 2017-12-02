using MapEditor.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace MapEditor.Modules
{
	public class NfcManager : Nfc
	{
		public event EventHandler<LocationInfo> Added;
		public event EventHandler<EventArgs> Painting;
		public event EventHandler<EventArgs> Removed;

		/// <summary>
		/// Initialize a new instance
		/// </summary>
		public NfcManager() : base() { Blank(); }

		/// <summary>
		/// Add new regions
		/// </summary>
		/// <param name="location"></param>
		public void Add(PointF[] points, PointF center)
		{
			var region = new LocationInfo();
			var polygon = _2DUtils.PointToPolygon(points);

			region.X = center.X;
			region.Y = center.Y;
			region.Polygons.Add(polygon);
			Region.Add(region);

			Added?.Invoke(this, region);
		}

		/// <summary>
		/// Reinitilize child objet
		/// </summary>
		public void Blank()
		{
			Region = new List<LocationInfo>();
		}

		/// <summary>
		/// get buffer final file
		/// </summary>
		/// <returns></returns>
		public byte[] GetBuffer()
		{
			try
			{
				var stream = new MemoryStream();

				using (BinaryWriter b = new BinaryWriter(stream))
				{
					b.Write(Region.Count);

					for (int i = 0; i < Region.Count; i++)
					{
						b.Write(Region[i].Priority);
						b.Write(Region[i].X);
						b.Write(Region[i].Y);
						b.Write(Region[i].Z);
						b.Write(Region[i].Radius);
						b.Write(Region[i].Description.Length);
						b.Write(Encoding.Default.GetBytes(Region[i].Description));

						var script = Region[i].Scripts.Length == 0 ?
							Region[i].Scripts :
							Region[i].Scripts.Replace("\0", "") + '\0';

						b.Write(script.Length);
						b.Write(Encoding.Default.GetBytes(script));
						b.Write(Region[i].Polygons.Count);

						for (int p = 0; p < Region[i].Polygons.Count; p++)
						{
							b.Write(Region[i].Polygons[p].Points.Count);

							for (int n = 0; n < Region[i].Polygons[p].Points.Count; n++)
							{
								b.Write(Region[i].Polygons[p].Points[n].X);
								b.Write(Region[i].Polygons[p].Points[n].Y);
							}

						}
					}
				}

				XLog.WriteLine(Levels.Good, "Ok");
				return stream.ToArray();
			}
			catch (Exception exception)
			{
				XLog.WriteLine(Levels.Error, "Failed");
				XLog.WriteLine(Levels.Fatal, "NfcManager::GetBuffer<Exception> -> {0}", exception);
			}

			return null;
		}

		/// <summary>
		/// Load existing region
		/// </summary>
		/// <param name="buffer"></param>
		public void Load(byte[] buffer)
		{
			try
			{
				using (BinaryReader b = new BinaryReader(new MemoryStream(buffer)))
				{
					var regionCount = b.ReadInt32();

					for (int i = 0; i < regionCount; i++)
					{
						var region = new LocationInfo();
						region.Priority = b.ReadInt32();
						region.X = b.ReadSingle();
						region.Y = b.ReadSingle();
						region.Z = b.ReadSingle();
						region.Radius = b.ReadSingle();
						var DescriptionSize = b.ReadInt32();
						region.Description = Encoding.Default.GetString(b.ReadBytes(DescriptionSize));
						var ScriptSize = b.ReadInt32();
						region.Scripts = Encoding.Default.GetString(b.ReadBytes(ScriptSize));

						var polygonCount = b.ReadInt32();

						for (int p = 0; p < polygonCount; p++)
						{
							var polygon = new Polygon2();
							var pointNum = b.ReadInt32();

							for (int n = 0; n < pointNum; n++)
							{
								var point = new K2DPosition();
								point.X = b.ReadInt32();
								point.Y = b.ReadInt32();
								polygon.Points.Add(point);
							}

							region.Polygons.Add(polygon);
						}

						Region.Add(region);
					}
				}

				XLog.WriteLine(Levels.Good, "Ok");
			}
			catch (Exception exception)
			{
				Blank();
				XLog.WriteLine(Levels.Error, "Failed");
				XLog.WriteLine(Levels.Fatal, "NfcManager::Load<Exception> -> {0}", exception);
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
			Region.RemoveAt(index);

			Removed?.Invoke(this, new EventArgs());
		}

		/// <summary>
		/// Get class name to string
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return "nFlavor Word";
		}
	}
}
