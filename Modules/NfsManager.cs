using DataCore;
using MapEditor.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
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
		public NfsManager() : base() { Blank(); }

		/// <summary>
		/// Add new regions
		/// </summary>
		/// <param name="location"></param>
		public void Add(PointF first, PointF last)
		{
			var location = new Location();
			location.Left = (int)first.X;
			location.Top = (int)first.Y;
			location.Right = (int)last.X;
			location.Bottom = (int)last.Y;
			Respawns.Add(location);

			AddedLocation?.Invoke(this, location);
		}

		/// <summary>
		/// Add prop script
		/// </summary>
		/// <param name="propScript"></param>
		public void Add(PointF point)
		{
			var propScript = new PropScriptInfo();
			propScript.X = point.X;
			propScript.Y = point.Y;
			Props.Add(propScript);

			AddedPropScript?.Invoke(this, propScript);
		}

		/// <summary>
		/// Reinitilize child objet
		/// </summary>
		public void Blank()
		{
			Respawns = new List<Location>();
			Props = new List<PropScriptInfo>();
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

				XLog.WriteLine(Levels.Good, "Ok");
				return stream.ToArray();
			}
			catch (Exception exception)
			{
				XLog.WriteLine(Levels.Error, "Failed");
				XLog.WriteLine(Levels.Fatal, "NfsManager::GetBuffer<Exception> -> {0}", exception);
			}

			return null;
		}

		/// <summary>
		/// Load existing nflavor script
		/// </summary>
		/// <param name="buffer"></param>
		public void Load(byte[] buffer)
		{
			try
			{
				using (BinaryReader b = new BinaryReader(new MemoryStream(buffer)))
				{
					Sign = Encoding.Default.GetString(b.ReadBytes(16));
					Version = b.ReadInt32();

#if DEBUG == false
					if (!SupportedVersion.Contains(Version))
					{
						XLog.WriteLine(Levels.Error, $"Failed");
						XLog.WriteLine(Levels.Fatal, "NfsManager::Load<Version> -> Incompatible version {0} is not supported", Version);
						return;
					}
#endif

					/* nfs.dwEventLocationOffset = */
					b.ReadInt32();
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

				XLog.WriteLine(Levels.Good, "Ok");
			}
			catch (Exception exception)
			{
				Blank();
				XLog.WriteLine(Levels.Error, "Failed");
				XLog.WriteLine(Levels.Fatal, "NfsManager::Load<Exception> -> {0}", exception);
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
		/// Remove respawn nflavor script
		/// </summary>
		/// <param name="index"></param>
		public void RemoveR(int index)
		{
			Respawns.RemoveAt(index);

			Removed?.Invoke(this, new EventArgs());
		}

		/// <summary>
		/// Remove prop nflavor script
		/// </summary>
		/// <param name="index"></param>
		public void RemoveP(int index)
		{
			Props.RemoveAt(index);

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
