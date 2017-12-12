using DataCore;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;

namespace MapEditor.Modules
{
	/// <summary>
	/// Manage the map picture
	/// </summary>
	public class MapManager : INotifyPropertyChanged
	{
		public static MapManager Instance = null;

		public const int Width = 2048;
		public const int Height = 2048;

		/// <summary>
		/// Bitmap for cache blank map
		/// </summary>
		private Bitmap map = new Bitmap(Width, Height);

		/// <summary>
		/// Bitmap for draw all element
		/// </summary>
		private Bitmap picture = new Bitmap(Width, Height);

		/// <summary>
		/// Manage bitmap for draw element
		/// </summary>
		private Graphics graphic;

		/// <summary>
		/// Bitmap nude map show in form
		/// </summary>
		public Bitmap Map
		{
			get { return map; }
			set
			{
				map = value;
				OnPropertyChanged("Map");
			}
		}

		/// <summary>
		/// Bitmap actualy show in form
		/// </summary>
		public Bitmap Picture
		{
			get { return picture; }
			set
			{
				picture = value;
				OnPropertyChanged("Picture");
			}
		}

		/// <summary>
		/// Event when property changed
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		#region Constructor

		/// <summary>
		/// Initialize a new instance
		/// </summary>
		protected MapManager()
		{
			graphic = Graphics.FromImage(Picture);
			New();
		}

		/// <summary>
		/// Initialize singleton instance
		/// </summary>
		/// <returns></returns>
		public static bool Init()
		{
			if (Instance == null)
			{
				Instance = new MapManager();
				return true;
			}

			return false;
		}

		/// <summary>
		/// Destroy singleton instance
		/// </summary>
		/// <returns></returns>
		public static bool DeInit()
		{
			if (Instance != null)
			{
				Instance = new MapManager();
				return true;
			}

			return false;
		}

		#endregion

		/// <summary>
		/// Copy image on current pictures
		/// </summary>
		/// <param name="src">Source image</param>
		public void Copy(Bitmap src)
		{
			graphic.DrawImage(src, 0, 0, src.Width, src.Height);
		} 

		/// <summary>
		/// Make point
		/// </summary>
		/// <param name="pen"></param>
		/// <param name="point"></param>
		public void DrawPoint(Pen pen, PointF pointF)
		{
			try
			{
				graphic.DrawEllipse(pen, new RectangleF(pointF.X - 2, pointF.Y - 2, 4, 4));
				graphic.FillEllipse(pen.Brush, new RectangleF(pointF.X - 2, pointF.Y - 2, 4, 4));
			}
			catch { }
		}

		/// <summary>
		/// Make rectangle
		/// </summary>
		/// <param name="pen"></param>
		/// <param name="pt1"></param>
		/// <param name="pt2"></param>
		public void DrawRectangle(Pen pen, PointF pt1, PointF pt2)
		{
			var pointA = new PointF(pt1.X, pt2.Y);
			var pointD = new PointF(pt2.X, pt1.Y);
			try
			{
				graphic.DrawLine(pen, pointA, pt2);
				graphic.DrawLine(pen, pt2, pointD);
				graphic.DrawLine(pen, pointD, pt1);
				graphic.DrawLine(pen, pt1, pointA);
			}
			catch { }
		}

		/// <summary>
		/// Make rectangle with cross
		/// </summary>
		/// <param name="pen"></param>
		/// <param name="pointC"></param>
		/// <param name="pointB"></param>
		public void DrawCrossRectangle(Pen pen, PointF pointC, PointF pointB)
		{
			var pointA = new PointF(pointC.X, pointB.Y);
			var pointD = new PointF(pointB.X, pointC.Y);
			try
			{
				/// Rectangle random LTRB
				graphic.DrawLine(pen, pointA, pointB);
				graphic.DrawLine(pen, pointB, pointD);
				graphic.DrawLine(pen, pointD, pointC);
				graphic.DrawLine(pen, pointC, pointA);
				/// Cossing
				graphic.DrawLine(pen, pointA, pointD);
				graphic.DrawLine(pen, pointB, pointC);
			}
			catch { }
		}

		/// <summary>
		/// Make polygon
		/// </summary>
		/// <param name="pen"></param>
		/// <param name="points"></param>
		public void DrawPolygon(Pen pen, PointF[] points)
		{
			try
			{
				graphic.DrawPolygon(pen, points);
			}
			catch { }
		}

