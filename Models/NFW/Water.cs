using MapEditor.Attributes;
using MapEditor.Extends;
using MapEditor.Modules;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

namespace MapEditor.Models
{
    public class Water : INotifyPropertyChanged
    {
        private K3DPosition pointA = new K3DPosition();
        private K3DPosition pointB = new K3DPosition();
        private K3DPosition center = new K3DPosition();
        private int useReflect = 0;
        private int waterId = 0;

        [Browsable(false)]
        [DisplayName("Angle 1")]
        [Category("Coordonnate")]
        [Description("Different point of water location")]
        [PropertyGridBrowsable(true)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public K3DPosition PointA
        {
            get { return pointA; }
            set
            {
                pointA = value;
                OnPropertyChanged("PointA");
            }
        }

        [Browsable(false)]
        [DisplayName("Angle 2")]
        [Category("Coordonnate")]
        [Description("Different point of water location")]
        [PropertyGridBrowsable(true)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public K3DPosition PointB
        {
            get { return pointB; }
            set
            {
                pointB = value;
                OnPropertyChanged("PointB");
            }
        }

        [Browsable(false)]
        [DisplayName("Center")]
        [Category("Coordonnate")]
        [Description("Center point of water location")]
        [PropertyGridBrowsable(true)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public K3DPosition Center
        {
            get { return center; }
            set
            {
                center = value;
                OnPropertyChanged("Center");
            }
        }

        [DisplayName("Use reflection")]
        [Category("Aspect")]
        [Description("Enables or disables water reflection")]
        [PropertyGridBrowsable(true)]
        public int UseReflect
        {
            get { return useReflect; }
            set
            {
                useReflect = value;
                OnPropertyChanged("UseReflect");
            }
        }

        [DisplayName("Water Id")]
        [Category("Aspect")]
        [Description("Represents the texture or color")]
        [PropertyGridBrowsable(true)]
        public int WaterId
        {
            get { return waterId; }
            set
            {
                waterId = value;
                OnPropertyChanged("WaterId");
            }
        }

        [DisplayName("Color")]
        [Category("Aspect")]
        [Description("Setting coloring water")]
        [PropertyGridBrowsable(true)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public Color Color
        {
            get { return Color.FromArgb(WaterId); }
            set
            {
                waterId = value.ToArgb();
                OnPropertyChanged("WaterId");
            }
		}

		public event PropertyChangedEventHandler PropertyChanged;
		/// <summary>
		/// Convert to pixel location
		/// </summary>
		/// <returns></returns>
		public PointF[] ToPoints()
		{
			var points = new List<PointF>();
			points.Add(new PointF(PointA.X, PointA.Y));
			points.Add(new PointF(PointB.X, PointB.Y));
			points.Add(new PointF(Center.X, Center.Y));
			return _2DUtils.UnAdjustPolygonPoint(points, 1f, false, false);
		}

		public override string ToString()
		{
			return string.Format("Water : {0}", waterId);
		}

		protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
