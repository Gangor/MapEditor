using MapEditor.Attributes;
using MapEditor.Extends;
using MapEditor.Modules;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;

namespace MapEditor.Models
{
    public class Polygon3 : INotifyPropertyChanged
    {
        private List<K3DPosition> points = new List<K3DPosition>();

        [DisplayName("Points")]
        [PropertyGridBrowsable(true)]
        [Editor(typeof(CollectionEditorExtends), typeof(UITypeEditor))]
        public List<K3DPosition> Points
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

			return _2DUtils.UnAdjustPolygonPoint(points, 1f, false, false);
		}

		protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