		/// <summary>
		/// Make text center
		/// </summary>
		/// <param name="s"></param>
		/// <param name="font"></param>
		/// <param name="brush"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		public void DrawString(string s, Font font, Brush brush, float x, float y)
		{
			try
			{
				graphic.DrawString(s, font, brush, x, y);
			}
			catch { }
		}

		/// <summary>
		/// Make text center
		/// </summary>
		/// <param name="s"></param>
		/// <param name="font"></param>
		/// <param name="brush"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		public void DrawCenterString(string s, Font font, Brush brush, float x, float y)
		{
			try
			{
				var size = graphic.MeasureString(s, font);
				DrawString(s, font, brush, x - (size.Width / 2f), y + (size.Height / 2f));
			}
			catch { }
		}

		/// <summary>
		/// Create new picture
		/// </summary>
		public void New()
		{
			using (var g = Graphics.FromImage(map))
			{
				g.Clear(Color.FromArgb(255, 120, 146, 173));
				Copy(map);
			}
		}

		/// <summary>
		/// Load the full map
		/// </summary>
		/// <param name="path"></param>
		/// <param name="name"></param>
		/// <param name="encoding"></param>
		public void Load(string path, string name, string encoding)
		{
			Load(core:null, path: path, name: name, encoding:encoding, useCore: false);
		}

		/// <summary>
		/// Load the full map
		/// </summary>
		/// 
		/// <example>
		/// Version 1 : 2048 % 128 = 16
		/// Version 2 : 2048 % 256 = 8
		/// </example>
		/// 
		/// <param name="folder">The location to jpg dump</param>
		/// <param name="name">Associate name file</param>
		public void Load(Core core, string path, string name, string encoding, bool useCore = false)
		{
			New();

			if (!Directory.Exists(path))
			{
				XLog.WriteLine(Levels.Error, $"MapManager::Load<folder>() => Missing folder {path}.");
				return;
			}
			
			XLog.Write(Levels.Info, $"Loading minimap...\t");

			try
			{
				var error = 0;
				var load = 0;

				using (var g = Graphics.FromImage(map))
				{
					int partX;
					int partY;
					var partHeight = 128;
					var partWidth = 128;
					var prefix = string.Empty;

					var occurence = !useCore ?
						Directory.GetFiles(path, $"v256_{name}*")
						: core.GetEntriesByPartialName($"v256_{name}*").Select(r => r.Name);

					if (occurence.Count() > 0)
					{
						partHeight = 256;
						partWidth = 256;
						prefix = "v256_";
					}

					partX = Width / partWidth;
					partY = Height / partHeight;

					g.Clear(Color.FromArgb(255, 120, 146, 173));

					for (int y = 0; y < partY; y++)
					{
						for (int x = 0; x < partX; x++)
						{
							var filename = $"{prefix}{name}_{y}_{x}{encoding}.jpg";

							if (!useCore)
							{
								filename = Path.Combine(path, filename);

								if (!File.Exists(filename))
								{
									filename = encoding != "" ? filename.Replace(encoding, string.Empty) : filename;
								}
							}
							else
							{
								if (!occurence.Any(r => r == filename))
								{
									filename = encoding != "" ? filename.Replace(encoding, string.Empty) : filename;
								}
							}

							if (!useCore && !File.Exists(filename) || useCore && !occurence.Any(r => r == filename))
							{
								error++;
								continue;
							}

							var buffer = new byte[0];
							if (useCore) buffer = core.GetFileBytes(filename);

							var image = useCore ? Image.FromStream(new MemoryStream(buffer)) : Image.FromFile(filename);
							g.DrawImage(image, x * partWidth, y * partHeight, partWidth, partHeight);

							load++;
						}
					}

					map.RotateFlip(RotateFlipType.Rotate180FlipX);

					Restore();
				}
				
				if (error == 0) XLog.WriteLine(Levels.Good, "Ok");
				else if (load > 0) XLog.WriteLine(Levels.Warning, "Partially ok");
				else XLog.WriteLine(Levels.Error, "Failed");

				XLog.WriteLine(Levels.Info, $"Loading the minimap completed. (Error count : {error})");
			}
			catch (Exception exception)
			{
				New();
				XLog.WriteLine(Levels.Error, "Failed");
				XLog.WriteLine(Levels.Fatal, "MapManager::Load<Exception> -> {0}", exception);
			}
		}

		/// <summary>
		/// Method used when property changed
		/// </summary>
		/// <param name="propertyName"></param>
		protected virtual void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		/// <summary>
		/// Refresh the current pictures
		/// </summary>
		public void Restore()
		{
			Copy(map);
		}
	}
}
