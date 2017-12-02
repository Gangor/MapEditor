using DataCore;
using MapEditor.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

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
		public NfwManager() : base(true) { Blank(); }

		/// <summary>
		/// Add new prop
		/// </summary>
		/// <param name="location"></param>
		public void Add(PointF first, PointF last)
		{
			var water = new Water();
			var center = _2DUtils.GetCenterPoint(first, last);
			water.PointA = new K3DPosition(first.X, first.Y, 0f);
			water.Center = new K3DPosition(center.X, center.Y, 0f);
			water.PointB = new K3DPosition(last.X, last.Y, 0f);
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

				XLog.WriteLine(Levels.Good, "Ok");
				return stream.ToArray();
			}
			catch (Exception exception)
			{
				XLog.WriteLine(Levels.Error, "Failed");
				XLog.WriteLine(Levels.Fatal, "NfwManager::GetBuffer<Exception> -> {0}", exception);
			}

			return null;
		}

		/// <summary>
		/// Load existing nflavor water
		/// </summary>
		/// <param name="buffer"></param>
		public void Load(byte[] buffer)
		{
			try
			{
				using (BinaryReader b = new BinaryReader(new MemoryStream(buffer)))
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

				XLog.WriteLine(Levels.Good, "Ok");
			}
			catch (Exception exception)
			{
				Blank();
				XLog.WriteLine(Levels.Error, "Failed");
				XLog.WriteLine(Levels.Fatal, "NfwManager::Load<Exception> -> {0}", exception);
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
		/// Get class name to string
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return "nFlavor Water";
		}
	}
}
