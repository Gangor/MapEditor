using MapEditor.Attributes;
using MapEditor.Extends;
using MapEditor.Modules;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;

namespace MapEditor.Models
{
    public class Polygon2 : INotifyPropertyChanged
    {
        private int count = 0;
        private List<K2DPosition> points = new List<K2DPosition>();

        [DisplayName("Count")]
        public int Count
        {
            get { return Points.Count; }
            set
            {
                count = value;
            }
        }

        [DisplayName("Points")]
        [PropertyGridBrowsable(true)]
        [Editor(typeof(CollectionEditorExtends), typeof(UITypeEditor))]
        public List<K2DPosition> Points
        {
            get { return points; }
            set
            {
                points = value;
                OnPropertyChanged("Points");
            }
        }

        public override string ToString()
        {
            return string.Format("Polygons x{0}", Points.Count);
        }

        /// <summary>
        /// Convert to pixel location
        /// </summary>
        /// <returns></returns>
		public PointF[] ToPoints()
		{
			var points = new List<PointF>();

			foreach (var point in Points)
			{
				points.Add(new PointF(point.X, point.Y));
			}

			return _2DUtils.UnAdjustPolygonPoint(points, 1f, true, true);
		}

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
