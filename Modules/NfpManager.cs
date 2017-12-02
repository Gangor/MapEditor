using DataCore;
using MapEditor.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MapEditor.Modules
{
	public class NfpManager : Nfp
	{
		public event EventHandler<RecordNfp> Added;
		public event EventHandler<EventArgs> Painting;
		public event EventHandler<EventArgs> Removed;

		/// <summary>
		/// Initialize a new instance
		/// </summary>
		public NfpManager() : base() { Blank(); }

		/// <summary>
		/// Add new regions
		/// </summary>
		/// <param name="location"></param>
		public void AddNfp(RecordNfp record)
		{
			Records.Add(record);

			Added?.Invoke(this, record);
		}

		/// <summary>
		/// Reinitilize child objet
		/// </summary>
		public void Blank()
		{
			Records = new List<RecordNfp>();
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
					b.Write(Records.Count);

					for (int i = 0; i < Records.Count; i++)
					{
						b.Write(Records[i].Id);
						b.Write(Records[i].Polygons.Count);

						for (int p = 0; p < Records[i].Polygons.Count; p++)
						{
							b.Write(Records[i].Polygons[p].Points.Count);

							for (int t = 0; t < Records[i].Polygons[p].Points.Count; t++)
							{
								b.Write(Records[i].Polygons[p].Points[t].X);
								b.Write(Records[i].Polygons[p].Points[t].Y);
								b.Write(Records[i].Polygons[p].Points[t].Z);
							}
						}

						var description = Records[i].Description.Length == 0 ?
							Records[i].Description :
							Records[i].Description.Replace("\0", "") + '\0';

						b.Write(description.Length);
						b.Write(Encoding.Default.GetBytes(description));
					}
				}

				XLog.WriteLine(Levels.Good, "Ok");
				return stream.ToArray();
			}
			catch (Exception exception)
			{
				XLog.WriteLine(Levels.Error, "Failed");
				XLog.WriteLine(Levels.Fatal, "NfpManager::GetBuffer<Exception> -> {0}", exception);
			}

			return null;
		}

		/// <summary>
		/// Load existing unknown
		/// </summary>
		/// <param name="buffer"></param>
		public void Load(byte[] buffer)
		{
			try
			{
				using (BinaryReader b = new BinaryReader(new MemoryStream(buffer)))
				{
					var nfpCount = b.ReadInt32();

					for (int i = 0; i < nfpCount; i++)
					{
						var nfp = new RecordNfp();
						nfp.Id = b.ReadInt32();
						var polygonCount = b.ReadInt32();

						for (int p = 0; p < polygonCount; p++)
						{
							var polygon = new Polygon3();
							var pointNum = b.ReadInt32();

							for (int t = 0; t < pointNum; t++)
							{
								var point = new K3DPosition(b.ReadSingle(), b.ReadSingle(), b.ReadSingle());
								polygon.Points.Add(point);
							}

							nfp.Polygons.Add(polygon);
						}

						var DescriptionCount = b.ReadInt32();
						nfp.Description = Encoding.Default.GetString(b.ReadBytes(DescriptionCount));

						Records.Add(nfp);
					}
				}

				XLog.WriteLine(Levels.Good, "Ok");
			}
			catch (Exception exception)
			{
				Blank();
				XLog.WriteLine(Levels.Error, "Failed");
				XLog.WriteLine(Levels.Fatal, "NfpManager::Load<Exception> -> {0}", exception);
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
		/// Remove record from list
		/// </summary>
		/// <param name="index"></param>
		public void Remove(int index)
		{
			Records.RemoveAt(index);

			Removed?.Invoke(this, new EventArgs());
		}

		/// <summary>
		/// Get class name to string
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return "nFlavor Unknown";
		}
	}
}
