using MapEditor.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
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
		public NfaManager() : base() { Blank(); }

		/// <summary>
		/// Add new collision
		/// </summary>
		/// <param name="location"></param>
		public void Add(PointF[] points)
		{
			var polygon = _2DUtils.PointToPolygon(points);
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

				XLog.WriteLine(Levels.Good, "Ok");
				return stream.ToArray();
			}
			catch (Exception exception)
			{
				XLog.WriteLine(Levels.Error, "Failed");
				XLog.WriteLine(Levels.Fatal, "NfaManager::GetBuffer<Exception> -> {0}", exception);
			}

			return null;
		}

		/// <summary>
		/// Load existing collision
		/// </summary>
		/// <param name="buffer"></param>
		public void Load(byte[] buffer)
		{
			try
			{
				using (BinaryReader b = new BinaryReader(new MemoryStream(buffer)))
				{
					var polygonCount = b.ReadInt32();

					for (int i = 0; i < polygonCount; i++)
					{
						var polygon = new Polygon2();
						var pointNum = b.ReadInt32();

						for (int p = 0; p < pointNum; p++)
						{
							var point = new K2DPosition();
							point.X = b.ReadInt32();
							point.Y = b.ReadInt32();
							polygon.Points.Add(point);
						}

						Polygons.Add(polygon);
					}
				}

				XLog.WriteLine(Levels.Good, "Ok");
			}
			catch (Exception exception)
			{
				Blank();
				XLog.WriteLine(Levels.Error, "Failed");
				XLog.WriteLine(Levels.Fatal, "NfaManager::Load<Exception> -> {0}", exception);
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
		/// Get class name to string
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return "nFlavor Collision";
		}
	}
}
