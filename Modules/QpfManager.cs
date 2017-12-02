using MapEditor.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace MapEditor.Modules
{
	public class QpfManager : Qpf
	{
		public uint[] SupportedVersion = { 1, 3 };

		public event EventHandler<Prop> Added;
		public event EventHandler<EventArgs> Painting;
		public event EventHandler<EventArgs> Removed;

		/// <summary>
		/// Initialize a new instance
		/// </summary>
		public QpfManager() : base() { Blank(); }

		/// <summary>
		/// Add new prop
		/// </summary>
		/// <param name="location"></param>
		public void Add(PointF point)
		{
			var prop = new Prop();
			prop.X = point.X;
			prop.Y = point.Y;
			Props.Add(prop);

			Added?.Invoke(this, prop);
		}

		/// <summary>
		/// Reinitilize child objet
		/// </summary>
		public void Blank()
		{
			Props = new List<Prop>();
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
					b.Write(Encoding.Default.GetBytes(Sign));
					b.Write(Version);
					b.Write(Props.Count);

					for (int i = 0; i < Props.Count; i++)
					{
						b.Write(Props[i].QuestPropID);
						b.Write(Props[i].X);
						b.Write(Props[i].Y);
						b.Write(Props[i].OffSet);
						b.Write(Props[i].RotateX);
						b.Write(Props[i].RotateY);
						b.Write(Props[i].RotateZ);
						b.Write(Props[i].ScaleX);
						b.Write(Props[i].ScaleY);
						b.Write(Props[i].ScaleZ);
						b.Write(Props[i].PropNum);
						b.Write(Props[i].LockedHeight);
						b.Write(Props[i].LockHeight);
						b.Write(Props[i].TextureGroupIndex);
					}
				}

				XLog.WriteLine(Levels.Good, "Ok");
				return stream.ToArray();
			}
			catch (Exception exception)
			{
				XLog.WriteLine(Levels.Error, "Failed");
				XLog.WriteLine(Levels.Fatal, "QpfManager::GetBuffer<Exception> -> {0}", exception);
			}

			return null;
		}

		/// <summary>
		/// Load existing nflavor props
		/// </summary>
		/// <param name="buffer"></param>
		public void Load(byte[] buffer)
		{
			try
			{
				using (BinaryReader b = new BinaryReader(new MemoryStream(buffer)))
				{
					Sign = Encoding.Default.GetString(b.ReadBytes(18));
					Version = b.ReadUInt32();

#if DEBUG == false
					if (!SupportedVersion.Contains(Version))
					{
						XLog.WriteLine(Levels.Error, $"Failed");
						XLog.WriteLine(Levels.Fatal, "QpfManager::Load<Version> -> Incompatible version {0} is not supported", Version);
						return;
					}
#endif

					var PropCount = b.ReadInt32();

					for (int i = 0; i < PropCount; i++)
					{
						var prop = new Prop();
						prop.QuestPropID = b.ReadInt32();
						prop.X = b.ReadSingle();
						prop.Y = b.ReadSingle();
						prop.OffSet = b.ReadSingle();
						prop.RotateX = b.ReadSingle();
						prop.RotateY = b.ReadSingle();
						prop.RotateZ = b.ReadSingle();
						prop.ScaleX = b.ReadSingle();
						prop.ScaleY = (Version >= 3) ? b.ReadSingle() : prop.ScaleX;
						prop.ScaleZ = (Version >= 3) ? b.ReadSingle() : prop.ScaleX;
						prop.PropNum = b.ReadUInt16();
						prop.LockedHeight = (Version >= 3) ? b.ReadBoolean() : false;
						prop.LockHeight = (Version >= 3) ? b.ReadSingle() : 0f;
						prop.TextureGroupIndex = (Version >= 3) ? b.ReadInt16() : (short)-1;
						Props.Add(prop);
					}
				}

				XLog.WriteLine(Levels.Good, "Ok");
			}
			catch (Exception exception)
			{
				Blank();
				XLog.WriteLine(Levels.Error, "Failed");
				XLog.WriteLine(Levels.Fatal, "QpfManager::Load<Exception> -> {0}", exception);
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
			Props.RemoveAt(index);

			Removed?.Invoke(this, new EventArgs());
		}

		public override string ToString()
		{
			return "nFlavor QuestProp";
		}
	}
}
