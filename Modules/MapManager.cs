using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;

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
        /// Event when property changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

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
        /// <param name="pointC"></param>
        /// <param name="pointB"></param>
        public void DrawRectangle(Pen pen, PointF pointC, PointF pointB)
        {
            var pointA = new PointF(pointC.X, pointB.Y);
            var pointD = new PointF(pointB.X, pointC.Y);
            try
            {
                graphic.DrawLine(pen, pointA, pointB);
                graphic.DrawLine(pen, pointB, pointD);
                graphic.DrawLine(pen, pointD, pointC);
                graphic.DrawLine(pen, pointC, pointA);
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
        /// Make text
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
        /// Convert one pointF with rotation 180° and flip Y
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public PointF GetPointRotate180FlipY(PointF pointF)
        {
            var newPoint = new PointF(pointF.X, pointF.Y);
            newPoint.X = Width - newPoint.X;
            newPoint.Y = Height - newPoint.Y;
            newPoint.X = Width - newPoint.X;
            return newPoint;
        }

        /// <summary>
        /// Convert multiple point with rotation 180° and flip Y
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public PointF[] GetPointsRotate180FlipY(PointF[] points)
        {
            var newPoints = (PointF[])points.Clone();
            for (int i = 0; i < newPoints.Length; i++)
            {
                newPoints[i].X = Width - newPoints[i].X;
                newPoints[i].Y = Height - newPoints[i].Y;
                newPoints[i].X = Width - newPoints[i].X;
            }
            return newPoints;
        }

        /// <summary>
        /// Measure the size of string
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public SizeF MeasureString(string text, int size = 6)
        {
            return graphic.MeasureString(text, new Font("Arial", size, FontStyle.Regular));
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
        /// 
        /// <example>
        /// Version 1 : 2048 % 128 = 16
        /// Version 2 : 2048 % 256 = 8
        /// </example>
        /// 
        /// <param name="folder">The location to jpg dump</param>
        /// <param name="name">Associate name file</param>
        public void Load(string folder, string name)
        {
            New();

            if (!Directory.Exists(folder))
            {
                XLog.WriteLine(Levels.Warning, $"MapManager::Load<folder>() => Missing folder {folder}.");
                return;
            }

            try
            {
                using (var g = Graphics.FromImage(map))
                {
                    int partX;
                    int partY;
                    var partHeight = 128;
                    var partWidth = 128;
                    var version = 1;

                    var search = Directory.GetFiles(folder, $"v256_{name}*");
                    if (search.Length > 0)
                    {
                        partHeight = 256;
                        partWidth = 256;
                        version = 2;
                    }

                    partX = Width / partWidth;
                    partY = Height / partHeight;

                    g.Clear(Color.FromArgb(255, 120, 146, 173));

                    XLog.WriteLine(Levels.Info, $"Loading minimap version {version}.");

                    for (int y = 0; y < partY; y++)
                    {
                        for (int x = 0; x < partX; x++)
                        {
                            string path = Path.Combine(folder, (version == 2 ? "v256_" : "") + $"{name}_{y}_{x}.jpg");

                            if (File.Exists(path))
                            {
                                Image image = Image.FromFile(path);
                                g.DrawImage(image, x * partWidth, y * partHeight, partWidth, partHeight);
                            }
                        }
                    }

                    map.RotateFlip(RotateFlipType.Rotate180FlipX);

                    Restore();
                }
            }
            catch (Exception exception)
            {
                New();
                XLog.WriteLine(Levels.Error, "MapManager::Load<Exception> -> {0}", exception);
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
