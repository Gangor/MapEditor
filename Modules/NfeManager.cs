using MapEditor.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
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
		public NfeManager() : base(true) { Blank(); }

		/// <summary>
		/// Add new regions
		/// </summary>
		/// <param name="location"></param>
		public void Add(PointF[] points)
		{
			var eventArea = new EventAreaScript();
			var polygon = _2DUtils.PointToPolygon(points);

			eventArea.Polygons.Add(polygon);
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

				XLog.WriteLine(Levels.Good, "Ok");
				return stream.ToArray();
			}
			catch (Exception exception)
			{
				XLog.WriteLine(Levels.Error, "Failed");
				XLog.WriteLine(Levels.Fatal, "NfeManager::GetBuffer<Exception> -> {0}", exception);
			}

			return null;
		}

		/// <summary>
		/// Load existing event area
		/// </summary>
		/// <param name="buffer"></param>
		public void Load(byte[] buffer)
		{
			try
			{
				using (BinaryReader b = new BinaryReader(new MemoryStream(buffer)))
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
								var point = new K2DPosition();
								point.X = b.ReadInt32();
								point.Y = b.ReadInt32();
								polygon.Points.Add(point);
							}

							area.Polygons.Add(polygon);
						}

						Events.Add(area);
					}
				}

				XLog.WriteLine(Levels.Good, "Ok");
			}
			catch (Exception exception)
			{
				Blank();
				XLog.WriteLine(Levels.Error, "Failed");
				XLog.WriteLine(Levels.Fatal, "NfeManager::Load<Exception> -> {0}", exception);
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
			Events.RemoveAt(index);

			Removed?.Invoke(this, new EventArgs());
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
